using UnityEngine;

/// <summary>
/// Hazard (Bẫy chông / lửa): Gây sát thương thay vì giết ngay.
/// Hỗ trợ hệ thống HP — Player mất 1 máu khi chạm bẫy.
/// </summary>
public class Hazard : MonoBehaviour
{
    [Tooltip("Bẫy có giết ngay lập tức không? (true = 1 hit kill, false = dùng hệ thống HP)")]
    public bool isInstantKill = false;

    private void Awake()
    {
        // Đảm bảo bẫy hiển thị đúng màu, không bị tối do URP 2D
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Kiểm tra bất tử
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null && player.isInvincible) return;

            if (GameManager.instance != null)
            {
                if (isInstantKill)
                {
                    // Bẫy đặc biệt: giết ngay
                    GameManager.instance.GameOver();
                }
                else
                {
                    // Bẫy thường: trừ 1 HP
                    GameManager.instance.TakeDamage();
                }
            }
        }
    }

    // Hỗ trợ cả Collision (khi collider không phải trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && player.isInvincible) return;

            if (GameManager.instance != null)
            {
                if (isInstantKill)
                {
                    GameManager.instance.GameOver();
                }
                else
                {
                    GameManager.instance.TakeDamage();
                }
            }
        }
    }
}
