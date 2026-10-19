using UnityEngine;

/// <summary>
/// Quái Nấm Goomba chuẩn Mario:
/// - Đi tuần tra qua lại.
/// - Mario dẫm từ trên xuống -> Goomba bị bẹp dúm (squashed flat) và biến mất sau 0.5s!
/// - Mario chạm từ hông -> Mario bị thương (teo nhỏ hoặc mất máu).
/// </summary>
public class GoombaEnemy : MonoBehaviour
{
    public float moveSpeed = 1.8f;
    private float direction = -1f;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (isDead || rb == null) return;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // Nếu chạm tường hoặc mép gờ -> quay đầu
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.6f && !collision.gameObject.CompareTag("Player"))
            {
                direction = -direction;
                break;
            }
        }

        // Tương tác với Mario
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            // Nếu Mario đang trong trạng thái Sao Bất Tử (Starman)
            if (MarioPowerUpManager.instance != null && MarioPowerUpManager.instance.isStarman)
            {
                DefeatByStar();
                return;
            }

            // Kiểm tra dẫm từ trên xuống
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f) // Player đang đè xuống đầu quái
                {
                    Stomp();
                    if (player != null) player.Bounce(10.5f);
                    return;
                }
            }

            // Chạm từ hông -> Gây sát thương cho Mario
            DamagePlayer(player);
        }
    }

    public void Stomp()
    {
        if (isDead) return;
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        if (col != null) col.enabled = false;

        // Co bẹp sprite Goomba chuẩn Mario
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.3f, 1f);

        if (GameManager.instance != null) GameManager.instance.AddScore(100);

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 8, new Color(0.7f, 0.4f, 0.15f));
        }

        Destroy(gameObject, 0.45f);
    }

    public void DefeatByStar()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.instance != null) GameManager.instance.AddScore(200);

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 16, Color.yellow);
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
