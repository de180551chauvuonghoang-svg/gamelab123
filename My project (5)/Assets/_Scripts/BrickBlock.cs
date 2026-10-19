using UnityEngine;
using System.Collections;

/// <summary>
/// Khối Gạch (Brick Block) chuẩn Mario:
/// - Small Mario húc đầu từ dưới lên -> Khối nảy tưng lên rồi rớt xuống. Quái đứng trên khối sẽ bị hất văng!
/// - Super/Fire Mario húc đầu từ dưới lên -> Khối gạch vỡ vụn thành 4 mảnh bay tán loạn!
/// </summary>
public class BrickBlock : MonoBehaviour
{
    private SpriteRenderer sr;
    private Vector3 originalPos;
    private bool isBumping = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBumping) return;

        // Kiểm tra xem Player có húc đầu vào đáy khối gạch không
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // contact.normal.y > 0.5f nghĩa là đối tượng va chạm đang đi từ dưới lên
            if (contact.normal.y > 0.5f && (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerController>() != null))
            {
                HitBlock();
                break;
            }
        }
    }

    private void HitBlock()
    {
        bool isBigMario = (MarioPowerUpManager.instance != null && MarioPowerUpManager.instance.currentForm != MarioPowerUpManager.MarioForm.Small);

        if (isBigMario)
        {
            // Super / Fire Mario: Phá nát khối gạch!
            if (GameManager.instance != null) GameManager.instance.AddScore(50);

            BlockDebris.SpawnDebris(transform.position, sr != null ? sr.sprite : null, sr != null ? sr.material : null);

            // Bụi vỡ
            if (ParticleSpawner.instance != null)
            {
                ParticleSpawner.instance.SpawnDust(transform.position, 10, new Color(0.85f, 0.4f, 0.15f));
            }

            Destroy(gameObject);
        }
        else
        {
            // Small Mario: Nảy khối gạch lên rồi rớt xuống
            StartCoroutine(BumpRoutine());
        }
    }

    private IEnumerator BumpRoutine()
    {
        isBumping = true;
        float bumpHeight = 0.25f;
        float duration = 0.08f;

        // Nảy lên
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = originalPos + Vector3.up * Mathf.Lerp(0, bumpHeight, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Kiểm tra hất văng kẻ địch đang đứng trên nóc khối gạch
        Collider2D hitAbove = Physics2D.OverlapBox(originalPos + Vector3.up * 0.7f, new Vector2(0.8f, 0.4f), 0);
        if (hitAbove != null)
        {
            EnemyPatrol patrol = hitAbove.GetComponentInParent<EnemyPatrol>();
            if (patrol != null) patrol.StompKill();

            GoombaEnemy goomba = hitAbove.GetComponentInParent<GoombaEnemy>();
            if (goomba != null) goomba.Stomp();

            KoopaEnemy koopa = hitAbove.GetComponentInParent<KoopaEnemy>();
            if (koopa != null) koopa.DefeatByFireball();
        }

        // Rớt về vị trí cũ
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = originalPos + Vector3.up * Mathf.Lerp(bumpHeight, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        isBumping = false;
    }
}
