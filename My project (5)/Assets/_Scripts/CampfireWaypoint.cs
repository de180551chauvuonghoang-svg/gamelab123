using UnityEngine;

/// <summary>
/// Trạm Lửa Trại (Campfire Waypoint):
/// - Khi Player đi ngang qua: Hồi đầy 100% máu (HP)
/// - Lưu vị trí hồi sinh Checkpoint
/// - Bùng cháy lửa rực rỡ và thông báo
/// </summary>
public class CampfireWaypoint : MonoBehaviour
{
    [Header("--- THIẾT LẬP ---")]
    public string campfireName = "Lửa Trại Khởi Nguyên";
    public Color flameColor = new Color(1f, 0.6f, 0.1f);

    private bool isLit = false;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            ActivateCampfire();
        }
    }

    public void ActivateCampfire()
    {
        // Hồi đầy máu
        if (GameManager.instance != null)
        {
            GameManager.instance.Heal(GameManager.instance.maxHP);
            GameManager.instance.SetCheckpoint(transform.position + Vector3.up * 0.5f);
        }

        // Bùng nổ hạt lửa
        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(transform.position, 20, flameColor);
        }

        if (!isLit)
        {
            isLit = true;
            if (sr != null)
            {
                sr.color = Color.yellow;
            }

            if (MetroidvaniaQuestManager.instance != null)
            {
                MetroidvaniaQuestManager.instance.ShowLoreHint($"🔥 Đã thắp sáng {campfireName}! Hồi đầy máu và lưu điểm hồi sinh.");
            }
        }
    }
}
