using UnityEngine;

/// <summary>
/// Cầu Lửa chuẩn Mario:
/// - Bay ngang với tốc độ cao.
/// - Đập xuống đất tự động nảy tưng tưng (bouncing).
/// - Chạm tường hoặc kẻ địch -> phát nổ, tiêu diệt kẻ địch!
/// </summary>
public class Fireball : MonoBehaviour
{
    public float speed = 9.5f;
    public float bounceForce = 7f;
    public float lifetime = 3f;

    private Rigidbody2D rb;
    private float direction = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(float dir)
    {
        direction = Mathf.Sign(dir);
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * speed, -2f);
        }
    }

    private void Update()
    {
        // Duy trì vận tốc ngang ổn định
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }

        // Xoay tròn cầu lửa khi bay
        transform.Rotate(0, 0, -direction * 720f * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Nếu chạm mặt sàn từ bên dưới (mặt sàn đỡ đạn)
            if (contact.normal.y > 0.5f)
            {
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(direction * speed, bounceForce);
                }
                return;
            }
            // Nếu chạm tường bên hông hoặc trần nhà -> nổ biến mất
            else if (Mathf.Abs(contact.normal.x) > 0.5f || contact.normal.y < -0.5f)
            {
                Explode();
                return;
            }
        }

        // Kiểm tra xem có trúng quái vật không
        CheckHitEnemy(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckHitEnemy(other.gameObject);
    }

    private void CheckHitEnemy(GameObject go)
    {
        // Kiểm tra Goomba, Koopa, EnemyPatrol
        EnemyPatrol patrol = go.GetComponentInParent<EnemyPatrol>();
        if (patrol != null)
        {
            patrol.StompKill();
            Explode();
            return;
        }

        KoopaEnemy koopa = go.GetComponentInParent<KoopaEnemy>();
        if (koopa != null)
        {
            koopa.DefeatByFireball();
            Explode();
            return;
        }

        GoombaEnemy goomba = go.GetComponentInParent<GoombaEnemy>();
        if (goomba != null)
        {
            goomba.Stomp();
            Explode();
            return;
        }
    }

    private void Explode()
    {
        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 12, new Color(1f, 0.6f, 0.1f));
        }
        Destroy(gameObject);
    }
}
