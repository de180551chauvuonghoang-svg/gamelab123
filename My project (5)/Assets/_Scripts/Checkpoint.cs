using UnityEngine;

/// <summary>
/// Checkpoint: Khi Player đi qua, vị trí này trở thành điểm hồi sinh.
/// Đặt Collider2D (Is Trigger = true) lên một vật thể bất kỳ, gắn script này.
/// Checkpoint sẽ tự đổi màu khi kích hoạt.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Đã được kích hoạt chưa?")]
    private bool isActivated = false;

    [Header("--- MÀU SẮC ---")]
    [Tooltip("Màu khi chưa kích hoạt")]
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    [Tooltip("Màu khi đã kích hoạt")]
    public Color activeColor = new Color(0.2f, 1f, 0.4f, 1f);

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Đảm bảo hiển thị đúng màu, không bị tối do URP 2D
        if (sr != null)
        {
            Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                        ?? Shader.Find("Sprites/Default");
            if (unlit != null)
            {
                sr.material = new Material(unlit);
            }
            sr.color = inactiveColor;
        }

        // Đảm bảo collider là Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated) return;

        if (collision.CompareTag("Player"))
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        isActivated = true;

        // Đổi màu thành xanh lá sáng
        if (sr != null)
        {
            sr.color = activeColor;
        }

        // Lưu vị trí checkpoint vào GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.SetCheckpoint(transform.position);
        }

        // Hiệu ứng particle tỏa sáng
        ParticleSpawner.SpawnCheckpointSparkle(transform.position);

        Debug.Log($"<color=green><b>[Checkpoint]</b> Kích hoạt tại {transform.position}</color>");
    }
}
