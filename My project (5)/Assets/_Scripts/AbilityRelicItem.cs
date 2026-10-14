using UnityEngine;

/// <summary>
/// Cổ vật mở khóa kỹ năng (Double Jump hoặc Air Dash)
/// </summary>
public class AbilityRelicItem : MonoBehaviour
{
    public enum AbilityType { DoubleJump, AirDash }

    [Header("--- THÔNG TIN CỔ VẬT ---")]
    public AbilityType abilityType;
    public string relicName = "Giày Nhảy Kép Cổ Đại";

    [Header("--- HIỆU ỨNG ---")]
    public float bobSpeed = 2.5f;
    public float bobHeight = 0.2f;

    private Vector3 startPos;
    private bool isCollected = false;

    private void Start()
    {
        startPos = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (abilityType == AbilityType.DoubleJump)
            {
                sr.color = new Color(1f, 0.9f, 0.3f, 1f); // Vàng sáng
            }
            else
            {
                sr.color = new Color(0.4f, 0.8f, 1f, 1f); // Lam lướt gió
            }
        }
    }

    private void Update()
    {
        if (isCollected) return;

        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            isCollected = true;

            // Hiệu ứng bùng nổ hạt
            if (ParticleSpawner.instance != null)
            {
                ParticleSpawner.instance.SpawnDust(transform.position, 30, Color.yellow);
            }

            // Mở khóa kỹ năng
            if (MetroidvaniaQuestManager.instance != null)
            {
                string id = abilityType == AbilityType.DoubleJump ? "DoubleJump" : "AirDash";
                string desc = abilityType == AbilityType.DoubleJump
                    ? "Nhấn Space lần 2 trên không để bật nhảy kép!"
                    : "Nhấn Left Shift để lướt gió siêu tốc!";
                MetroidvaniaQuestManager.instance.UnlockAbility(id, desc);
            }

            Destroy(gameObject);
        }
    }
}
