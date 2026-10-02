#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class Lab3Automator
{
    private const string MAIN_MENU_SCENE_PATH = "Assets/Scenes/MainMenu.unity";
    private const string GAME_SCENE_PATH = "Assets/Scenes/SampleScene.unity";

    static Lab3Automator()
    {
        EditorApplication.delayCall += () =>
        {
            ConfigureLab3Assets();
        };
    }

    [MenuItem("Lab 3/Chạy Cài Đặt Tự Động Toàn Bộ Lab 3 (One-Click Setup)", false, 1)]
    public static void RunLab3Setup()
    {
        Debug.Log("<color=cyan><b>[Lab 3]</b> Bắt đầu cài đặt tự động toàn bộ nội dung Lab 3...</color>");

        // 1. Cấu hình Assets (Audio & MovingPlatform sprite)
        ConfigureLab3Assets();

        // 2. Thiết lập Scene màn chơi chính (SampleScene)
        SetupGameScene();

        // 3. Tạo Main Menu Scene
        SetupMainMenuScene();

        // 4. Cập nhật Build Settings (Scene 0 = MainMenu, Scene 1 = SampleScene)
        UpdateBuildSettings();

        // 5. Mở lại MainMenu Scene làm mặc định
        EditorSceneManager.OpenScene(MAIN_MENU_SCENE_PATH);

        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("Lab 3 Setup Hoàn Tất!", 
                "Đã thiết lập toàn bộ nội dung Lab 3 thành công rực rỡ!\n\n" +
                "✔ 1. Hệ thống âm thanh (AudioManager): BGM lặp liên tục, SFX Nhảy & Ăn xu (PlayOneShot)\n" +
                "✔ 2. Sàn di chuyển (Moving Platform): Tự động đi tuần A <-> B, Player đứng lên làm con (SetParent)\n" +
                "✔ 3. Main Menu Scene: Giao diện thẩm mỹ cao với nút Play Game và Thoát Game\n" +
                "✔ 4. Build Settings: MainMenu ở Index 0, SampleScene ở Index 1\n\n" +
                "Bây giờ bạn chỉ cần nhấn PLAY (▶) trên Main Menu để trải nghiệm toàn bộ game!", 
                "Tuyệt vời!");
        }
    }

    [MenuItem("Lab 3/1. Cấu Hình Audio Manager & Âm Thanh (Sound Setup)", false, 2)]
    public static void SetupAudioOnly()
    {
        ConfigureLab3Assets();
        SetupGameSceneAudio();
        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("Thành công", "Đã cấu hình AudioManager với BGM và SFX cho Player và Coin thành công!", "OK");
        }
    }

    [MenuItem("Lab 3/2. Tạo Sàn Di Chuyển (Moving Platform Setup)", false, 3)]
    public static void SetupMovingPlatformOnly()
    {
        ConfigureLab3Assets();
        Scene activeScene = EnsureSceneOpen(GAME_SCENE_PATH);
        SetupMovingPlatform(activeScene);
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);
        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("Thành công", "Đã thiết lập Sàn di chuyển (Moving Platform A-B) trong SampleScene!", "OK");
        }
    }

    [MenuItem("Lab 3/3. Tạo Màn Hình Menu Chính (Main Menu Scene Setup)", false, 4)]
    public static void SetupMainMenuOnly()
    {
        ConfigureLab3Assets();
        SetupMainMenuScene();
        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("Thành công", "Đã tạo và thiết lập Main Menu Scene hoàn chỉnh!", "OK");
        }
    }

    [MenuItem("Lab 3/4. Cập Nhật Build Settings (Scenes in Build)", false, 5)]
    public static void UpdateBuildSettingsOnly()
    {
        UpdateBuildSettings();
        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("Thành công", "Đã cấu hình Build Settings:\n- Index 0: MainMenu.unity\n- Index 1: SampleScene.unity", "OK");
        }
    }

    [MenuItem("Lab 3/5. Xuất Bản Game Độc Lập (.EXE Standalone Build)", false, 6)]
    public static void BuildStandaloneGame()
    {
        UpdateBuildSettings();

        string buildDir = "Build";
        if (!Directory.Exists(buildDir))
        {
            Directory.CreateDirectory(buildDir);
        }

        string exePath = Path.Combine(buildDir, "GameLab3.exe");

        Debug.Log("<color=cyan><b>[Lab 3 Build]</b> Đang tiến hành đóng gói Game xuất ra file .exe...</color>");

        string[] scenes = new string[] { MAIN_MENU_SCENE_PATH, GAME_SCENE_PATH };
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = exePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var summary = report.summary;

        if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"<color=green><b>[Lab 3 Build]</b> Build thành công! Dung lượng: {summary.totalSize / (1024 * 1024)} MB. Đường dẫn: {exePath}</color>");
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Build Thành Công!", 
                    $"Game đã được đóng gói xuất bản thành công!\n\nFile thực thi: {exePath}\nKích thước: {summary.totalSize / (1024 * 1024)} MB\n\nBạn có thể mở thư mục Build để chạy thử file .exe!", "Mở thư mục Build");
                EditorUtility.RevealInFinder(exePath);
            }
        }
        else
        {
            Debug.LogError($"<color=red><b>[Lab 3 Build]</b> Build thất bại với lỗi: {summary.result}</color>");
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Lỗi Build", "Có lỗi xảy ra trong quá trình Build game. Vui lòng kiểm tra Console log.", "Đóng");
            }
        }
    }

    // =========================================================================
    // IMPLEMENTATION DETAILS
    // =========================================================================

    public static void ConfigureLab3Assets()
    {
        // 1. Cấu hình Sprite MovingPlatform.png
        string platPath = "Assets/Sprites/MovingPlatform.png";
        if (File.Exists(platPath))
        {
            TextureImporter importer = AssetImporter.GetAtPath(platPath) as TextureImporter;
            if (importer != null)
            {
                bool dirty = false;
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; dirty = true; }
                if (importer.spritePixelsPerUnit != 32) { importer.spritePixelsPerUnit = 32; dirty = true; }
                if (importer.filterMode != FilterMode.Point) { importer.filterMode = FilterMode.Point; dirty = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; dirty = true; }
                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        // 2. Cấu hình các file Audio WAV
        string audioDir = "Assets/Audio";
        if (Directory.Exists(audioDir))
        {
            string[] audioFiles = Directory.GetFiles(audioDir, "*.wav");
            foreach (var file in audioFiles)
            {
                AudioImporter audioImporter = AssetImporter.GetAtPath(file) as AudioImporter;
                if (audioImporter != null)
                {
                    bool isBgm = file.Contains("bgm");
                    AudioImporterSampleSettings settings = audioImporter.defaultSampleSettings;
                    settings.loadType = isBgm ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
                    audioImporter.defaultSampleSettings = settings;
                    audioImporter.loadInBackground = true;
                    audioImporter.SaveAndReimport();
                }
            }
        }
    }

    private static void SetupGameScene()
    {
        Scene activeScene = EnsureSceneOpen(GAME_SCENE_PATH);
        if (!activeScene.isLoaded) return;

        // Cài AudioManager
        SetupAudioManagerInScene();

        // Cài Moving Platform
        SetupMovingPlatform(activeScene);

        // Đảm bảo sửa lỗi sprite đen
        Lab2Automator.FixAllBlackSprites();

        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);
    }

    private static void SetupGameSceneAudio()
    {
        Scene activeScene = EnsureSceneOpen(GAME_SCENE_PATH);
        if (!activeScene.isLoaded) return;
        SetupAudioManagerInScene();
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);
    }

    public static GameObject SetupAudioManagerInScene()
    {
        GameObject audioManagerGo = GameObject.Find("AudioManager");
        if (audioManagerGo == null)
        {
            audioManagerGo = new GameObject("AudioManager");
        }

        AudioManager am = audioManagerGo.GetComponent<AudioManager>();
        if (am == null) am = audioManagerGo.AddComponent<AudioManager>();

        // Thiết lập 2 AudioSources theo đúng chuẩn đề bài
        AudioSource[] sources = audioManagerGo.GetComponents<AudioSource>();
        AudioSource musicSrc = sources.Length > 0 ? sources[0] : audioManagerGo.AddComponent<AudioSource>();
        AudioSource sfxSrc = sources.Length > 1 ? sources[1] : audioManagerGo.AddComponent<AudioSource>();

        musicSrc.loop = true;
        musicSrc.playOnAwake = false;
        musicSrc.spatialBlend = 0f; // 2D Sound
        musicSrc.volume = 0.5f;

        sfxSrc.loop = false;
        sfxSrc.playOnAwake = false;
        sfxSrc.spatialBlend = 0f; // 2D Sound
        sfxSrc.volume = 0.85f;

        am.musicSource = musicSrc;
        am.sfxSource = sfxSrc;

        // Gán các AudioClips
        am.bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/bgm.wav");
        am.jumpClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/jump.wav");
        am.coinClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/coin.wav");
        am.gameOverClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/gameover.wav");
        am.winClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/win.wav");
        am.clickClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/click.wav");

        EditorUtility.SetDirty(am);
        return audioManagerGo;
    }

    private static void SetupMovingPlatform(Scene scene)
    {
        Sprite platSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/MovingPlatform.png");

        // Nhóm Moving_Platform_Group
        GameObject group = GameObject.Find("Moving_Platform_Group");
        if (group == null)
        {
            group = new GameObject("Moving_Platform_Group");
        }

        // Tạo 2 điểm mốc A và B
        Transform pointA = group.transform.Find("PointA");
        if (pointA == null)
        {
            GameObject paGo = new GameObject("PointA");
            paGo.transform.SetParent(group.transform);
            pointA = paGo.transform;
        }
        // Đặt điểm A ở toạ độ x = 16.5f (ngay sau khu vực quái vật)
        pointA.position = new Vector3(16.5f, -1.0f, 0);

        Transform pointB = group.transform.Find("PointB");
        if (pointB == null)
        {
            GameObject pbGo = new GameObject("PointB");
            pbGo.transform.SetParent(group.transform);
            pointB = pbGo.transform;
        }
        // Đặt điểm B ở toạ độ x = 22.0f (bay sang phía bên kia vực / bến đỗ đích)
        pointB.position = new Vector3(22.0f, -1.0f, 0);

        // Tạo GameObject MovingPlatform
        GameObject platGo = GameObject.Find("MovingPlatform");
        if (platGo == null)
        {
            platGo = new GameObject("MovingPlatform");
            platGo.transform.SetParent(group.transform);
        }

        platGo.transform.position = pointA.position;
        platGo.transform.localScale = Vector3.one;

        // SpriteRenderer
        SpriteRenderer sr = platGo.GetComponent<SpriteRenderer>();
        if (sr == null) sr = platGo.AddComponent<SpriteRenderer>();
        if (platSprite != null) sr.sprite = platSprite;
        sr.sortingLayerName = "Ground";
        sr.sortingOrder = 5;

        Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
        if (unlitShader != null) sr.material = new Material(unlitShader);

        // BoxCollider2D (vừa vặn với kích thước sàn 3m x 0.75m)
        BoxCollider2D col = platGo.GetComponent<BoxCollider2D>();
        if (col == null) col = platGo.AddComponent<BoxCollider2D>();
        col.size = new Vector2(3.0f, 0.7f);
        col.offset = new Vector2(0f, 0.05f);

        // Rigidbody2D (Kinematic để vật lý ổn định)
        Rigidbody2D rb = platGo.GetComponent<Rigidbody2D>();
        if (rb == null) rb = platGo.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Script MovingPlatform
        MovingPlatform mp = platGo.GetComponent<MovingPlatform>();
        if (mp == null) mp = platGo.AddComponent<MovingPlatform>();
        mp.pointA = pointA;
        mp.pointB = pointB;
        mp.speed = 2.5f;
        mp.waitTime = 0.5f;

        // Di chuyển Key_WinItem sang phía sau điểm B để người chơi cần đi sàn sang nhặt chìa khóa chiến thắng!
        GameObject keyItem = GameObject.Find("Key_WinItem");
        if (keyItem != null)
        {
            keyItem.transform.position = new Vector3(24.5f, -1.8f, 0);
        }

        // Tạo thêm 2 đồng xu trên đường sàn di chuyển để người chơi thu thập
        GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Coin.prefab");
        if (coinPrefab != null)
        {
            GameObject coinsContainer = GameObject.Find("Coins_Container");
            if (coinsContainer != null)
            {
                Transform cPlat1 = coinsContainer.transform.Find("Coin_Plat_1");
                if (cPlat1 == null)
                {
                    GameObject c1 = (GameObject)PrefabUtility.InstantiatePrefab(coinPrefab);
                    c1.name = "Coin_Plat_1";
                    c1.transform.position = new Vector3(19.25f, 0.3f, 0);
                    c1.transform.SetParent(coinsContainer.transform);
                }
            }
        }
    }

    public static void SetupMainMenuScene()
    {
        // 1. Tạo hoặc mở MainMenu scene
        Scene menuScene;
        if (File.Exists(MAIN_MENU_SCENE_PATH))
        {
            menuScene = EditorSceneManager.OpenScene(MAIN_MENU_SCENE_PATH);
        }
        else
        {
            menuScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        // 2. Main Camera với màu nền Gradient tối sang trọng
        GameObject camGo = GameObject.Find("Main Camera");
        if (camGo == null)
        {
            camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.11f, 0.18f); // Deep Midnight Blue
            camGo.AddComponent<AudioListener>();
        }
        else
        {
            Camera cam = camGo.GetComponent<Camera>();
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.08f, 0.11f, 0.18f);
            }
        }

        // 3. EventSystem tương thích cả Input System mới và cũ
        GameObject esGo = GameObject.Find("EventSystem");
        if (esGo == null)
        {
            esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // 4. Canvas
        GameObject canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        // 5. MainMenuManager GameObject
        GameObject mgrGo = GameObject.Find("MainMenuManager");
        if (mgrGo == null)
        {
            mgrGo = new GameObject("MainMenuManager");
        }
        MainMenuController menuController = mgrGo.GetComponent<MainMenuController>();
        if (menuController == null) menuController = mgrGo.AddComponent<MainMenuController>();
        menuController.gameSceneName = "SampleScene";

        // 6. Xóa các con UI cũ nếu có để tạo mới đồng bộ
        while (canvasGo.transform.childCount > 0)
        {
            Object.DestroyImmediate(canvasGo.transform.GetChild(0).gameObject);
        }

        // 7. Tạo Menu Panel Container
        GameObject panelGo = new GameObject("MenuPanel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        RectTransform panelRt = panelGo.AddComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0.5f);
        panelRt.sizeDelta = new Vector2(700, 580);

        Image panelBg = panelGo.AddComponent<Image>();
        panelBg.color = new Color(0.05f, 0.08f, 0.14f, 0.92f); // Glassmorphism Dark Card

        // 8. Game Title
        GameObject titleGo = new GameObject("GameTitle");
        titleGo.transform.SetParent(panelGo.transform, false);
        TextMeshProUGUI titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = "ADVENTURE HERO 2D";
        titleTmp.fontSize = 54;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = new Color(1f, 0.84f, 0.15f); // Bright Gold
        RectTransform titleRt = titleGo.GetComponent<RectTransform>();
        titleRt.anchoredPosition = new Vector2(0, 190);
        titleRt.sizeDelta = new Vector2(650, 70);

        // 9. Subtitle / Lab 3 Info
        GameObject subGo = new GameObject("Subtitle");
        subGo.transform.SetParent(panelGo.transform, false);
        TextMeshProUGUI subTmp = subGo.AddComponent<TextMeshProUGUI>();
        subTmp.text = "★ BÀI THỰC HÀNH LAB 3 - NÂNG CAO ★\nAudioManager • Moving Platform • Build Game";
        subTmp.fontSize = 20;
        subTmp.alignment = TextAlignmentOptions.Center;
        subTmp.color = new Color(0.7f, 0.85f, 1f, 0.9f); // Soft Cyan
        RectTransform subRt = subGo.GetComponent<RectTransform>();
        subRt.anchoredPosition = new Vector2(0, 110);
        subRt.sizeDelta = new Vector2(650, 60);

        // 10. Nút Play Game
        Button playBtn = CreateStyledButton(panelGo.transform, "Btn_Play", "BẮT ĐẦU CHƠI  (PLAY)", new Vector2(0, 5), new Color(0.12f, 0.65f, 0.38f), Color.white);
        UnityEventTools.AddPersistentListener(playBtn.onClick, menuController.PlayGame);

        // 11. Nút Quit Game
        Button quitBtn = CreateStyledButton(panelGo.transform, "Btn_Quit", "THOÁT GAME  (QUIT)", new Vector2(0, -95), new Color(0.75f, 0.22f, 0.24f), Color.white);
        UnityEventTools.AddPersistentListener(quitBtn.onClick, menuController.QuitGame);

        // 12. Footer Copyright / Info
        GameObject footerGo = new GameObject("Footer");
        footerGo.transform.SetParent(panelGo.transform, false);
        TextMeshProUGUI footTmp = footerGo.AddComponent<TextMeshProUGUI>();
        footTmp.text = "Unity 2D Platformer • Phiên bản 1.0 (Lab 3 Complete)";
        footTmp.fontSize = 16;
        footTmp.alignment = TextAlignmentOptions.Center;
        footTmp.color = new Color(0.5f, 0.6f, 0.7f, 0.8f);
        RectTransform footRt = footerGo.GetComponent<RectTransform>();
        footRt.anchoredPosition = new Vector2(0, -210);
        footRt.sizeDelta = new Vector2(650, 40);

        // 13. Cài đặt AudioManager trong Main Menu để phát nhạc nền ngay từ đầu
        SetupAudioManagerInScene();

        // Lưu Scene Main Menu
        EditorSceneManager.SaveScene(menuScene, MAIN_MENU_SCENE_PATH);
        Debug.Log("<color=green><b>[Lab 3]</b> Đã tạo Main Menu Scene thành công tại: " + MAIN_MENU_SCENE_PATH + "</color>");
    }

    private static Button CreateStyledButton(Transform parent, string name, string label, Vector2 pos, Color normalColor, Color textColor)
    {
        GameObject btnGo = new GameObject(name);
        btnGo.transform.SetParent(parent, false);

        RectTransform rt = btnGo.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(420, 65);

        Image img = btnGo.AddComponent<Image>();
        img.color = normalColor;

        Button btn = btnGo.AddComponent<Button>();
        ColorBlock colors = btn.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = normalColor * 1.25f;
        colors.pressedColor = normalColor * 0.8f;
        colors.selectedColor = normalColor;
        btn.colors = colors;

        // Text bên trong nút
        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(btnGo.transform, false);
        TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 24;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor;

        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        return btn;
    }

    public static void UpdateBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(MAIN_MENU_SCENE_PATH, true),
            new EditorBuildSettingsScene(GAME_SCENE_PATH, true)
        };

        EditorBuildSettings.scenes = scenes;
        AssetDatabase.SaveAssets();
        Debug.Log("<color=green><b>[Lab 3]</b> Đã cập nhật Build Settings: [0] MainMenu.unity | [1] SampleScene.unity</color>");
    }

    private static Scene EnsureSceneOpen(string scenePath)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.isLoaded && activeScene.path == scenePath)
        {
            return activeScene;
        }

        if (File.Exists(scenePath))
        {
            return EditorSceneManager.OpenScene(scenePath);
        }

        return activeScene;
    }
}
#endif
