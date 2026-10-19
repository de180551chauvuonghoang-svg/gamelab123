using UnityEngine;
using System.Collections;

/// <summary>
/// Cột Cờ Về Đích (Flagpole & Castle Sequence) chuẩn huyền thoại Super Mario Bros:
/// 1. Mario nhảy bám vào cột cờ.
/// 2. Mario tụt xuống dần, lá cờ trên đỉnh cột đồng thời hạ xuống đáy.
/// 3. Mario nhảy xuống đất, tự động bước đi oai vệ vào trong Lâu Đài.
/// 4. Pháo hoa nổ rực rỡ trên nóc lâu đài, nhận điểm thưởng cực lớn và hoàn thành màn chơi (STAGE CLEAR)!
/// </summary>
public class MarioFlagpole : MonoBehaviour
{
    [Header("--- THIẾT LẬP CỘT CỜ ---")]
    public Transform flagTransform;
    public Transform poleTopPoint;
    public Transform poleBottomPoint;
    public Transform castleDoorPoint;

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        if (collision.CompareTag("Player") || collision.GetComponent<PlayerController>() != null)
        {
            isTriggered = true;
            PlayerController player = collision.GetComponent<PlayerController>();
            StartCoroutine(FlagpoleSequence(player));
        }
    }

    private IEnumerator FlagpoleSequence(PlayerController player)
    {
        // Khóa input và vật lý
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Tính điểm thưởng dựa trên độ cao lúc bám cột
        float grabHeightRatio = Mathf.InverseLerp(poleBottomPoint.position.y, poleTopPoint.position.y, player.transform.position.y);
        int bonusScore = Mathf.Clamp((int)(grabHeightRatio * 5000), 500, 5000);
        if (GameManager.instance != null) GameManager.instance.AddScore(bonusScore);

        // 1. Mario bám vào cột và trượt xuống
        Vector3 startPlayerPos = new Vector3(transform.position.x - 0.2f, player.transform.position.y, 0);
        Vector3 endPlayerPos = new Vector3(transform.position.x - 0.2f, poleBottomPoint.position.y + 0.5f, 0);

        Vector3 startFlagPos = flagTransform != null ? flagTransform.position : Vector3.zero;
        Vector3 endFlagPos = poleBottomPoint != null ? poleBottomPoint.position + Vector3.up * 0.8f : Vector3.zero;

        float slideDuration = 1.2f;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            player.transform.position = Vector3.Lerp(startPlayerPos, endPlayerPos, t);
            if (flagTransform != null)
            {
                flagTransform.position = Vector3.Lerp(startFlagPos, endFlagPos, t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPlayerPos;
        if (flagTransform != null) flagTransform.position = endFlagPos;

        // 2. Mario nhảy sang bên phải cột
        yield return new WaitForSeconds(0.2f);
        player.transform.position = new Vector3(transform.position.x + 0.4f, poleBottomPoint.position.y, 0);
        yield return new WaitForSeconds(0.2f);

        // 3. Mario tự động đi bộ vào Lâu Đài
        Vector3 walkStart = player.transform.position;
        Vector3 walkEnd = castleDoorPoint != null ? castleDoorPoint.position : walkStart + Vector3.right * 6f;
        float walkDuration = 2.0f;
        elapsed = 0f;

        while (elapsed < walkDuration)
        {
            player.transform.position = Vector3.Lerp(walkStart, walkEnd, elapsed / walkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Mario biến mất vào trong lâu đài
        player.gameObject.SetActive(false);

        // 4. Bắn pháo hoa rực rỡ trên nóc lâu đài!
        Vector3 fireworksPos = walkEnd + Vector3.up * 3.5f;
        for (int i = 0; i < 3; i++)
        {
            if (ParticleSpawner.instance != null)
            {
                ParticleSpawner.instance.SpawnDust(fireworksPos + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-0.5f, 1f), 0), 30, Color.HSVToRGB(Random.value, 0.9f, 1f));
            }
            yield return new WaitForSeconds(0.4f);
        }

        // 5. Chiến thắng màn chơi (STAGE CLEAR)
        if (GameManager.instance != null)
        {
            GameManager.instance.WinGame();
        }
    }
}
