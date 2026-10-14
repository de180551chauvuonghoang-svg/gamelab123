using UnityEngine;

/// <summary>
/// Trigger kích hoạt thông báo khi bước vào vùng đất mới
/// </summary>
public class ZoneTrigger : MonoBehaviour
{
    public string zoneName = "🌲 ĐỒNG CỎ KHỞI NGUYÊN (Whispering Plains)";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            if (MetroidvaniaQuestManager.instance != null)
            {
                MetroidvaniaQuestManager.instance.ShowZoneBanner(zoneName);
            }
        }
    }
}
