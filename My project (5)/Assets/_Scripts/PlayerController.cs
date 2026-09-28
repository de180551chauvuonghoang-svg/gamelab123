using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("--- THIẾT LẬP DI CHUYỂN ---")]
    [Tooltip("Tốc độ di chuyển ngang của nhân vật")]
    public float moveSpeed = 7f;
    
    [Tooltip("Lực nhảy của nhân vật (vừa vặn để qua chướng ngại vật)")]
    public float jumpForce = 11.5f;

    [Header("--- TRỌNG LỰC & CẢM GIÁC NHẢY (GAME FEEL) ---")]
    [Tooltip("Trọng lực cơ bản (giúp nhân vật bớt bồng bềnh như bay trên cung trăng)")]
    public float baseGravityScale = 2.2f;

    [Tooltip("Hệ số gia tốc khi rơi (giúp rơi xuống nhanh, dứt khoát hơn)")]
    public float fallMultiplier = 1.6f;

    [Header("--- KIỂM TRA MẶT ĐẤT ---")]
    [Tooltip("Điểm kiểm tra đặt dưới chân nhân vật")]
    public Transform groundCheck;
    
    [Tooltip("Bán kính vòng tròn quét va chạm")]
    public float checkRadius = 0.15f;
    
    [Tooltip("Layer đại diện cho mặt đất")]
    public LayerMask groundLayer;

    // Các biến nội bộ
    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = baseGravityScale;
        }

        // Tự động căn chỉnh lực nhảy vừa vặn
        jumpForce = 11.5f;

        // 1. Căn chỉnh lại BoxCollider2D vừa khít bàn chân để nhân vật chạm sàn đất chuẩn xác
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(0.55f, 0.62f);
            col.offset = new Vector2(0, 0f);
        }

        // 2. Căn chỉnh vị trí GroundCheck sát đáy chân
        if (groundCheck != null)
        {
            groundCheck.localPosition = new Vector3(0, -0.32f, 0);
        }

        // 3. Sửa lỗi nhân vật bị đen bằng cách dùng Shader Unlit (không bị ảnh hưởng bởi bóng tối)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") 
                              ?? Shader.Find("Sprites/Default");
            if (unlitShader != null)
            {
                sr.material = new Material(unlitShader);
            }
        }
    }

    void Start()
    {
        // Lấy tham chiếu đến các component trên cùng GameObject
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Nhận tín hiệu phím di chuyển ngang & phím nhảy tương thích cả 2 hệ thống Input của Unity
        horizontalInput = 0f;
        bool jumpPressed = false;

#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null)
        {
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontalInput += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontalInput -= 1f;
            if (kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
            {
                jumpPressed = true;
            }
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (horizontalInput == 0f)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
#endif

        // 2. Sử dụng Physics2D.OverlapCircle để quét xem chân có chạm đất không
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        }

        // 3. Cơ chế nhảy: Chỉ nhảy khi bấm phím Jump VÀ đang tiếp đất
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 4. Tự động quay mặt theo hướng di chuyển
        Flip();

        // 5. Cập nhật các thông số cho Animator Controller
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // Áp dụng vận tốc vật lý trong FixedUpdate để đảm bảo mượt mà
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Kỹ thuật Better Jump: Tăng tốc độ rơi khi nhân vật đang đi xuống để tiếp đất nhanh, dứt khoát
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravityScale * fallMultiplier;
        }
        else
        {
            rb.gravityScale = baseGravityScale;
        }
    }

    void Flip()
    {
        // Đang đi sang phải nhưng mặt đang quay trái -> Lật sang phải
        if (horizontalInput > 0 && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 scaler = transform.localScale;
            scaler.x = Mathf.Abs(scaler.x);
            transform.localScale = scaler;
        }
        // Đang đi sang trái nhưng mặt đang quay phải -> Lật sang trái
        else if (horizontalInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            Vector3 scaler = transform.localScale;
            scaler.x = -Mathf.Abs(scaler.x);
            transform.localScale = scaler;
        }
    }

    void UpdateAnimation()
    {
        if (anim != null)
        {
            // isRunning = true khi có bấm di chuyển (horizontalInput khác 0)
            anim.SetBool("isRunning", horizontalInput != 0);
            
            // isJumping = true khi nhân vật đang ở trên không (không chạm đất)
            anim.SetBool("isJumping", !isGrounded);
        }
    }

    // Hiển thị vòng tròn GroundCheck trong Scene để dễ dàng căn chỉnh bằng mắt
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
