using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Quản lý giao diện HUD, tính điểm (Score), nhặt xu (Coins), mạng (Lives) và đếm ngược thời gian chuẩn Super Mario Bros:
/// MARIO       COIN       WORLD       TIME       LIVES
/// 000250      x04         1-1         372        x3
/// </summary>
public class MarioGameManager : MonoBehaviour
{
    public static MarioGameManager instance;

    [Header("--- THÔNG SỐ MARIO ---")]
    public int score = 0;
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
        // Ẩn các UI của các chế độ khác (Metroidvania/Lab cũ) nếu còn sót
        CleanupLegacyUI();

        EnsureUI();

        // Đồng bộ điểm ban đầu từ GameManager nếu có
        if (GameManager.instance != null)
        {
            score = GameManager.instance.CurrentScore;
        }

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
                if (GameManager.instance != null)
                {
                    GameManager.instance.GameOver();
                }
            }
        }
    }

    /// <summary>
    /// Cộng điểm khi giết quái, đập khối gạch hoặc nhặt xu
    /// </summary>
    public void AddScore(int amount, Vector3? worldPos = null)
    {
        score += amount;

        // Đồng bộ ngược sang GameManager
        if (GameManager.instance != null && GameManager.instance.CurrentScore != score)
        {
            GameManager.instance.AddScore(amount);
        }

        // Hiển thị chữ số điểm bay lên tại vị trí quái chết hoặc ăn coin
        if (worldPos.HasValue)
        {
            ShowFloatingScore(worldPos.Value, amount);
        }

        UpdateAllMarioUI();
    }

    /// <summary>
    /// Đồng bộ điểm số từ GameManager sang Mario HUD
    /// </summary>
    public void SyncScore(int newScore)
    {
        score = newScore;
        UpdateAllMarioUI();
    }

    /// <summary>
    /// Nhặt coin: Tăng số coin, cộng 200 điểm chuẩn Mario, đủ 100 coin tặng 1 mạng
    /// </summary>
    public void AddCoin(int amount = 1, Vector3? worldPos = null)
    {
        coins += amount;
        
        // Mỗi coin tặng 200 điểm
        AddScore(amount * 200, worldPos);

        // Đủ 100 coin tặng thêm 1 mạng (1-Up)
        if (coins >= 100)
        {
            coins -= 100;
            lives++;
            if (ParticleSpawner.instance != null)
            {
                ParticleSpawner.instance.SpawnDust(transform.position, 25, Color.green);
            }
        }

        UpdateAllMarioUI();
    }

    public void UpdateAllMarioUI()
    {
        EnsureUI();

        if (marioScoreText != null) marioScoreText.text = $"MARIO\n{score:D6}";
        if (coinsText != null) coinsText.text = $"COIN\nx{coins:D2}";
        if (worldText != null) worldText.text = $"WORLD\n{worldName}";
        if (timeText != null) timeText.text = $"TIME\n{(int)Mathf.Max(0, timeRemaining)}";
        if (livesText != null) livesText.text = $"LIVES\nx{lives}";
    }

    private void CleanupLegacyUI()
    {
        // Tắt QuestManager Metroidvania nếu đang chạy
        GameObject questMgr = GameObject.Find("QuestManager");
        if (questMgr != null) questMgr.SetActive(false);

        // Tắt các text thừa của Metroidvania
        GameObject gemsText = GameObject.Find("GemsHUDText");
        if (gemsText != null) gemsText.SetActive(false);

        GameObject zoneBanner = GameObject.Find("ZoneBannerText");
        if (zoneBanner != null) zoneBanner.SetActive(false);

        GameObject loreHint = GameObject.Find("LoreHintText");
        if (loreHint != null) loreHint.SetActive(false);

        // Tắt ScoreText cũ của Lab 2
        GameObject oldScore = GameObject.Find("ScoreText");
        if (oldScore != null) oldScore.SetActive(false);
    }

    private void EnsureUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGo = new GameObject("MarioCanvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        // Đảm bảo CanvasScaler hỗ trợ co giãn chuẩn theo màn hình (1920x1080)
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        Transform hudGroup = canvas.transform.Find("Mario_HUD");
        if (hudGroup == null)
        {
            GameObject hudGo = new GameObject("Mario_HUD", typeof(RectTransform));
            hudGo.transform.SetParent(canvas.transform, false);
            hudGroup = hudGo.transform;

            RectTransform rt = (RectTransform)hudGo.transform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -15f);
            rt.sizeDelta = new Vector2(0f, 70f);
        }

        if (hudGroup != null)
        {
            if (marioScoreText == null) marioScoreText = CreateHUDColumn(hudGroup, "ScoreText", new Vector2(-600, 0), $"MARIO\n{score:D6}");
            if (coinsText == null) coinsText = CreateHUDColumn(hudGroup, "CoinText", new Vector2(-300, 0), $"COIN\nx{coins:D2}");
            if (worldText == null) worldText = CreateHUDColumn(hudGroup, "WorldText", new Vector2(0, 0), $"WORLD\n{worldName}");
            if (timeText == null) timeText = CreateHUDColumn(hudGroup, "TimeText", new Vector2(300, 0), $"TIME\n{(int)timeRemaining}");
            if (livesText == null) livesText = CreateHUDColumn(hudGroup, "LivesText", new Vector2(600, 0), $"LIVES\nx{lives}");
        }
    }

    private TextMeshProUGUI CreateHUDColumn(Transform parent, string name, Vector2 pos, string defaultText)
    {
        if (parent == null) return null;

        Transform existing = parent.Find(name);
        if (existing != null)
        {
            return existing.GetComponent<TextMeshProUGUI>();
        }

        GameObject col = new GameObject(name, typeof(RectTransform));
        col.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = col.AddComponent<TextMeshProUGUI>();
        tmp.text = defaultText;
        tmp.fontSize = 28;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.95f, 0.4f, 1f); // Vàng Mario sáng đẹp

        RectTransform rt = (RectTransform)col.transform;
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(250, 60);

        return tmp;
    }

    // ========================================================================
    // HIỆU ỨNG POPUP ĐIỂM SỐ BAY LÊN (+100, +200, +400, +800)
    // ========================================================================
    /// <summary>
    /// Tạo chữ điểm bay lên tại vị trí thế giới (world space) rồi tan biến
    /// </summary>
    public static void ShowFloatingScore(Vector3 worldPos, int points)
    {
        if (points <= 0) return;

        GameObject popup = new GameObject("ScorePopup_" + points, typeof(RectTransform));
        popup.transform.position = worldPos + Vector3.up * 0.3f;

        TextMeshPro tm = popup.AddComponent<TextMeshPro>();
        tm.text = $"+{points}";
        tm.fontSize = 6;
        tm.fontStyle = FontStyles.Bold;
        tm.alignment = TextAlignmentOptions.Center;
        tm.color = new Color(1f, 0.9f, 0.2f, 1f); // Màu vàng rực rỡ
        tm.sortingOrder = 20;

        if (instance != null)
        {
            instance.StartCoroutine(AnimateScorePopup(popup, tm));
        }
        else
        {
            Destroy(popup, 0.6f);
        }
    }

    private static IEnumerator AnimateScorePopup(GameObject popup, TextMeshPro tm)
    {
        float duration = 0.6f;
        float elapsed = 0f;
        if (popup == null || tm == null) yield break;

        Vector3 startPos = popup.transform.position;
        Vector3 targetPos = startPos + Vector3.up * 1.3f;
        Color initialColor = tm.color;

        while (elapsed < duration)
        {
            if (popup == null || tm == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Di chuyển lên cao dần
            popup.transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));

            // Mờ dần về cuối
            float alpha = Mathf.Lerp(1f, 0f, t * t);
            tm.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            yield return null;
        }

        if (popup != null)
        {
            Destroy(popup);
        }
    }
}
