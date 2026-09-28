using UnityEngine;

public class Coin : MonoBehaviour
{
    [Tooltip("Số điểm cộng khi nhặt coin")]
    public int scoreValue = 1;

    [Tooltip("Tốc độ tự xoay nhẹ của coin để tạo hiệu ứng sinh động")]
    public float rotateSpeed = 100f;

    private void Awake()
    {
        // Đảm bảo đồng xu luôn sáng rõ, không bị đen do ánh sáng URP 2D
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
    }

    private void Update()
    {
        // Hiệu ứng xoay tròn nhẹ quanh trục Y
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        if (collision.CompareTag("Player"))
        {
            // Gọi GameManager cộng điểm
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(scoreValue);
            }

            // Xóa đồng coin khỏi Scene
            Destroy(gameObject);
        }
    }
}
