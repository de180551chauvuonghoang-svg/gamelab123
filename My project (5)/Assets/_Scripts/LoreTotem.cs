using UnityEngine;

/// <summary>
/// Bia Đá Cổ (Lore Totem): Hiện thông điệp cổ dẫn dắt câu chuyện và hướng dẫn đường đi khi Player lại gần.
/// </summary>
public class LoreTotem : MonoBehaviour
{
    [TextArea(2, 4)]
    public string loreMessage = "Nơi đây lưu giữ lời tiên tri: Gom đủ 3 viên Cổ Ngọc để mở Cổng Thần Bí.";

    private float cooldown = 4f;
    private float lastTriggerTime = -10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            if (Time.time - lastTriggerTime > cooldown)
            {
                lastTriggerTime = Time.time;
                if (MetroidvaniaQuestManager.instance != null)
                {
                    MetroidvaniaQuestManager.instance.ShowLoreHint(loreMessage);
                }

                if (ParticleSpawner.instance != null)
                {
                    ParticleSpawner.instance.SpawnDust(transform.position + Vector3.up * 0.5f, 10, new Color(1f, 0.9f, 0.5f));
                }
            }
        }
    }
}
