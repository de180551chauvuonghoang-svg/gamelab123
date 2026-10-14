using UnityEngine;

/// <summary>
/// Đại diện cho 1 trong 3 Viên Ngọc Cổ Đại (Emerald, Ruby, Sapphire)
/// </summary>
public class AncientGemItem : MonoBehaviour
{
    public MetroidvaniaQuestManager.GemType gemType;

    [Header("--- HIỆU ỨNG THỊ GIÁC ---")]
    public float bobSpeed = 3f;
    public float bobHeight = 0.25f;
    public float rotationSpeed = 90f;

    private Vector3 startPos;
    private bool isCollected = false;

    private void Start()
    {
        startPos = transform.position;

        // Tự động gán màu hào quang theo loại ngọc
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            switch (gemType)
            {
                case MetroidvaniaQuestManager.GemType.Emerald:
                    sr.color = new Color(0.2f, 1f, 0.4f, 1f);
                    break;
                case MetroidvaniaQuestManager.GemType.Ruby:
                    sr.color = new Color(1f, 0.2f, 0.3f, 1f);
                    break;
                case MetroidvaniaQuestManager.GemType.Sapphire:
                    sr.color = new Color(0.2f, 0.6f, 1f, 1f);
                    break;
            }
        }
    }

    private void Update()
    {
        if (isCollected) return;

        // Bobbing lên xuống
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Xoay nhẹ
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            isCollected = true;

            // Nổ particle màu ngọc
            Color gemColor = Color.cyan;
            if (gemType == MetroidvaniaQuestManager.GemType.Emerald) gemColor = Color.green;
            else if (gemType == MetroidvaniaQuestManager.GemType.Ruby) gemColor = Color.red;

            if (ParticleSpawner.instance != null)
            {
                ParticleSpawner.instance.SpawnDust(transform.position, 25, gemColor);
            }

            // Ghi nhận vào Quest Manager
            if (MetroidvaniaQuestManager.instance != null)
            {
                MetroidvaniaQuestManager.instance.CollectGem(gemType);
            }

            Destroy(gameObject);
        }
    }
}
