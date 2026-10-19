using UnityEngine;

/// <summary>
/// Hoa Lửa (Fire Flower) chuẩn Mario:
/// - Chui từ trong khối [?] lên.
/// - Mario chạm vào -> Hóa Fire Mario bắn cầu lửa.
/// </summary>
public class FireFlowerItem : MonoBehaviour
{
    private bool isReady = false;

    public void EmergeFromBlock(Vector3 blockPos)
    {
        transform.position = blockPos;
        StartCoroutine(EmergeRoutine(blockPos));
    }

    private System.Collections.IEnumerator EmergeRoutine(Vector3 startPos)
    {
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos + Vector3.up * 1f;
        isReady = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isReady) return;

        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            if (MarioPowerUpManager.instance != null)
            {
                MarioPowerUpManager.instance.CollectFireFlower();
            }
            else if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(1000);
            }

            if (ParticleSpawner.instance != null)
            {
                ParticleSpawner.instance.SpawnDust(transform.position, 15, Color.red);
            }

            Destroy(gameObject);
        }
    }
}
