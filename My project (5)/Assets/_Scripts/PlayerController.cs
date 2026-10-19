using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerController nâng cấp chuyên nghiệp:
/// - Coyote Time (nhảy muộn sau khi rời đất)
/// - Jump Buffering (bấm nhảy sớm trước khi chạm đất)
/// - Variable Jump Height (nhấn Space lâu = nhảy cao, nhả sớm = nhảy thấp)
/// - Gia tốc / giảm tốc mượt (trượt nhẹ khi dừng)
/// - Wall Slide & Wall Jump (trượt tường + nhảy tường)
/// - Squash & Stretch (co dãn thị giác khi nhảy/tiếp đất)
/// - Bụi Particle khi nhảy & tiếp đất
/// - Camera Shake khi tiếp đất mạnh
/// - Hệ thống bất tử (Iframe) khi bị đánh
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ========================================================================
    // DI CHUYỂN
    // ========================================================================
    [Header("--- DI CHUYỂN ---")]
    [Tooltip("Tốc độ di chuyển tối đa")]
    public float moveSpeed = 7f;

    [Tooltip("Gia tốc khi bắt đầu di chuyển (càng cao càng phản hồi nhanh)")]
    public float acceleration = 55f;

    [Tooltip("Giảm tốc khi dừng di chuyển (càng thấp càng trượt nhiều)")]
    public float deceleration = 45f;

    // ========================================================================
    // NHẢY
    // ========================================================================
    [Header("--- NHẢY ---")]
    [Tooltip("Lực nhảy")]
    public float jumpForce = 11.5f;

    [Tooltip("Coyote Time: Thời gian vẫn cho nhảy sau khi rời mặt đất (giây)")]
    public float coyoteTimeDuration = 0.12f;

    [Tooltip("Jump Buffer: Thời gian lưu lệnh nhảy trước khi tiếp đất (giây)")]
    public float jumpBufferDuration = 0.15f;

    [Tooltip("Hệ số cắt vận tốc khi nhả phím nhảy sớm (Variable Jump)")]
    public float jumpCutMultiplier = 0.45f;

    // ========================================================================
    // TRỌNG LỰC
    // ========================================================================
    [Header("--- TRỌNG LỰC & CẢM GIÁC (GAME FEEL) ---")]
    [Tooltip("Trọng lực cơ bản")]
    public float baseGravityScale = 2.2f;

    [Tooltip("Hệ số gia tốc rơi (rơi nhanh hơn, dứt khoát)")]
    public float fallMultiplier = 1.6f;

    [Tooltip("Trọng lực tối đa khi rơi (giới hạn tốc độ rơi)")]
    public float maxFallSpeed = -20f;

    // ========================================================================
    // KIỂM TRA MẶT ĐẤT
    // ========================================================================
    [Header("--- KIỂM TRA MẶT ĐẤT ---")]
    [Tooltip("Điểm kiểm tra đặt dưới chân nhân vật")]
    public Transform groundCheck;

    [Tooltip("Bán kính vòng tròn quét va chạm")]
    public float checkRadius = 0.15f;

    [Tooltip("Layer đại diện cho mặt đất")]
    public LayerMask groundLayer;

    // ========================================================================
    // WALL SLIDE & WALL JUMP
    // ========================================================================
    [Header("--- TRƯỢT TƯỜNG & NHẢY TƯỜNG ---")]
    [Tooltip("Bật/tắt cơ chế trượt tường")]
    public bool enableWallSlide = true;

    [Tooltip("Tốc độ trượt xuống khi bám tường (càng nhỏ trượt càng chậm)")]
    public float wallSlideSpeed = 2.5f;

    [Tooltip("Khoảng cách raycast kiểm tra tường")]
    public float wallCheckDistance = 0.4f;

    [Tooltip("Lực đẩy ngang khi nhảy tường")]
    public float wallJumpForceX = 9f;

    [Tooltip("Lực đẩy lên khi nhảy tường")]
    public float wallJumpForceY = 13f;

    [Tooltip("Thời gian khoá input sau khi nhảy tường (để quỹ đạo nhảy đẹp)")]
    public float wallJumpLockTime = 0.18f;

    // ========================================================================
    // KỸ NĂNG METROIDVANIA (DOUBLE JUMP & AIR DASH)
    // ========================================================================
    [Header("--- KỸ NĂNG METROIDVANIA ---")]
    [Tooltip("Cho phép Nhảy Kép")]
    public bool canDoubleJump = false;

    [Tooltip("Lực nhảy kép")]
    public float doubleJumpForce = 11.5f;

    [Tooltip("Cho phép Lướt Gió (Air Dash)")]
    public bool canAirDash = false;

    [Tooltip("Tốc độ lướt")]
    public float dashSpeed = 19f;

    [Tooltip("Thời gian lướt (giây)")]
    public float dashDuration = 0.18f;

    [Tooltip("Thời gian hồi lướt (giây)")]
    public float dashCooldown = 0.8f;

    // Trạng thái kỹ năng
    private bool hasDoubleJumped = false;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private float dashTrailTimer = 0f;

    // ========================================================================
    // SQUASH & STRETCH (CO DÃN THỊ GIÁC)
    // ========================================================================
    [Header("--- SQUASH & STRETCH ---")]
    [Tooltip("Tốc độ lerp về scale bình thường")]
    public float squashStretchSpeed = 10f;

    // ========================================================================
    // BIẾN NỘI BỘ
    // ========================================================================
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;

    public bool IsGrounded => isGrounded;
    public float HorizontalInput => horizontalInput;
    public bool IsFacingRight => isFacingRight;

    // Coyote Time
    private float coyoteTimeCounter;

    // Jump Buffer
    private float jumpBufferCounter;
    private bool jumpConsumed; // Đã dùng hết lần nhảy trong lần rời đất này chưa

    // Variable Jump
    private bool jumpReleased;

    // Wall
    private bool isTouchingWall;
    private bool isWallSliding;
    private float wallJumpLockCounter;
    private int wallJumpDirection; // 1 = phải, -1 = trái

    // Squash & Stretch
    private float baseScaleX;
    private float baseScaleY;
    private float ssX = 1f; // stretch/squash X multiplier
    private float ssY = 1f; // stretch/squash Y multiplier

    // Landing Detection
    private bool wasGroundedLastFrame;
    private float verticalVelocityLastFrame;

    // Invincibility (Iframe)
    private bool _isInvincible;
    public bool isInvincible => _isInvincible;
    private Coroutine invincibilityCoroutine;

    // ========================================================================
    // KHỞI TẠO
    // ========================================================================
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = baseGravityScale;
        }

        // Căn chỉnh lại BoxCollider2D vừa khít bàn chân
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(0.55f, 0.62f);
            col.offset = new Vector2(0, 0f);
        }

        // Căn chỉnh GroundCheck sát đáy chân
        if (groundCheck != null)
        {
            groundCheck.localPosition = new Vector3(0, -0.32f, 0);
        }

        // Sửa lỗi sprite đen trong URP 2D
        if (sr != null)
        {
            Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                              ?? Shader.Find("Sprites/Default");
            if (unlitShader != null)
            {
                sr.material = new Material(unlitShader);
            }
        }

        // Lưu scale gốc cho squash & stretch
        baseScaleX = Mathf.Abs(transform.localScale.x);
        baseScaleY = Mathf.Abs(transform.localScale.y);

        // Tự động gắn MarioPowerUpManager
        if (GetComponent<MarioPowerUpManager>() == null)
        {
            gameObject.AddComponent<MarioPowerUpManager>();
        }
    }

    // Animator Parameters Cache
    private bool hasIsRunningParam;
    private bool hasIsJumpingParam;
    private bool hasIsWallSlidingParam;
    private bool hasYVelocityParam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (anim != null)
        {
            foreach (var param in anim.parameters)
            {
                if (param.name == "isRunning") hasIsRunningParam = true;
                if (param.name == "isJumping") hasIsJumpingParam = true;
                if (param.name == "isWallSliding") hasIsWallSlidingParam = true;
                if (param.name == "yVelocity") hasYVelocityParam = true;
            }
        }
    }

    // ========================================================================
    // UPDATE — NHẬN INPUT & XỬ LÝ LOGIC NHẢY
    // ========================================================================
    void Update()
    {
        // --- Đọc input ---
        ReadInput(out float rawHorizontal, out bool jumpPressed, out bool jumpUp, out bool dashPressed);

        // --- Hồi chiêu Lướt Gió ---
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // --- Kích hoạt Lướt Gió (Air Dash) ---
        if (dashPressed && canAirDash && dashCooldownTimer <= 0 && !isDashing)
        {
            StartDash();
        }

        // --- Xử lý khi đang Dash ---
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            dashTrailTimer -= Time.deltaTime;

            if (dashTrailTimer <= 0)
            {
                dashTrailTimer = 0.04f;
                if (ParticleSpawner.instance != null)
                {
                    ParticleSpawner.instance.SpawnDust(transform.position, 3, new Color(0.3f, 0.8f, 1f, 0.7f));
                }
            }

            float dashDir = isFacingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);
            rb.gravityScale = 0f;

            if (dashTimeLeft <= 0)
            {
                isDashing = false;
                rb.gravityScale = baseGravityScale;
                rb.linearVelocity = new Vector2(dashDir * moveSpeed, 0f);
            }
            return;
        }

        // Chỉ áp dụng input ngang nếu không bị khoá bởi wall jump
        if (wallJumpLockCounter > 0)
        {
            wallJumpLockCounter -= Time.deltaTime;
        }
        else
        {
            horizontalInput = rawHorizontal;
        }

        // --- Kiểm tra mặt đất ---
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        }

        // Reset quyền nhảy kép khi tiếp đất
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        // --- Kiểm tra tường (Wall Detection) ---
        if (enableWallSlide)
        {
            DetectWall();
        }

        // --- Coyote Time ---
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration;
            jumpConsumed = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // --- Jump Buffer ---
        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferDuration;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // --- NHẢY ---
        // 1. Wall Jump (ưu tiên kiểm tra trước)
        if (jumpPressed && isWallSliding)
        {
            PerformWallJump();
        }
        // 2. Normal Jump (Coyote Time + Jump Buffer)
        else if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !jumpConsumed)
        {
            PerformJump();
        }
        // 3. Double Jump (Nhảy kép giữa không trung)
        else if (jumpPressed && !isGrounded && coyoteTimeCounter <= 0 && !isWallSliding && canDoubleJump && !hasDoubleJumped)
        {
            PerformDoubleJump();
        }

        // --- VARIABLE JUMP: Nhả Space sớm → cắt vận tốc lên ---
        if (jumpUp && rb.linearVelocity.y > 0 && !isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            coyoteTimeCounter = 0; // Hết quyền nhảy
        }

        // --- LANDING DETECTION ---
        DetectLanding();

        // --- FLIP ---
        if (wallJumpLockCounter <= 0)
        {
            Flip();
        }

        // --- SQUASH & STRETCH ---
        UpdateSquashStretch();

        // --- ANIMATION ---
        UpdateAnimation();

        // Lưu trạng thái frame trước
        wasGroundedLastFrame = isGrounded;
        verticalVelocityLastFrame = rb != null ? rb.linearVelocity.y : 0f;
    }

    // ========================================================================
    // FIXED UPDATE — VẬT LÝ
    // ========================================================================
    void FixedUpdate()
    {
        // --- Di chuyển có gia tốc ---
        float targetVelX = horizontalInput * moveSpeed;
        float velDiff = targetVelX - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(horizontalInput) > 0.01f) ? acceleration : deceleration;
        float force = velDiff * accelRate;

        // Dùng AddForce thay vì set velocity trực tiếp → cảm giác vật lý hơn
        rb.AddForce(force * Vector2.right * Time.fixedDeltaTime, ForceMode2D.Impulse);

        // Clamp tốc độ ngang để không vượt quá moveSpeed
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed);
        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);

        // --- TRỌNG LỰC NÂNG CAO ---
        if (rb.linearVelocity.y < 0)
        {
            // Rơi xuống → tăng trọng lực
            rb.gravityScale = baseGravityScale * fallMultiplier;

            // Giới hạn tốc độ rơi tối đa
            if (rb.linearVelocity.y < maxFallSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
            }
        }
        else
        {
            rb.gravityScale = baseGravityScale;
        }

        // --- WALL SLIDE ---
        if (isWallSliding)
        {
            // Giới hạn tốc độ rơi khi trượt tường
            if (rb.linearVelocity.y < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            }
        }
    }

    // ========================================================================
    // ĐỌC INPUT (Hỗ trợ cả 2 hệ thống Input)
    // ========================================================================
    void ReadInput(out float horizontal, out bool jumpDown, out bool jumpUp, out bool dashDown)
    {
        horizontal = 0f;
        jumpDown = false;
        jumpUp = false;
        dashDown = false;

#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null)
        {
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal -= 1f;

            if (kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
                jumpDown = true;

            if (kb.spaceKey.wasReleasedThisFrame || kb.wKey.wasReleasedThisFrame || kb.upArrowKey.wasReleasedThisFrame)
                jumpUp = true;

            if (kb.leftShiftKey.wasPressedThisFrame || kb.rightShiftKey.wasPressedThisFrame || kb.cKey.wasPressedThisFrame)
                dashDown = true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (horizontal == 0f)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }
        if (Input.GetButtonDown("Jump"))
            jumpDown = true;
        if (Input.GetButtonUp("Jump"))
            jumpUp = true;
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.C))
            dashDown = true;
#endif
    }

    // ========================================================================
    // NHẢY BÌNH THƯỜNG
    // ========================================================================
    void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        jumpBufferCounter = 0;
        coyoteTimeCounter = 0;
        jumpConsumed = true;

        // Hiệu ứng stretch (kéo dài) khi nhảy
        ssX = 0.8f;
        ssY = 1.25f;

        // Bụi nhảy
        if (groundCheck != null)
        {
            ParticleSpawner.SpawnJumpDust(groundCheck.position);
        }

        // Âm thanh nhảy
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayJumpSFX();
        }
    }

    // ========================================================================
    // NHẢY KÉP (DOUBLE JUMP)
    // ========================================================================
    void PerformDoubleJump()
    {
        hasDoubleJumped = true;
        jumpBufferCounter = 0;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);

        // Hiệu ứng stretch
        ssX = 0.75f;
        ssY = 1.35f;

        // Bụi năng lượng nhảy kép màu lam sáng
        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 16, new Color(0.3f, 1f, 0.9f));
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayJumpSFX();
        }
    }

    // ========================================================================
    // LƯỚT GIÓ (AIR DASH)
    // ========================================================================
    void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;
        dashTrailTimer = 0f;

        // Bất tử ngắn trong lúc lướt
        StartInvincibility(dashDuration + 0.1f);

        // Co dãn thân mình
        ssX = 1.4f;
        ssY = 0.7f;

        // Bụi lướt
        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 18, new Color(0.2f, 0.7f, 1f));
        }

        CameraFollow cam = Object.FindFirstObjectByType<CameraFollow>();
        if (cam != null)
        {
            cam.Shake(0.06f, 0.15f);
        }
    }

    // ========================================================================
    // NHẢY TƯỜNG (WALL JUMP)
    // ========================================================================
    void PerformWallJump()
    {
        // Hướng nhảy = ngược chiều tường
        wallJumpDirection = isFacingRight ? -1 : 1;

        rb.linearVelocity = new Vector2(wallJumpForceX * wallJumpDirection, wallJumpForceY);

        // Khoá input ngang tạm thời để quỹ đạo nhảy tường đẹp
        wallJumpLockCounter = wallJumpLockTime;

        // Lật hướng
        isFacingRight = wallJumpDirection > 0;

        // Squash & stretch nhẹ
        ssX = 0.85f;
        ssY = 1.15f;

        // Bụi tường
        Vector3 dustPos = transform.position + (isFacingRight ? Vector3.left : Vector3.right) * 0.3f;
        ParticleSpawner.SpawnJumpDust(dustPos);

        // Âm thanh
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayJumpSFX();
        }

        jumpBufferCounter = 0;

        Debug.Log("<color=cyan><b>[Wall Jump]</b> Nhảy tường!</color>");
    }

    // ========================================================================
    // PHÁT HIỆN TƯỜNG
    // ========================================================================
    void DetectWall()
    {
        if (!enableWallSlide)
        {
            isTouchingWall = false;
            isWallSliding = false;
            return;
        }

        // Raycast theo hướng đang nhìn
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        isTouchingWall = Physics2D.Raycast(transform.position, direction, wallCheckDistance, groundLayer);

        // Wall Slide: chạm tường + không ở trên đất + đang đẩy vào tường
        bool pushingIntoWall = (isFacingRight && horizontalInput > 0.1f) || (!isFacingRight && horizontalInput < -0.1f);
        isWallSliding = isTouchingWall && !isGrounded && pushingIntoWall && rb.linearVelocity.y <= 0;
    }

    // ========================================================================
    // PHÁT HIỆN TIẾP ĐẤT
    // ========================================================================
    void DetectLanding()
    {
        if (isGrounded && !wasGroundedLastFrame)
        {
            // Vừa tiếp đất!
            float fallSpeed = Mathf.Abs(verticalVelocityLastFrame);

            // Squash (bẹp xuống) tỷ lệ với tốc độ rơi
            float squashIntensity = Mathf.Clamp01(fallSpeed / 15f);
            ssX = 1f + 0.35f * squashIntensity; // Phình ngang
            ssY = 1f - 0.3f * squashIntensity;  // Bẹp dọc

            // Bụi tiếp đất
            if (groundCheck != null)
            {
                ParticleSpawner.SpawnLandingDust(groundCheck.position, squashIntensity);
            }

            // Camera Shake (chỉ khi rơi từ cao)
            if (fallSpeed > 8f)
            {
                CameraFollow cam = Object.FindFirstObjectByType<CameraFollow>();
                if (cam != null)
                {
                    cam.Shake(0.08f * squashIntensity, 0.15f);
                }
            }

            // Reset combo khi chạm đất
            if (GameManager.instance != null)
            {
                GameManager.instance.ResetCombo();
            }
        }
    }

    // ========================================================================
    // FLIP (Quay mặt nhân vật)
    // ========================================================================
    void Flip()
    {
        if (horizontalInput > 0.01f && !isFacingRight)
        {
            isFacingRight = true;
        }
        else if (horizontalInput < -0.01f && isFacingRight)
        {
            isFacingRight = false;
        }
        // Scale thực tế được cập nhật trong UpdateSquashStretch()
    }

    // ========================================================================
    // SQUASH & STRETCH
    // ========================================================================
    void UpdateSquashStretch()
    {
        // Lerp dần về scale bình thường (1, 1)
        ssX = Mathf.Lerp(ssX, 1f, squashStretchSpeed * Time.deltaTime);
        ssY = Mathf.Lerp(ssY, 1f, squashStretchSpeed * Time.deltaTime);

        // Nhẹ stretch khi đang rơi
        if (!isGrounded && rb != null && rb.linearVelocity.y < -3f)
        {
            float fallStretch = Mathf.Clamp01(Mathf.Abs(rb.linearVelocity.y) / 15f);
            ssX = Mathf.Lerp(ssX, 1f - 0.1f * fallStretch, 3f * Time.deltaTime);
            ssY = Mathf.Lerp(ssY, 1f + 0.15f * fallStretch, 3f * Time.deltaTime);
        }

        // Áp dụng scale: base * squashStretch * faceDirection
        float dirX = isFacingRight ? 1f : -1f;
        transform.localScale = new Vector3(
            baseScaleX * ssX * dirX,
            baseScaleY * ssY,
            transform.localScale.z
        );
    }

    // ========================================================================
    // ANIMATION
    // ========================================================================
    void UpdateAnimation()
    {
        if (anim != null)
        {
            if (hasIsRunningParam) anim.SetBool("isRunning", Mathf.Abs(horizontalInput) > 0.01f);
            if (hasIsJumpingParam) anim.SetBool("isJumping", !isGrounded);
            if (hasIsWallSlidingParam) anim.SetBool("isWallSliding", isWallSliding);
            if (hasYVelocityParam) anim.SetFloat("yVelocity", rb != null ? rb.linearVelocity.y : 0f);
        }
    }

    // ========================================================================
    // BOUNCE — NẨY KHI DẪM ĐẦU QUÁI / LÒ XO
    // ========================================================================
    public void Bounce(float force = 10f)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);

            // Stretch lên khi nảy
            ssX = 0.8f;
            ssY = 1.2f;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayJumpSFX();
            }
        }
    }

    // ========================================================================
    // BẤT TỬ (IFRAME) — Nhấp nháy khi bị đánh
    // ========================================================================
    public void StartInvincibility(float duration = 1.5f)
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }
        invincibilityCoroutine = StartCoroutine(InvincibilityRoutine(duration));
    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        _isInvincible = true;
        float timer = 0f;
        float blinkInterval = 0.1f;

        while (timer < duration)
        {
            // Nhấp nháy sprite
            if (sr != null)
            {
                sr.enabled = !sr.enabled;
            }

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // Đảm bảo sprite hiển thị lại
        if (sr != null)
        {
            sr.enabled = true;
        }
        _isInvincible = false;
        invincibilityCoroutine = null;
    }

    // ========================================================================
    // DỊCH CHUYỂN ĐẾN VỊ TRÍ (cho checkpoint respawn)
    // ========================================================================
    public void TeleportTo(Vector3 position)
    {
        transform.position = position;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // ========================================================================
    // GIZMOS
    // ========================================================================
    private void OnDrawGizmosSelected()
    {
        // GroundCheck
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }

        // Wall Check Raycast
        if (enableWallSlide)
        {
            Gizmos.color = Color.blue;
            Vector3 dir = (isFacingRight ? Vector3.right : Vector3.left) * wallCheckDistance;
            Gizmos.DrawLine(transform.position, transform.position + dir);
        }
    }
}
