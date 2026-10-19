using UnityEngine;

/// <summary>
/// Sao Bất Tử (Starman) chuẩn Mario:
/// - Chui từ trong khối [?] lên.
/// - Nhảy tưng tưng theo vòng cung về phía trước.
/// - Mario nhặt vào -> Bất tử 10 giây, húc văng mọi kẻ địch!
/// </summary>
public class StarmanItem : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float bounceForce = 7f;
    private float direction = 1f;
    private Rigidbody2D rb;
    private bool isReady = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void EmergeFromBlock(Vector3 blockPos)
    {
        transform.position = blockPos;
        StartCoroutine(EmergeRoutine(blockPos));
    }

    private System.Collections.IEnumerator EmergeRoutine(Vector3 startPos)
    {
        if (rb != null) rb.simulated = false;

        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos + Vector3.up * 1f;
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = new Vector2(direction * moveSpeed, bounceForce);
        }
        isReady = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Nảy lên khi chạm đất
            if (contact.normal.y > 0.5f)
            {
                if (rb != null) rb.linearVelocity = new Vector2(direction * moveSpeed, bounceForce);
                break;
            }
            // Đổi hướng khi chạm tường
            else if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                direction = -direction;
                break;
            }
        }

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerController>() != null)
        {
            Collect();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (MarioPowerUpManager.instance != null)
        {
            MarioPowerUpManager.instance.ActivateStarman(10f);
        }

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 25, Color.cyan);
        }

        Destroy(gameObject);
    }
}
