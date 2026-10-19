using UnityEngine;
using System.Collections;

/// <summary>
/// Ống Nước (Warp Pipe) chuẩn Mario:
/// - Vật cản platform kinh điển.
/// - Ống có thể chui được (Enterable): Đứng trên miệng ống, nhấn [S] hoặc [Mũi Tên Xuống] -> Mario thụt dần vào trong lòng ống nước, dịch chuyển xuống Hầm Bí Mật (Underground Bonus Room)!
/// - Ống trồi lên (Exit Pipe): Tự động đẩy Mario trồi lên khỏi mặt đất khi ra khỏi hầm.
/// </summary>
public class WarpPipe : MonoBehaviour
{
    [Header("--- THIẾT LẬP ỐNG NƯỚC ---")]
    [Tooltip("Ống nước có thể chui xuống được không")]
    public bool isEnterable = false;

    [Tooltip("Điểm đích đến khi chui qua ống (VD: Cửa hầm ngầm hoặc mặt đất)")]
    public Transform destinationPoint;

    [Tooltip("Hướng chui vào: Down (thụt xuống) hoặc Right (chui ngang)")]
    public Vector2 enterDirection = Vector2.down;

    private bool isPlayerOnTop = false;
    private PlayerController currentPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            isPlayerOnTop = true;
            currentPlayer = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            isPlayerOnTop = false;
            currentPlayer = null;
        }
    }

    private void Update()
    {
        if (!isEnterable || !isPlayerOnTop || currentPlayer == null || destinationPoint == null) return;

        // Đọc phím bấm xuống (S hoặc Mũi tên xuống)
        bool downPressed = false;
#if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null && (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame))
        {
            downPressed = true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            downPressed = true;
        }
#endif

        if (downPressed)
        {
            StartCoroutine(PipeDescentRoutine(currentPlayer));
        }
    }

    private IEnumerator PipeDescentRoutine(PlayerController player)
    {
        isPlayerOnTop = false;

        // Khóa input và vô hiệu hóa vật lý
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Collider2D col = player.GetComponent<Collider2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Vector3 startPos = player.transform.position;
        Vector3 endPos = startPos + (Vector3)enterDirection * 1.5f;

        // Trượt dần vào ống
        float elapsed = 0f;
        float duration = 0.8f;
        while (elapsed < duration)
        {
            player.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Bụi ống nước
        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(destinationPoint.position, 12, new Color(0.2f, 0.8f, 0.3f));
        }

        // Dịch chuyển đến điểm đích
        player.transform.position = destinationPoint.position;

        // Trồi lên ở đầu ra
        startPos = destinationPoint.position;
        endPos = startPos + Vector3.up * 1.2f;
        elapsed = 0f;
        duration = 0.6f;
        while (elapsed < duration)
        {
            player.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPos;

        // Bật lại vật lý
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
