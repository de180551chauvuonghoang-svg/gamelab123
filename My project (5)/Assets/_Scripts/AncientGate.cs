using UnityEngine;

/// <summary>
/// Cổng Phong Ấn Cổ Đại (The Ancient Gate):
/// Yêu cầu đủ 3 Viên Ngọc Cổ Đại (Emerald, Ruby, Sapphire) để mở và hoàn thành game.
/// </summary>
public class AncientGate : MonoBehaviour
{
    private bool isOpened = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpened) return;

        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            if (MetroidvaniaQuestManager.instance != null)
            {
                if (MetroidvaniaQuestManager.instance.TryOpenAncientGate())
                {
                    isOpened = true;
                    if (ParticleSpawner.instance != null)
                    {
                        ParticleSpawner.instance.SpawnDust(transform.position, 50, Color.cyan);
                    }
                }
            }
        }
    }
}
