using UnityEngine;

/// <summary>
/// Coin nâng cấp: Thêm hiệu ứng nổ vàng particle khi nhặt,
/// bobbing animation (lên xuống nhẹ), và vòng xoay mượt hơn.
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("--- THIẾT LẬP ---")]
    [Tooltip("Số điểm cộng khi nhặt coin")]
    public int scoreValue = 1;

    [Tooltip("Tốc độ tự xoay nhẹ")]
    public float rotateSpeed = 100f;

    [Header("--- BOBBING (LƠNG LỬNG) ---")]
    [Tooltip("Biên độ lên xuống")]
    public float bobAmplitude = 0.12f;

    [Tooltip("Tốc độ lên xuống")]
    public float bobFrequency = 2f;

    private Vector3 startPosition;
    private float bobOffset;

    private void Awake()
    {
        // Đảm bảo sáng rõ trong URP 2D
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                        ?? Shader.Find("Sprites/Default");
            if (unlit != null)
            {
                sr.material = new Material(unlit);
            }
        }

        startPosition = transform.position;

        // Offset ngẫu nhiên để các coin không đồng bộ lên xuống
        bobOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        // Xoay tròn nhẹ quanh trục Y
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

        // Bobbing: lên xuống nhẹ nhàng
        float newY = startPosition.y + Mathf.Sin((Time.time * bobFrequency) + bobOffset) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Hiệu ứng particle nổ vàng tại vị trí coin
            ParticleSpawner.SpawnCoinBurst(transform.position);

            // Phát âm thanh
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayCoinSFX();
            }

            // Cộng điểm
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(scoreValue);
            }

            // Xóa coin
            Destroy(gameObject);
        }
    }
}
