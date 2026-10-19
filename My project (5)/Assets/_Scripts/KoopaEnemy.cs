using UnityEngine;

/// <summary>
/// Rùa Koopa Troopa chuẩn Mario với cơ chế đá mai rùa (Shell Kicking):
/// - Trạng thái bình thường: Đi tuần tra qua lại.
/// - Mario dẫm lần 1: Rùa thụt vào mai rùa (Shell). Đứng yên tại chỗ.
/// - Mario chạm vào mai từ bên hông: ĐÁ MAI RÙA TRƯỢT SIÊU TỐC!
/// - Mai rùa trượt đập vào tường tự động bật ngược lại, đè bẹp mọi quái vật trên đường!
/// - Mario dẫm lên mai rùa đang trượt: Dừng mai rùa lại.
/// </summary>
public class KoopaEnemy : MonoBehaviour
{
    [Header("--- THÔNG SỐ TỐC ĐỘ ---")]
    public float walkSpeed = 1.8f;
    public float shellSlideSpeed = 13.5f;

    [Header("--- SPRITES ---")]
    public Sprite koopaWalkSprite;
    public Sprite shellSprite;

    // Trạng thái
    public bool isShell = false;
    public bool isSliding = false;
    private float direction = -1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (shellSprite == null)
        {
#if UNITY_EDITOR
            shellSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Enemy_KoopaShell.png");
#endif
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        if (!isShell)
        {
            // Đi tuần tra bình thường
            rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
        }
        else if (isSliding)
        {
            // Trượt mai rùa với tốc độ cao
            rb.linearVelocity = new Vector2(direction * shellSlideSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Mai rùa đứng yên
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Va chạm với tường
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.6f && !collision.gameObject.CompareTag("Player"))
            {
                direction = -direction; // Bật ngược chiều
                if (isSliding && ParticleSpawner.instance != null)
                {
                    ParticleSpawner.instance.SpawnDust(contact.point, 10, Color.white);
                }
                break;
            }
        }

        // 2. Va chạm với Mario
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            // Nếu Mario đang bất tử bằng Starman
            if (MarioPowerUpManager.instance != null && MarioPowerUpManager.instance.isStarman)
            {
                DefeatByFireball();
                return;
            }

            // Kiểm tra dẫm từ trên xuống
            bool stompedFromAbove = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    stompedFromAbove = true;
                    break;
                }
            }

            if (stompedFromAbove)
            {
                if (!isShell)
                {
                    // Dẫm lần 1: Thụt vào mai rùa
                    RetreatToShell();
                    if (player != null) player.Bounce(11f);
                }
                else if (isSliding)
                {
                    // Dẫm lên mai đang trượt: Dừng trượt
                    isSliding = false;
                    if (player != null) player.Bounce(11f);
                }
                else
                {
                    // Dẫm lên mai đang đứng yên: Đá cho trượt theo hướng Mario đang nhìn
                    float kickDir = (player.transform.position.x < transform.position.x) ? 1f : -1f;
                    KickShell(kickDir);
                    if (player != null) player.Bounce(9f);
                }
                return;
            }

            // Chạm từ bên hông
            if (isShell && !isSliding)
            {
                // Đá mai rùa đứng yên!
                float kickDir = (player.transform.position.x < transform.position.x) ? 1f : -1f;
                KickShell(kickDir);
            }
            else
            {
                // Đang đi bộ hoặc mai rùa đang trượt đâm vào Mario -> Gây sát thương!
                DamagePlayer(player);
            }
            return;
        }

        // 3. Mai rùa đang trượt đâm vào quái khác -> TIÊU DIỆT KẺ ĐỊCH!
        if (isSliding)
        {
            GoombaEnemy goomba = collision.gameObject.GetComponentInParent<GoombaEnemy>();
            if (goomba != null)
            {
                goomba.Stomp();
                if (GameManager.instance != null) GameManager.instance.AddScore(200);
                return;
            }

            EnemyPatrol patrol = collision.gameObject.GetComponentInParent<EnemyPatrol>();
            if (patrol != null)
            {
                patrol.StompKill();
                if (GameManager.instance != null) GameManager.instance.AddScore(200);
                return;
            }

            KoopaEnemy otherKoopa = collision.gameObject.GetComponentInParent<KoopaEnemy>();
            if (otherKoopa != null && otherKoopa != this)
            {
                otherKoopa.DefeatByFireball();
                if (GameManager.instance != null) GameManager.instance.AddScore(200);
            }
        }
    }

    public void RetreatToShell()
    {
        isShell = true;
        isSliding = false;

        if (shellSprite != null && sr != null)
        {
            sr.sprite = shellSprite;
        }

        if (GameManager.instance != null) GameManager.instance.AddScore(100);

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 10, new Color(0.2f, 0.8f, 0.3f));
        }
    }

    public void KickShell(float kickDirection)
    {
        isSliding = true;
        direction = Mathf.Sign(kickDirection);

        if (GameManager.instance != null) GameManager.instance.AddScore(400);

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 14, Color.yellow);
        }
    }

    public void DefeatByFireball()
    {
        if (GameManager.instance != null) GameManager.instance.AddScore(200);

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 15, new Color(0.2f, 0.8f, 0.3f));
        }

        Destroy(gameObject);
    }

    private void DamagePlayer(PlayerController player)
    {
        if (MarioPowerUpManager.instance != null)
        {
            bool survived = MarioPowerUpManager.instance.TakeHit();
            if (!survived && GameManager.instance != null)
            {
                GameManager.instance.TakeDamage();
            }
        }
        else if (GameManager.instance != null)
        {
            GameManager.instance.TakeDamage();
        }
    }
}
