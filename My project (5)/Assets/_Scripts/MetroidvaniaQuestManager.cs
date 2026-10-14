using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Quản lý nhiệm vụ Metroidvania thế giới mở:
/// - Mở khóa kỹ năng: Double Jump, Air Dash
/// - Thu thập 3 Viên Ngọc Cổ Đại: Emerald, Ruby, Sapphire
/// - Hiển thị HUD ngọc, thông báo Zone, popup mở khóa kỹ năng
/// </summary>
public class MetroidvaniaQuestManager : MonoBehaviour
{
    public static MetroidvaniaQuestManager instance;

    // ========================================================================
    // KỸ NĂNG MỞ KHÓA (ABILITIES)
    // ========================================================================
    [Header("--- KỸ NĂNG ĐÃ MỞ KHÓA ---")]
    public bool hasDoubleJump = false;
    public bool hasAirDash = false;

    // ========================================================================
    // 3 VIÊN NGỌC CỔ ĐẠI (ANCIENT GEMS)
    // ========================================================================
    [Header("--- 3 VIÊN NGỌC CỔ ĐẠI ---")]
    public bool hasEmerald = false;   // Hang Pha Lê Ngầm
    public bool hasRuby = false;      // Thành Cổ Tàn Tích
    public bool hasSapphire = false;  // Đỉnh Tháp Mây

    public int gemCount => (hasEmerald ? 1 : 0) + (hasRuby ? 1 : 0) + (hasSapphire ? 1 : 0);

    // ========================================================================
    // GIAO DIỆN UI (HUD & NOTIFICATIONS)
    // ========================================================================
    [Header("--- UI THÔNG BÁO & HUD ---")]
    [Tooltip("Text hiển thị 3 viên ngọc trên HUD")]
    public TextMeshProUGUI gemsHudText;

    [Tooltip("Banner thông báo tên vùng đất mới")]
    public TextMeshProUGUI zoneBannerText;

    [Tooltip("Popup mở khóa kỹ năng mới")]
    public GameObject unlockPopupPanel;
    public TextMeshProUGUI unlockPopupTitle;
    public TextMeshProUGUI unlockPopupDesc;

    [Tooltip("Gợi ý bia đá / hội thoại")]
    public TextMeshProUGUI loreHintText;

    private Coroutine zoneBannerRoutine;
    private Coroutine loreHintRoutine;
    private Coroutine unlockPopupRoutine;

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
        EnsureUIReferences();
        UpdateGemsHUD();
        ShowZoneBanner("[ ĐỒNG CỎ KHỞI NGUYÊN - Whispering Plains ]");
    }

    /// <summary>
    /// Tự động tìm hoặc tạo UI Text nếu chưa gán trong Inspector
    /// </summary>
    private void EnsureUIReferences()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        // Gems HUD
        if (gemsHudText == null)
        {
            Transform t = canvas.transform.Find("GemsHUDText");
            if (t != null) gemsHudText = t.GetComponent<TextMeshProUGUI>();
            else
            {
                GameObject go = new GameObject("GemsHUDText");
                go.transform.SetParent(canvas.transform, false);
                gemsHudText = go.AddComponent<TextMeshProUGUI>();
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.anchoredPosition = new Vector2(30, -80);
                rt.sizeDelta = new Vector2(400, 45);
                gemsHudText.fontSize = 24;
                gemsHudText.fontStyle = FontStyles.Bold;
            }
        }

        // Zone Banner Text
        if (zoneBannerText == null)
        {
            Transform t = canvas.transform.Find("ZoneBannerText");
            if (t != null) zoneBannerText = t.GetComponent<TextMeshProUGUI>();
            else
            {
                GameObject go = new GameObject("ZoneBannerText");
                go.transform.SetParent(canvas.transform, false);
                zoneBannerText = go.AddComponent<TextMeshProUGUI>();
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.85f);
                rt.anchorMax = new Vector2(0.5f, 0.85f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(800, 70);
                zoneBannerText.alignment = TextAlignmentOptions.Center;
                zoneBannerText.fontSize = 32;
                zoneBannerText.fontStyle = FontStyles.Bold;
                zoneBannerText.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        // Lore Hint Text
        if (loreHintText == null)
        {
            Transform t = canvas.transform.Find("LoreHintText");
            if (t != null) loreHintText = t.GetComponent<TextMeshProUGUI>();
            else
            {
                GameObject go = new GameObject("LoreHintText");
                go.transform.SetParent(canvas.transform, false);
                loreHintText = go.AddComponent<TextMeshProUGUI>();
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.25f);
                rt.anchorMax = new Vector2(0.5f, 0.25f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(900, 80);
                loreHintText.alignment = TextAlignmentOptions.Center;
                loreHintText.fontSize = 24;
                loreHintText.fontStyle = FontStyles.Italic;
                loreHintText.color = new Color(1f, 0.95f, 0.6f, 0f);
            }
        }
    }

    // ========================================================================
    // THU THẬP NGỌC CỔ ĐẠI
    // ========================================================================
    public enum GemType { Emerald, Ruby, Sapphire }

    public void CollectGem(GemType gemType)
    {
        string gemName = "";
        string gemColor = "";

        switch (gemType)
        {
            case GemType.Emerald:
                hasEmerald = true;
                gemName = "NGỌC LỤC BẢO HANG SÂU (Emerald)";
                gemColor = "#55FF55";
                break;
            case GemType.Ruby:
                hasRuby = true;
                gemName = "NGỌC HỒNG BẢO THÀNH CỔ (Ruby)";
                gemColor = "#FF4444";
                break;
            case GemType.Sapphire:
                hasSapphire = true;
                gemName = "NGỌC LAM BẢO ĐỈNH TRỜI (Sapphire)";
                gemColor = "#44AAFF";
                break;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(500);
        }

        UpdateGemsHUD();
        ShowUnlockPopup($"ĐÃ THU THẬP: <color={gemColor}>{gemName}</color>!", $"Số ngọc đã có: {gemCount}/3 viên cổ đại.\nThu thập đủ 3 viên để giải phóng Cổng Phong Ấn!");
    }

    public void UpdateGemsHUD()
    {
        if (gemsHudText == null) EnsureUIReferences();
        if (gemsHudText == null) return;

        string em = hasEmerald ? "<color=#55FF55>[Emerald: OK]</color>" : "<color=#666666>[Emerald: --]</color>";
        string rb = hasRuby ? "<color=#FF4444>[Ruby: OK]</color>" : "<color=#666666>[Ruby: --]</color>";
        string sp = hasSapphire ? "<color=#44AAFF>[Sapphire: OK]</color>" : "<color=#666666>[Sapphire: --]</color>";

        gemsHudText.text = $"Cổ Ngọc: {em} {rb} {sp} ({gemCount}/3)";
    }

    // ========================================================================
    // MỞ KHÓA KỸ NĂNG
    // ========================================================================
    public void UnlockAbility(string abilityName, string description)
    {
        if (abilityName == "DoubleJump")
        {
            hasDoubleJump = true;
            PlayerController pc = FindFirstObjectByType<PlayerController>();
            if (pc != null) pc.canDoubleJump = true;
            ShowUnlockPopup("[KỸ NĂNG MỚI: NHẢY KÉP - DOUBLE JUMP]", "Bạn có thể bấm [SPACE] lần thứ hai giữa không trung để nhảy cao hơn!");
        }
        else if (abilityName == "AirDash")
        {
            hasAirDash = true;
            PlayerController pc = FindFirstObjectByType<PlayerController>();
            if (pc != null) pc.canAirDash = true;
            ShowUnlockPopup("[KỸ NĂNG MỚI: LƯỚT GIÓ - AIR DASH]", "Nhấn phím [LEFT SHIFT] để lướt cực nhanh về phía trước và né đòn hiểm!");
        }
    }

    // ========================================================================
    // HIỂN THỊ UI & POPUPS
    // ========================================================================
    public void ShowZoneBanner(string zoneName)
    {
        if (zoneBannerText == null) EnsureUIReferences();
        if (zoneBannerText == null) return;

        if (zoneBannerRoutine != null) StopCoroutine(zoneBannerRoutine);
        zoneBannerRoutine = StartCoroutine(AnimateZoneBanner(zoneName));
    }

    private IEnumerator AnimateZoneBanner(string text)
    {
        zoneBannerText.text = text;
        Color baseCol = Color.white;

        // Fade in
        for (float t = 0; t < 0.4f; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / 0.4f);
            zoneBannerText.color = new Color(baseCol.r, baseCol.g, baseCol.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(2.8f);

        // Fade out
        for (float t = 0; t < 0.6f; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / 0.6f);
            zoneBannerText.color = new Color(baseCol.r, baseCol.g, baseCol.b, alpha);
            yield return null;
        }
        zoneBannerText.color = new Color(baseCol.r, baseCol.g, baseCol.b, 0f);
    }

    public void ShowLoreHint(string message)
    {
        if (loreHintText == null) EnsureUIReferences();
        if (loreHintText == null) return;

        if (loreHintRoutine != null) StopCoroutine(loreHintRoutine);
        loreHintRoutine = StartCoroutine(AnimateLoreHint(message));
    }

    private IEnumerator AnimateLoreHint(string message)
    {
        loreHintText.text = $"[GỢI Ý] {message}";
        Color col = new Color(1f, 0.92f, 0.5f);

        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            loreHintText.color = new Color(col.r, col.g, col.b, t / 0.3f);
            yield return null;
        }

        yield return new WaitForSeconds(3.5f);

        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            loreHintText.color = new Color(col.r, col.g, col.b, 1f - (t / 0.5f));
            yield return null;
        }
        loreHintText.color = new Color(col.r, col.g, col.b, 0f);
    }

    public void ShowUnlockPopup(string title, string description)
    {
        if (unlockPopupRoutine != null) StopCoroutine(unlockPopupRoutine);
        unlockPopupRoutine = StartCoroutine(AnimateUnlockPopup(title, description));
    }

    private IEnumerator AnimateUnlockPopup(string title, string desc)
    {
        // Nếu không có panel riêng thì dùng ZoneBanner và LoreHint để hiển thị cực đẹp
        ShowZoneBanner(title);
        yield return new WaitForSeconds(0.3f);
        ShowLoreHint(desc);
    }

    /// <summary>
    /// Kiểm tra khi tương tác với Cổng Phong Ấn
    /// </summary>
    public bool TryOpenAncientGate()
    {
        if (gemCount >= 3)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.WinGame();
            }
            return true;
        }
        else
        {
            ShowLoreHint($"Cổng Phong Ấn Cổ Đại đang khóa! Bạn cần đủ 3 Viên Ngọc Cổ Đại (hiện có {gemCount}/3 viên).");
            return false;
        }
    }
}
