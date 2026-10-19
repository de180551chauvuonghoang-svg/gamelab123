using UnityEngine;

/// <summary>
/// Nấm Lớn (Super Mushroom) chuẩn Mario:
/// - Chui từ trong khối [?] trồi lên trên.
/// - Tự động trượt ngang, đổi hướng khi gặp chướng ngại vật/tường.
/// - Mario chạm vào -> Hóa Super Mario!
/// </summary>
public class SuperMushroom : MonoBehaviour
{
    public float moveSpeed = 2.8f;
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
            rb.linearVelocity = new Vector2(direction * moveSpeed, 0f);
        }
        isReady = true;
    }

    private void FixedUpdate()
    {
        if (!isReady || rb == null) return;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Nếu chạm tường bên hông -> quay đầu
            if (Mathf.Abs(contact.normal.x) > 0.5f)
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
            MarioPowerUpManager.instance.CollectMushroom();
        }
        else if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(1000);
        }

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 15, Color.yellow);
        }

        Destroy(gameObject);
    }
}
