using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// GameManager nâng cấp chuyên nghiệp:
/// - Hệ thống HP (3 máu) thay vì chết 1 hit
/// - Iframe (bất tử tạm thời) khi mất máu
/// - Checkpoint respawn
/// - Combo Score khi dẫm quái liên tiếp
/// - High Score lưu PlayerPrefs
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // ========================================================================
    // ĐIỂM SỐ
    // ========================================================================
    [Header("--- ĐIỂM SỐ & GIAO DIỆN UI ---")]
    [Tooltip("Text hiển thị điểm số (TextMeshPro)")]
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("--- HIGH SCORE ---")]
    [Tooltip("Text hiển thị High Score (tùy chọn)")]
    public TextMeshProUGUI highScoreText;
    private int highScore = 0;

    // ========================================================================
    // MÁU (HP)
    // ========================================================================
    [Header("--- HỆ THỐNG MÁU ---")]
    [Tooltip("Số máu tối đa")]
    public int maxHP = 3;

    [Tooltip("Text hiển thị HP (tùy chọn — nếu không có sẽ hiển thị cùng Score)")]
    public TextMeshProUGUI hpText;

    [Tooltip("Thời gian bất tử sau khi mất máu (giây)")]
    public float invincibilityDuration = 1.5f;

    private int currentHP;

    // ========================================================================
    // COMBO
    // ========================================================================
    [Header("--- COMBO SYSTEM ---")]
    [Tooltip("Text hiển thị combo (tùy chọn)")]
    public TextMeshProUGUI comboText;

    private int comboCount = 0;
    private Coroutine comboDisplayCoroutine;

    // ========================================================================
    // BẢNG THÔNG BÁO
    // ========================================================================
    [Header("--- BẢNG THÔNG BÁO ---")]
    [Tooltip("Panel hiển thị khi Game Over")]
    public GameObject gameOverPanel;

    [Tooltip("Panel hiển thị khi chiến thắng (You Win)")]
    public GameObject winPanel;

    // ========================================================================
    // CHECKPOINT
    // ========================================================================
    private Vector3 lastCheckpointPos;
    private bool hasCheckpoint = false;

    private bool isGameEnded = false;

    // ========================================================================
    // SINGLETON
    // ========================================================================
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        isGameEnded = false;
        currentHP = maxHP;
        comboCount = 0;

        // Load High Score
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Cập nhật UI ban đầu
        UpdateAllUI();

        // Ẩn các bảng thông báo
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    // ========================================================================
    // ĐIỂM SỐ
    // ========================================================================
    public void AddScore(int amount)
    {
        if (isGameEnded) return;

        score += amount;
        UpdateScoreUI();

        // Cập nhật High Score real-time
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScoreUI();
        }
    }

    /// <summary>
    /// Cộng điểm khi dẫm quái — có nhân combo.
    /// comboCount tăng mỗi lần gọi, reset khi chạm đất (ResetCombo).
    /// </summary>
    public void AddStompScore(int baseReward, Vector3 stompPosition)
    {
        if (isGameEnded) return;

        comboCount++;
        int multipliedScore = baseReward * comboCount;
        AddScore(multipliedScore);

        // Hiệu ứng particle combo
        if (comboCount >= 2)
        {
            ParticleSpawner.SpawnComboStars(stompPosition, comboCount);
        }

        // Hiển thị combo text
        ShowComboText();

        Debug.Log($"<color=yellow><b>[COMBO x{comboCount}]</b> +{multipliedScore} điểm!</color>");
    }

    /// <summary>
    /// Reset combo khi Player chạm đất (gọi từ PlayerController).
    /// </summary>
    public void ResetCombo()
    {
        if (comboCount > 0)
        {
            comboCount = 0;
            HideComboText();
        }
    }

    // ========================================================================
    // MÁU (HP) & SÁT THƯƠNG
    // ========================================================================
    /// <summary>
    /// Gọi khi Player bị đánh (thay thế cho GameOver trực tiếp).
    /// Trừ 1 HP, nếu còn máu → iframe + respawn. Nếu hết máu → Game Over.
    /// </summary>
    public void TakeDamage()
    {
        if (isGameEnded) return;

        // Tìm Player
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null && player.isInvincible) return; // Đang bất tử → bỏ qua

        currentHP--;
        UpdateHPUI();

        // Camera Shake khi bị đánh
        CameraFollow cam = FindFirstObjectByType<CameraFollow>();
        if (cam != null)
        {
            cam.Shake(0.2f, 0.25f);
        }

        if (currentHP <= 0)
        {
            // HẾT MÁU → GAME OVER
            GameOver();
        }
        else
        {
            // CÒN MÁU → iframe + respawn tại checkpoint
            Debug.Log($"<color=orange><b>[Mất máu]</b> HP còn: {currentHP}/{maxHP}</color>");

            if (player != null)
            {
                player.StartInvincibility(invincibilityDuration);

                // Respawn tại checkpoint nếu có
                if (hasCheckpoint)
                {
                    player.TeleportTo(lastCheckpointPos + Vector3.up * 0.5f);
                }
            }

            // Phát âm thanh bị đánh (dùng game over sound nhẹ hơn)
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.gameOverClip, 0.4f);
            }
        }
    }

    // ========================================================================
    // HỒI MÁU (HEAL)
    // ========================================================================
    /// <summary>
    /// Hồi máu cho Player (ví dụ khi chạm Campfire).
    /// </summary>
    public void Heal(int amount = 1)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        UpdateHPUI();
        Debug.Log($"<color=green><b>[Hồi máu]</b> HP: {currentHP}/{maxHP}</color>");
    }

    // ========================================================================
    // CHECKPOINT
    // ========================================================================
    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPos = position;
        hasCheckpoint = true;
        Debug.Log($"<color=green><b>[Checkpoint]</b> Lưu vị trí: {position}</color>");
    }

    // ========================================================================
    // GAME OVER & WIN
    // ========================================================================
    public void GameOver()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("<color=red><b>[Game Over]</b> Người chơi đã thua!</color>");

        // Lưu High Score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayGameOverSFX();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void WinGame()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("<color=yellow><b>[You Win]</b> Chúc mừng! Bạn đã chiến thắng!</color>");

        // Lưu High Score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayWinSFX();
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // ========================================================================
    // NÚT BẤM UI
    // ========================================================================
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (Application.CanStreamedLevelBeLoaded("MainMenu"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // ========================================================================
    // CẬP NHẬT GIAO DIỆN
    // ========================================================================
    private void UpdateAllUI()
    {
        UpdateScoreUI();
        UpdateHPUI();
        UpdateHighScoreUI();
        HideComboText();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            // Nếu không có hpText riêng, hiển thị HP cùng Score
            if (hpText == null)
            {
                string hearts = "";
                for (int i = 0; i < currentHP; i++) hearts += "❤";
                for (int i = currentHP; i < maxHP; i++) hearts += "🖤";
                scoreText.text = $"{hearts}  Score: {score}";
            }
            else
            {
                scoreText.text = $"Score: {score}";
            }
        }
    }

    private void UpdateHPUI()
    {
        if (hpText != null)
        {
            string hearts = "";
            for (int i = 0; i < currentHP; i++) hearts += "❤";
            for (int i = currentHP; i < maxHP; i++) hearts += "🖤";
            hpText.text = hearts;
        }
        else
        {
            // Cập nhật lại scoreText vì nó chứa cả HP
            UpdateScoreUI();
        }
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"Best: {highScore}";
        }
    }

    private void ShowComboText()
    {
        if (comboText != null && comboCount >= 2)
        {
            comboText.text = $"COMBO x{comboCount}!";
            comboText.gameObject.SetActive(true);

            // Tự động ẩn sau 1.5 giây
            if (comboDisplayCoroutine != null)
            {
                StopCoroutine(comboDisplayCoroutine);
            }
            comboDisplayCoroutine = StartCoroutine(HideComboAfterDelay(1.5f));
        }
    }

    private void HideComboText()
    {
        if (comboText != null)
        {
            comboText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideComboAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideComboText();
    }
}
