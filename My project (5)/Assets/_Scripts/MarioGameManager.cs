using UnityEngine;
using TMPro;

/// <summary>
/// Quản lý giao diện HUD và đếm ngược thời gian chuẩn Super Mario Bros:
/// MARIO      WORLD      TIME
/// 000250      1-1        372
/// COINS: x04             LIVES: x3
/// </summary>
public class MarioGameManager : MonoBehaviour
{
    public static MarioGameManager instance;

    [Header("--- THÔNG SỐ MARIO ---")]
    public int coins = 0;
    public int lives = 3;
    public float timeRemaining = 400f;
    public string worldName = "1-1";

    [Header("--- UI TEXT REFERENCES ---")]
    public TextMeshProUGUI marioScoreText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI worldText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI livesText;

    private bool isTimerRunning = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EnsureUI();
        UpdateAllMarioUI();
    }

    private void Update()
    {
        if (isTimerRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeText != null)
            {
                timeText.text = $"TIME\n{(int)Mathf.Max(0, timeRemaining)}";
            }

            if (timeRemaining <= 0)
            {
                isTimerRunning = false;
                // Hết giờ -> Mất mạng
                if (GameManager.instance != null)
                {
                    GameManager.instance.GameOver();
                }
            }
        }
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        if (coins >= 100)
        {
            coins -= 100;
            lives++;
            if (ParticleSpawner.instance != null)
            {
                ParticleSpawner.instance.SpawnDust(transform.position, 20, Color.green);
            }
        }
        UpdateAllMarioUI();
    }

    public void UpdateAllMarioUI()
    {
        EnsureUI();

        int score = GameManager.instance != null ? GetGameManagerScore() : 0;
        if (marioScoreText != null) marioScoreText.text = $"MARIO\n{score:D6}";
        if (coinsText != null) coinsText.text = $"COIN\nx{coins:D2}";
        if (worldText != null) worldText.text = $"WORLD\n{worldName}";
        if (timeText != null) timeText.text = $"TIME\n{(int)Mathf.Max(0, timeRemaining)}";
        if (livesText != null) livesText.text = $"LIVES\nx{lives}";
    }

    private int GetGameManagerScore()
    {
        // GameManager có score field
        return 0; // Tự động sync qua Event hoặc GameManager
    }

    private void EnsureUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        Transform hudGroup = canvas.transform.Find("Mario_HUD");
        if (hudGroup == null)
        {
            GameObject hudGo = new GameObject("Mario_HUD");
            hudGo.transform.SetParent(canvas.transform, false);
            hudGroup = hudGo.transform;

            RectTransform rt = hudGo.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -20);
            rt.sizeDelta = new Vector2(0, 60);

            // 5 cột: MARIO, COIN, WORLD, TIME, LIVES
            marioScoreText = CreateHUDColumn(hudGroup, "ScoreText", new Vector2(-450, 0), "MARIO\n000000");
            coinsText = CreateHUDColumn(hudGroup, "CoinText", new Vector2(-220, 0), "COIN\nx00");
            worldText = CreateHUDColumn(hudGroup, "WorldText", new Vector2(0, 0), "WORLD\n1-1");
            timeText = CreateHUDColumn(hudGroup, "TimeText", new Vector2(220, 0), "TIME\n400");
            livesText = CreateHUDColumn(hudGroup, "LivesText", new Vector2(450, 0), "LIVES\nx3");
        }
    }

    private TextMeshProUGUI CreateHUDColumn(Transform parent, string name, Vector2 pos, string defaultText)
    {
        GameObject col = new GameObject(name);
        col.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = col.AddComponent<TextMeshProUGUI>();
        tmp.text = defaultText;
        tmp.fontSize = 24;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform rt = col.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(180, 50);

        return tmp;
    }
}
