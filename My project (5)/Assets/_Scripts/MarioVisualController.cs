using UnityEngine;

/// <summary>
/// Quản lý hình ảnh và hoạt ảnh nhân vật Mario chuẩn Nintendo:
/// - Small Mario: Đứng yên (Idle), Chạy 3 nhịp (Run 1-3), Nhảy giơ tay đấm (Jump), Phanh gấp (Skid).
/// - Super Mario: Dáng người cao 2 khối, Chạy, Nhảy, Cúi người (Crouch).
/// - Fire Mario: Áo trắng yếm đỏ, tư thế vung tay bắn cầu lửa (Shoot).
/// - Starman: Hiệu ứng nhấp nháy cầu vồng rực rỡ.
/// </summary>
public class MarioVisualController : MonoBehaviour
{
    public static MarioVisualController instance;

    [Header("--- SMALL MARIO SPRITES (16x16) ---")]
    public Sprite smallIdle;
    public Sprite smallRun1;
    public Sprite smallRun2;
    public Sprite smallRun3;
    public Sprite smallJump;
    public Sprite smallSkid;

    [Header("--- SUPER MARIO SPRITES (16x32) ---")]
    public Sprite superIdle;
    public Sprite superRun1;
    public Sprite superRun2;
    public Sprite superRun3;
    public Sprite superJump;
    public Sprite superCrouch;

    [Header("--- FIRE MARIO SPRITES (16x32 ÁO TRẮNG YẾM ĐỎ) ---")]
    public Sprite fireIdle;
    public Sprite fireRun1;
    public Sprite fireRun2;
    public Sprite fireRun3;
    public Sprite fireJump;
    public Sprite fireShoot;

    [Header("--- THIẾT LẬP HOẠT ẢNH ---")]
    public float runAnimSpeed = 12f; // Tần suất đổi frame theo tốc độ di chuyển

    // Thành phần liên kết
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private PlayerController playerCtrl;
    private MarioPowerUpManager powerManager;
    private Animator animator;

    // Bộ đếm thời gian
    private float runTimer = 0f;
    private int runFrame = 0;
    private float shootPoseTimer = 0f;

    private void Awake()
    {
        instance = this;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerCtrl = GetComponent<PlayerController>();
        powerManager = GetComponent<MarioPowerUpManager>();

        // Tắt Animator controller cũ nếu có để tránh ghi đè sprite
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        LoadDefaultMarioSprites();
    }

    private void Start()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        if (shootPoseTimer > 0)
        {
            shootPoseTimer -= Time.deltaTime;
        }

        UpdateVisuals();
        UpdateStarmanEffect();
    }

    /// <summary>
    /// Kích hoạt tư thế giơ tay ném cầu lửa
    /// </summary>
    public void TriggerFireShootPose()
    {
        shootPoseTimer = 0.18f;
    }

    private void UpdateVisuals()
    {
        if (sr == null || rb == null) return;

        MarioPowerUpManager.MarioForm form = MarioPowerUpManager.MarioForm.Small;
        if (powerManager != null)
        {
            form = powerManager.currentForm;
        }

        bool isGrounded = playerCtrl != null ? playerCtrl.IsGrounded : true;
        float hInput = playerCtrl != null ? playerCtrl.HorizontalInput : 0f;
        float vx = rb.linearVelocity.x;
        float absVx = Mathf.Abs(vx);

        // Kiểm tra phanh gấp (Skidding)
        bool isSkidding = isGrounded && ((vx > 0.6f && hInput < -0.1f) || (vx < -0.6f && hInput > 0.1f));

        // Kiểm tra cúi người (Crouching cho Super/Fire Mario)
        bool isCrouching = false;
        if (form != MarioPowerUpManager.MarioForm.Small && isGrounded)
        {
#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb != null && (kb.sKey.isPressed || kb.downArrowKey.isPressed)) isCrouching = true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) isCrouching = true;
#endif
        }

        // Cập nhật bộ đếm chạy
        if (isGrounded && absVx > 0.15f && !isSkidding)
        {
            runTimer += Time.deltaTime * Mathf.Max(absVx * 2.2f, 8f);
            if (runTimer >= 1f)
            {
                runTimer -= 1f;
                runFrame = (runFrame + 1) % 3;
            }
        }
        else
        {
            runTimer = 0f;
            runFrame = 0;
        }

        // Chọn Sprite theo trạng thái
        Sprite targetSprite = null;

        switch (form)
        {
            case MarioPowerUpManager.MarioForm.Small:
                if (!isGrounded)
                {
                    targetSprite = smallJump != null ? smallJump : smallIdle;
                }
                else if (isSkidding && smallSkid != null)
                {
                    targetSprite = smallSkid;
                }
                else if (absVx > 0.2f)
                {
                    targetSprite = GetRunSprite(smallRun1, smallRun2, smallRun3, smallIdle);
                }
                else
                {
                    targetSprite = smallIdle;
                }
                break;

            case MarioPowerUpManager.MarioForm.Super:
                if (isCrouching && superCrouch != null)
                {
                    targetSprite = superCrouch;
                }
                else if (!isGrounded)
                {
                    targetSprite = superJump != null ? superJump : superIdle;
                }
                else if (absVx > 0.2f)
                {
                    targetSprite = GetRunSprite(superRun1, superRun2, superRun3, superIdle);
                }
                else
                {
                    targetSprite = superIdle;
                }
                break;

            case MarioPowerUpManager.MarioForm.Fire:
                if (shootPoseTimer > 0 && fireShoot != null)
                {
                    targetSprite = fireShoot;
                }
                else if (isCrouching && superCrouch != null)
                {
                    targetSprite = superCrouch;
                }
                else if (!isGrounded)
                {
                    targetSprite = fireJump != null ? fireJump : fireIdle;
                }
                else if (absVx > 0.2f)
                {
                    targetSprite = GetRunSprite(fireRun1, fireRun2, fireRun3, fireIdle);
                }
                else
                {
                    targetSprite = fireIdle;
                }
                break;
        }

        if (targetSprite != null && sr.sprite != targetSprite)
        {
            sr.sprite = targetSprite;
        }
    }

    private Sprite GetRunSprite(Sprite r1, Sprite r2, Sprite r3, Sprite fallback)
    {
        Sprite s = (runFrame == 0) ? r1 : ((runFrame == 1) ? r2 : r3);
        return s != null ? s : fallback;
    }

    private void UpdateStarmanEffect()
    {
        if (powerManager != null && powerManager.isStarman)
        {
            // Hiệu ứng cầu vồng chớp nháy siêu tốc khi ăn sao
            float hue = Mathf.Repeat(Time.time * 6.5f, 1f);
            sr.color = Color.HSVToRGB(hue, 0.9f, 1f);
        }
    }

    /// <summary>
    /// Nạp sprite mặc định từ thư mục Assets/Sprites/Mario/ nếu chưa được kéo vào Inspector
    /// </summary>
    public void LoadDefaultMarioSprites()
    {
#if UNITY_EDITOR
        if (smallIdle == null) smallIdle = EditorLoadSprite("Assets/Sprites/Mario/Mario_Small_Idle.png");
        if (smallRun1 == null) smallRun1 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Small_Run1.png");
        if (smallRun2 == null) smallRun2 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Small_Run2.png");
        if (smallRun3 == null) smallRun3 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Small_Run3.png");
        if (smallJump == null) smallJump = EditorLoadSprite("Assets/Sprites/Mario/Mario_Small_Jump.png");
        if (smallSkid == null) smallSkid = EditorLoadSprite("Assets/Sprites/Mario/Mario_Small_Skid.png");

        if (superIdle == null) superIdle = EditorLoadSprite("Assets/Sprites/Mario/Mario_Super_Idle.png");
        if (superRun1 == null) superRun1 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Super_Run1.png");
        if (superRun2 == null) superRun2 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Super_Run2.png");
        if (superRun3 == null) superRun3 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Super_Run3.png");
        if (superJump == null) superJump = EditorLoadSprite("Assets/Sprites/Mario/Mario_Super_Jump.png");
        if (superCrouch == null) superCrouch = EditorLoadSprite("Assets/Sprites/Mario/Mario_Super_Crouch.png");

        if (fireIdle == null) fireIdle = EditorLoadSprite("Assets/Sprites/Mario/Mario_Fire_Idle.png");
        if (fireRun1 == null) fireRun1 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Fire_Run1.png");
        if (fireRun2 == null) fireRun2 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Fire_Run2.png");
        if (fireRun3 == null) fireRun3 = EditorLoadSprite("Assets/Sprites/Mario/Mario_Fire_Run3.png");
        if (fireJump == null) fireJump = EditorLoadSprite("Assets/Sprites/Mario/Mario_Fire_Jump.png");
        if (fireShoot == null) fireShoot = EditorLoadSprite("Assets/Sprites/Mario/Mario_Fire_Shoot.png");
#endif
    }

#if UNITY_EDITOR
    private static Sprite EditorLoadSprite(string path)
    {
        Sprite s = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (s != null) return s;

        var all = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
        if (all != null)
        {
            foreach (var o in all)
            {
                if (o is Sprite sp) return sp;
            }
        }
        return null;
    }
#endif
}
