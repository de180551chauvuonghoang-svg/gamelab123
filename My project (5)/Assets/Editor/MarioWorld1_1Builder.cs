#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Trình tạo màn chơi kinh điển Super Mario Bros World 1-1 hoàn chỉnh:
/// - Khối [?] nảy Nấm Lớn, Hoa Lửa, Sao Bất Tử, Coin.
/// - Khối Gạch phá vụn khi Super Mario húc đầu.
/// - Quái Nấm Goomba (dẫm bẹp) và Rùa Koopa (đá mai rùa càn quét).
/// - 4 Ống Nước (Warp Pipe) có ống chui xuống Hầm Bí Mật gom 25 xu.
/// - Kim tự tháp bậc thang dẫn đến Cột Cờ Về Đích (Flagpole) & Lâu Đài pháo hoa!
/// </summary>
public class MarioWorld1_1Builder
{
    private static Sprite questionBlockSprite;
    private static Sprite brickBlockSprite;
    private static Sprite emptyBlockSprite;
    private static Sprite goombaSprite;
    private static Sprite koopaSprite;
    private static Sprite koopaShellSprite;
    private static Sprite pipeTopSprite;
    private static Sprite pipeBodySprite;
    private static Sprite flagpoleSprite;
    private static Sprite castleSprite;
    private static Sprite coinSprite;
    private static Sprite groundSprite;
    private static Sprite cloudSprite;
    private static Sprite bushSprite;
    private static Sprite hillLargeSprite;
    private static Sprite hillSmallSprite;
    private static Material unlitMat;

    [MenuItem("Lab 2/🍄 [KHUYÊN DÙNG] Xây Dựng Mario Vào SampleScene & Mở Chơi Ngay", false, 0)]
    public static void BuildToSampleSceneAndOpen()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Đang Chạy Play", "Vui lòng dừng Play trước khi tạo màn chơi Mario!", "OK");
            return;
        }

        // 1. Lưu scene hiện tại nếu có chỉnh sửa
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        // 2. Mở SampleScene.unity
        Scene sampleScene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);

        // 3. Khởi tạo lại toàn bộ Sprite chuẩn Mario
        MarioAssetGenerator.GenerateAllMarioSprites();

        // 4. Dọn dẹp/ẩn các vật thể cũ của Lab 3
        CleanupOrHideOldLabObjects();

        // 5. Xây dựng World 1-1
        BuildWorld1_1();

        // 6. Lưu scene
        EditorSceneManager.SaveScene(sampleScene);

        EditorUtility.DisplayDialog(
            "Hoàn Tất 100%!",
            "Đã cài đặt thành công Super Mario Bros World 1-1 vào SampleScene!\n\n" +
            "👉 Unity hiện đang mở trực tiếp scene 'SampleScene'.\n" +
            "👉 Bạn chỉ cần bấm nút PLAY ▶ trên thanh công cụ Unity để chơi ngay lập tức!\n\n" +
            "Không còn bất kỳ menu Lab 3 nào che màn hình nữa!",
            "Chơi Ngay"
        );
    }

    [MenuItem("Lab 2/🍄 Xây Dựng Màn Chơi Mario (Vào Scene Đang Mở Hiện Tại)", false, 1)]
    public static void BuildMarioWorldMenu()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "Đang Trong Play Mode",
                "Vui lòng dừng Play trước khi tạo màn chơi Mario!\nSau khi tạo xong, bạn nhấn Play để trải nghiệm.",
                "Đã hiểu"
            );
            return;
        }

        // Tạo trước toàn bộ sprite nếu chưa có
        MarioAssetGenerator.GenerateAllMarioSprites();

        // Tự động ẩn MenuPanel nếu đang ở trong scene MainMenu để không bị che
        GameObject menuPanel = GameObject.Find("MenuPanel");
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            Debug.Log("<color=cyan><b>[Mario] Đã tạm thời ẩn MenuPanel trong MainMenu scene để màn hình game thông thoáng!</b></color>");
        }

        BuildWorld1_1();

        EditorUtility.DisplayDialog(
            "Thành Công!",
            "Đã xây dựng thành công Màn Chơi Super Mario Bros World 1-1 Hoàn Chỉnh!\n\n" +
            "Gồm đầy đủ tính năng:\n" +
            "1. Khối [?] đập đầu nảy Coin & Nấm Lớn, Hoa Lửa, Sao Bất Tử\n" +
            "2. Khối gạch vỡ vụn 4 mảnh khi Super Mario húc đầu\n" +
            "3. Nấm Goomba (dẫm bẹp) & Rùa Koopa (đá mai rùa càn quét)\n" +
            "4. Ống nước chui (nhấn S / Mũi tên xuống) vào hầm ngầm 25 xu\n" +
            "5. Bậc thang, Cột cờ tụt xuống & Lâu Đài bắn pháo hoa về đích\n" +
            "6. HUD MARIO, COIN, WORLD 1-1, TIME 400s chuẩn Nintendo\n\n" +
            "Bấm PLAY ▶ để bắt đầu chơi!",
            "Chơi Ngay"
        );
    }

    [MenuItem("Lab 2/🍄 Bật / Tắt MenuPanel Trong Scene MainMenu", false, 2)]
    public static void ToggleMenuPanel()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform panelTrans = canvas.transform.Find("MenuPanel");
            if (panelTrans != null)
            {
                bool newState = !panelTrans.gameObject.activeSelf;
                panelTrans.gameObject.SetActive(newState);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                Debug.Log($"<color=yellow><b>[MenuPanel] Trạng thái hiện tại: {(newState ? "HIỆN (ON)" : "ẨN (OFF - Để chơi game trực tiếp)")}</b></color>");
                return;
            }
        }

        EditorUtility.DisplayDialog("Thông Báo", "Không tìm thấy MenuPanel trong scene hiện tại!", "OK");
    }

    public static void BuildWorld1_1()
    {
        // 0. Dọn dẹp/ẩn sạch sẽ toàn bộ các vật thể cũ của Lab 2 (spikes, slime, tilemap xám, score text, v.v.)
        CleanupOrHideOldLabObjects();

        LoadSprites();

        Scene scene = SceneManager.GetActiveScene();

        // 1. Dọn dẹp thế giới Mario cũ
        GameObject oldWorld = GameObject.Find("Mario_World_1_1");
        if (oldWorld != null)
        {
            Undo.DestroyObjectImmediate(oldWorld);
        }

        GameObject oldMetroid = GameObject.Find("Metroidvania_World");
        if (oldMetroid != null)
        {
            Undo.DestroyObjectImmediate(oldMetroid);
        }

        GameObject worldRoot = new GameObject("Mario_World_1_1");
        Undo.RegisterCreatedObjectUndo(worldRoot, "Build Mario World");

        // 2. Chuyển Camera sang màu trời xanh Mario kinh điển (#5C94FC)
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.36f, 0.58f, 0.98f, 1f); // Sky Blue Mario
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            CameraFollow cf = mainCam.GetComponent<CameraFollow>();
            if (cf != null)
            {
                cf.offset = new Vector3(2f, 1.2f, -10f);
                cf.smoothSpeed = 0.15f;
            }
        }

        // 3. Setup Player
        SetupPlayer();

        // 4. Setup Mario HUD Manager
        SetupMarioHUD();

        // 5. Xây dựng cảnh nền Parallax (Mây trời, Đồi xanh, Bụi cỏ Mario)
        BuildBackgroundScenery(worldRoot.transform);

        // 6. Xây dựng địa hình sàn đất Mario mới (Đất ấm + Cỏ xanh rì)
        BuildGroundAndPits(worldRoot.transform);

        // 7. Xây dựng Khối gạch & Khối chấm hỏi
        BuildBlocksAndItems(worldRoot.transform);

        // 8. Xây dựng các ống nước & hầm bí mật
        BuildPipesAndUnderground(worldRoot.transform);

        // 9. Bố trí kẻ địch Goomba & Koopa
        BuildEnemies(worldRoot.transform);

        // 10. Xây dựng Bậc thang, Cột cờ & Lâu đài chiến thắng
        BuildFlagpoleAndCastle(worldRoot.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("<color=#55FF55><b>[Super Mario] Màn chơi World 1-1 hoàn chỉnh với phong cách Mario mới đã sẵn sàng!</b></color>");
    }

    private static void CleanupOrHideOldLabObjects()
    {
        string[] oldNames = new string[] {
            "Metroidvania_World", "Citadel_Ground_1", "Cavern_Abyss_Spikes", "Relic_DoubleJump",
            "PointA", "Campfire_Cavern", "Chimney_Wall_L", "Chimney_Wall_R", "Zone3_Citadel", "Foreground",
            "Citadel_Guard", "Citadel_Guard_Group", "Pillar_Right", "Pillar_Left", "Citadel_Tower_Fl1", "Citadel_Tower_Fl2",
            "Slime_Patrol_1_Group", "Slime_Patrol_1", "Wall_Test", "Trigger_Cavern", "Trigger_Citadel", "Trigger_Peak",
            "Cavern_Stalker", "Cavern_Stalker_Group", "Zone4_CelestialPeak", "Grid", "Ground_Tilemap", "Plains_Ground",
            "Spike_Plains", "Trap_Spikes", "Spikes_Citadel", "Enemy", "Enemy_Patrol_Group", "Cavern_Mover", "Cavern_Mover_Group",
            "The_Ancient_Gate", "Campfire_Peak", "Campfire_Plains", "Campfire_Citadel", "Totem_Start", "Totem_Cavern", "Totem_Citadel",
            "Totem_Gate", "Emerald_Pillar", "Sky_Island_1", "Sky_Island_2", "Sky_Island_3", "Sky_Island_4", "Arch_Top",
            "Gem_Sapphire", "Gem_Ruby", "Gem_Emerald", "Ruby_Altar", "Sapphire_Altar", "Relic_Altar_DJ", "Relic_Altar_Dash",
            "Relic_AirDash", "Key_WinItem", "Cavern_Ceiling", "Cavern_Floor_1", "Cavern_Floor_2", "Temple_Plaza", "Shaft_LeftWall",
            "Shaft_RightWall", "MovingPlatform", "Moving_Platform_Group", "Zone1_WhisperingPlains", "Zone2_CrystalCavern",
            "LeftBoundaryWall", "Ground", "Coin"
        };

        GameObject backup = GameObject.Find("Old_Lab_Environment_Backup");
        if (backup == null)
        {
            backup = new GameObject("Old_Lab_Environment_Backup");
        }

        GameObject[] allSceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var obj in allSceneObjects)
        {
            if (obj == null || obj == backup) continue;

            // Kiểm tra theo danh sách tên hoặc tiền tố
            bool match = false;
            foreach (string name in oldNames)
            {
                if (obj.name == name) { match = true; break; }
            }
            if (obj.name.StartsWith("Enemy_") && obj.name != "Enemy_Goomba" && obj.name != "Enemy_Koopa") match = true;

            if (match)
            {
                if (obj.transform.IsChildOf(backup.transform)) continue;
                GameObject marioWorld = GameObject.Find("Mario_World_1_1");
                if (marioWorld != null && obj.transform.IsChildOf(marioWorld.transform)) continue;
                if (obj.name == "Player" || obj.name == "Main Camera" || obj.name == "MarioGameManager" || obj.name == "AudioManager") continue;

                obj.transform.SetParent(backup.transform);
            }
        }

        // Ẩn chữ ScoreText cũ của Lab 2 trong Canvas để nhường chỗ cho HUD Mario
        GameObject scoreText = GameObject.Find("ScoreText");
        if (scoreText != null)
        {
            scoreText.SetActive(false);
        }

        // Tắt toàn bộ môi trường cũ để không che hay va chạm với Mario
        backup.SetActive(false);
    }

    public static Sprite SafeLoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        // 1. Thử load dạng Sprite đơn
        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sp != null) return sp;

        // 2. Thử load sub-assets nếu là Sprite multiple
        Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        if (subAssets != null)
        {
            foreach (var sub in subAssets)
            {
                if (sub is Sprite s) return s;
            }
        }

        Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
        if (allAssets != null)
        {
            foreach (var obj in allAssets)
            {
                if (obj is Sprite s) return s;
            }
        }

        // 3. Fallback: Nếu là Texture2D, tạo Sprite trên bộ nhớ
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (tex != null)
        {
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                16f
            );
        }

        return null;
    }

    private static void LoadSprites()
    {
        questionBlockSprite = SafeLoadSprite("Assets/Sprites/Mario/Block_Question.png");
        brickBlockSprite = SafeLoadSprite("Assets/Sprites/Mario/Block_Brick.png");
        emptyBlockSprite = SafeLoadSprite("Assets/Sprites/Mario/Block_Empty.png");
        goombaSprite = SafeLoadSprite("Assets/Sprites/Mario/Enemy_Goomba.png");
        koopaSprite = SafeLoadSprite("Assets/Sprites/Mario/Enemy_Koopa.png");
        koopaShellSprite = SafeLoadSprite("Assets/Sprites/Mario/Enemy_KoopaShell.png");
        pipeTopSprite = SafeLoadSprite("Assets/Sprites/Mario/Pipe_Top.png");
        pipeBodySprite = SafeLoadSprite("Assets/Sprites/Mario/Pipe_Body.png");
        flagpoleSprite = SafeLoadSprite("Assets/Sprites/Mario/Flagpole_Flag.png");
        castleSprite = SafeLoadSprite("Assets/Sprites/Mario/Castle.png");
        coinSprite = SafeLoadSprite("Assets/Sprites/Coin.png") ?? SafeLoadSprite("Assets/Sprites/Mario/Block_Question.png");
        groundSprite = SafeLoadSprite("Assets/Sprites/Mario/Mario_Ground.png") ?? SafeLoadSprite("Assets/Sprites/Ground.png");
        cloudSprite = SafeLoadSprite("Assets/Sprites/Mario/Mario_Cloud.png");
        bushSprite = SafeLoadSprite("Assets/Sprites/Mario/Mario_Bush.png");
        hillLargeSprite = SafeLoadSprite("Assets/Sprites/Mario/Mario_Hill_Large.png");
        hillSmallSprite = SafeLoadSprite("Assets/Sprites/Mario/Mario_Hill_Small.png");

        unlitMat = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat");
    }

    [MenuItem("Lab 2/🍄 Thiết Kế Lại Template Nhân Vật Mario Chuẩn (Pixel Art Đầy Đủ)", false, 3)]
    public static void GenerateMarioCharacterMenu()
    {
        MarioAssetGenerator.CreateMarioCharacterSprites();
        SetupPlayer();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorUtility.DisplayDialog(
            "Thành Công!",
            "Đã thiết kế lại toàn bộ template nhân vật Mario chuẩn Nintendo!\n\n" +
            "Bao gồm đầy đủ các bộ sprite:\n" +
            "• Small Mario: Đứng yên, Chạy 3 nhịp, Nhảy giơ tay đấm, Phanh gấp\n" +
            "• Super Mario: Đứng yên, Chạy, Nhảy vươn người, Ngồi cúi (Crouch)\n" +
            "• Fire Mario: Mũ & áo trắng, yếm đỏ, tư thế vung tay bắn lửa\n" +
            "• Starman: Hiệu ứng cầu vồng chớp nháy siêu tốc\n" +
            "• Đã tự động cập nhật Assets/Sprites/Player.png thành Mario\n\n" +
            "Nhân vật Player đã được trang bị MarioVisualController để tự động chuyển động hoạt ảnh!",
            "Tuyệt Vời"
        );
    }

    private static void SetupPlayer()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(1f, -1.8f, 0);

            // Gắn hoặc làm mới MarioPowerUpManager
            MarioPowerUpManager pManager = player.GetComponent<MarioPowerUpManager>();
            if (pManager == null) pManager = player.AddComponent<MarioPowerUpManager>();
            pManager.currentForm = MarioPowerUpManager.MarioForm.Small;
            pManager.isStarman = false;

            // Gắn MarioVisualController để điều khiển hoạt ảnh Mario chuẩn
            MarioVisualController vCtrl = player.GetComponent<MarioVisualController>();
            if (vCtrl == null) vCtrl = player.AddComponent<MarioVisualController>();
            vCtrl.LoadDefaultMarioSprites();

            // Tắt Animator controller cũ nếu có
            Animator anim = player.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;

            // Cập nhật SpriteRenderer ngay lập tức sang Mario Small Idle
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Sprite marioIdle = SafeLoadSprite("Assets/Sprites/Mario/Mario_Small_Idle.png");
                if (marioIdle != null) sr.sprite = marioIdle;
                sr.sortingLayerName = "Player";
                sr.sortingOrder = 10;
            }

            // BoxCollider2D vừa khít với Mario Small (1m x 1m chuẩn)
            BoxCollider2D col = player.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.size = new Vector2(0.75f, 0.95f);
                col.offset = new Vector2(0f, 0.05f);
            }

            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.canDoubleJump = false; // Mario chuẩn không nhảy kép
                pc.canAirDash = false;
            }
        }
    }

    private static void SetupMarioHUD()
    {
        GameObject hudGo = GameObject.Find("MarioGameManager");
        if (hudGo == null)
        {
            hudGo = new GameObject("MarioGameManager");
        }
        MarioGameManager mgm = hudGo.GetComponent<MarioGameManager>();
        if (mgm == null) mgm = hudGo.AddComponent<MarioGameManager>();
        mgm.coins = 0;
        mgm.lives = 3;
        mgm.timeRemaining = 400f;
        mgm.worldName = "1-1";

        // Tắt ScoreText cũ của Lab 2
        GameObject oldScore = GameObject.Find("ScoreText");
        if (oldScore != null) oldScore.SetActive(false);
    }

    // ========================================================================
    // CẢNH NỀN PARALLAX (MÂY TRỜI, ĐỒI XANH, BỤI CÂY MARIO ĐẸP MẮT)
    // ========================================================================
    private static void BuildBackgroundScenery(Transform parent)
    {
        GameObject sceneryRoot = new GameObject("Background_Scenery");
        sceneryRoot.transform.SetParent(parent);

        // --- 1. LỚP ĐỒI XANH PHÍA XA (PARALLAX HILLS) ---
        GameObject hillsGroup = new GameObject("Scenery_Hills");
        hillsGroup.transform.SetParent(sceneryRoot.transform);
        ParallaxBackground hillParallax = hillsGroup.AddComponent<ParallaxBackground>();
        hillParallax.parallaxEffectMultiplier = new Vector2(0.25f, 0.05f);
        hillParallax.autoScrollSpeedX = 0f;

        float[] largeHillX = new float[] { -2f, 22f, 48f, 72f, 96f };
        foreach (float hx in largeHillX)
        {
            if (hillLargeSprite != null)
            {
                GameObject h = new GameObject($"Hill_Large_{hx}");
                h.transform.SetParent(hillsGroup.transform);
                h.transform.position = new Vector3(hx, -1.1f, 0);
                SpriteRenderer sr = h.AddComponent<SpriteRenderer>();
                sr.sprite = hillLargeSprite;
                sr.sortingLayerName = "Default";
                sr.sortingOrder = -30;
                h.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
            }
        }

        float[] smallHillX = new float[] { 9f, 34f, 60f, 84f };
        foreach (float sx in smallHillX)
        {
            if (hillSmallSprite != null)
            {
                GameObject h = new GameObject($"Hill_Small_{sx}");
                h.transform.SetParent(hillsGroup.transform);
                h.transform.position = new Vector3(sx, -1.75f, 0);
                SpriteRenderer sr = h.AddComponent<SpriteRenderer>();
                sr.sprite = hillSmallSprite;
                sr.sortingLayerName = "Default";
                sr.sortingOrder = -31;
                h.transform.localScale = new Vector3(2.0f, 2.0f, 1f);
            }
        }

        // --- 2. LỚP MÂY TRỜI LỮNG LỜ TRÔI (PARALLAX CLOUDS) ---
        GameObject cloudsGroup = new GameObject("Scenery_Clouds");
        cloudsGroup.transform.SetParent(sceneryRoot.transform);
        ParallaxBackground cloudParallax = cloudsGroup.AddComponent<ParallaxBackground>();
        cloudParallax.parallaxEffectMultiplier = new Vector2(0.12f, 0.02f);
        cloudParallax.autoScrollSpeedX = 0.25f; // Tự động trôi nhẹ 0.25m/s

        Vector3[] cloudPositions = new Vector3[] {
            new Vector3(3f, 4.2f, 0),
            new Vector3(17f, 5.0f, 0),
            new Vector3(31f, 3.8f, 0),
            new Vector3(45f, 4.6f, 0),
            new Vector3(59f, 4.0f, 0),
            new Vector3(73f, 5.2f, 0),
            new Vector3(87f, 4.3f, 0),
            new Vector3(101f, 4.7f, 0)
        };

        for (int i = 0; i < cloudPositions.Length; i++)
        {
            if (cloudSprite != null)
            {
                GameObject c = new GameObject($"Cloud_{i}");
                c.transform.SetParent(cloudsGroup.transform);
                c.transform.position = cloudPositions[i];
                SpriteRenderer sr = c.AddComponent<SpriteRenderer>();
                sr.sprite = cloudSprite;
                sr.sortingLayerName = "Default";
                sr.sortingOrder = -40;
                c.transform.localScale = new Vector3(2.2f, 2.2f, 1f);
            }
        }

        // --- 3. LỚP BỤI CÂY TRÊN MẶT ĐẤT (BUSHES) ---
        GameObject bushesGroup = new GameObject("Scenery_Bushes");
        bushesGroup.transform.SetParent(sceneryRoot.transform);

        float[] bushX = new float[] { 5f, 15f, 23.5f, 35.5f, 41.5f, 56f, 70f, 82f, 92f };
        foreach (float bx in bushX)
        {
            if (bushSprite != null)
            {
                GameObject b = new GameObject($"Bush_{bx}");
                b.transform.SetParent(bushesGroup.transform);
                b.transform.position = new Vector3(bx, -2.15f, 0);
                SpriteRenderer sr = b.AddComponent<SpriteRenderer>();
                sr.sprite = bushSprite;
                sr.sortingLayerName = "Default";
                sr.sortingOrder = -15;
                b.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
            }
        }
    }

    // ========================================================================
    // ĐỊA HÌNH SÀN ĐẤT & CÁC VỰC THẲM (PITS)
    // ========================================================================
    private static void BuildGroundAndPits(Transform parent)
    {
        GameObject terrain = new GameObject("Terrain_Grounds");
        terrain.transform.SetParent(parent);

        // Đoạn sàn 1: Từ X: -4 đến X: 42 (dài 46m)
        CreateGround(terrain.transform, "Ground_Part1", new Vector3(19f, -3.2f, 0), new Vector2(46f, 1.2f));

        // Vực thẳm 1: Rộng 3m từ X: 42 đến 45

        // Đoạn sàn 2: Từ X: 45 đến X: 58 (dài 13m)
        CreateGround(terrain.transform, "Ground_Part2", new Vector3(51.5f, -3.2f, 0), new Vector2(13f, 1.2f));

        // Vực thẳm 2: Rộng 3.5m từ X: 58 đến 61.5

        // Đoạn sàn 3: Từ X: 61.5 đến X: 115 (dài 53.5m - tới tận lâu đài)
        CreateGround(terrain.transform, "Ground_Part3", new Vector3(88.25f, -3.2f, 0), new Vector2(53.5f, 1.2f));

        // Tường chặn bên trái
        CreateWall(terrain.transform, "LeftWall", new Vector3(-4.5f, 2f, 0), new Vector2(1f, 12f));
    }

    // ========================================================================
    // KHỐI GẠCH & KHỐI [?] (BLOCKS & QUESTION BLOCKS)
    // ========================================================================
    private static void BuildBlocksAndItems(Transform parent)
    {
        GameObject blocks = new GameObject("Blocks_Container");
        blocks.transform.SetParent(parent);

        // Khối [?] đơn đầu tiên tại X: 8 (Chứa Coin)
        CreateQuestionBlock(blocks.transform, new Vector3(8f, -0.6f, 0), MysteryBlock.ContentType.Coin);

        // Cụm 5 khối kinh điển tại X: 12 đến 16:
        // [Brick] [?] (Mushroom) [Brick] [?] (Coin) [Brick]
        CreateBrickBlock(blocks.transform, new Vector3(12f, -0.6f, 0));
        CreateQuestionBlock(blocks.transform, new Vector3(13f, -0.6f, 0), MysteryBlock.ContentType.SuperMushroom);
        CreateBrickBlock(blocks.transform, new Vector3(14f, -0.6f, 0));
        CreateQuestionBlock(blocks.transform, new Vector3(15f, -0.6f, 0), MysteryBlock.ContentType.Coin);
        CreateBrickBlock(blocks.transform, new Vector3(16f, -0.6f, 0));

        // Khối [?] trên cao tại X: 14, Y: 2.5 (Coin)
        CreateQuestionBlock(blocks.transform, new Vector3(14f, 2.4f, 0), MysteryBlock.ContentType.Coin);

        // Hàng gạch dài giữa màn chơi (X: 47 đến 54)
        CreateBrickBlock(blocks.transform, new Vector3(47f, -0.6f, 0));
        CreateQuestionBlock(blocks.transform, new Vector3(48f, -0.6f, 0), MysteryBlock.ContentType.FireFlower);
        CreateBrickBlock(blocks.transform, new Vector3(49f, -0.6f, 0));
        CreateBrickBlock(blocks.transform, new Vector3(50f, -0.6f, 0));
        CreateQuestionBlock(blocks.transform, new Vector3(51f, -0.6f, 0), MysteryBlock.ContentType.Starman);
        CreateBrickBlock(blocks.transform, new Vector3(52f, -0.6f, 0));
        CreateQuestionBlock(blocks.transform, new Vector3(53f, -0.6f, 0), MysteryBlock.ContentType.Coin);
        CreateBrickBlock(blocks.transform, new Vector3(54f, -0.6f, 0));
    }

    // ========================================================================
    // ỐNG NƯỚC (WARP PIPES) & HẦM BÍ MẬT (UNDERGROUND)
    // ========================================================================
    private static void BuildPipesAndUnderground(Transform parent)
    {
        GameObject pipes = new GameObject("Pipes_Container");
        pipes.transform.SetParent(parent);

        // Ống 1: Thấp (2 khối cao) tại X: 20
        CreatePipe(pipes.transform, "Pipe_1", new Vector3(20f, -2f, 0), 2f, false, null);

        // Ống 2: Vừa (3 khối cao) tại X: 26
        CreatePipe(pipes.transform, "Pipe_2", new Vector3(26f, -1.5f, 0), 3f, false, null);

        // Ống 3: Cao (4 khối cao) tại X: 32
        CreatePipe(pipes.transform, "Pipe_3", new Vector3(32f, -1f, 0), 4f, false, null);

        // Ống 5 trên mặt đất (Cửa trồi lên khi ra khỏi hầm) tại X: 40
        GameObject exitPointGo = new GameObject("Underground_Exit_Surface");
        exitPointGo.transform.SetParent(pipes.transform);
        exitPointGo.transform.position = new Vector3(40f, -1f, 0);
        CreatePipe(pipes.transform, "Pipe_Surface_Exit", new Vector3(40f, -1f, 0), 4f, false, null);

        // HẦM BÍ MẬT DƯỚI LÒNG ĐẤT (Underground Bonus Room tại Y: -16)
        GameObject underRoom = new GameObject("Underground_Secret_Room");
        underRoom.transform.SetParent(parent);

        // Sàn hầm (màu gạch xanh đen)
        CreateGround(underRoom.transform, "Under_Floor", new Vector3(30f, -17f, 0), new Vector2(25f, 1.2f), new Color(0.2f, 0.35f, 0.6f));
        CreateGround(underRoom.transform, "Under_Ceiling", new Vector3(30f, -10f, 0), new Vector2(25f, 1.2f), new Color(0.2f, 0.35f, 0.6f));
        CreateWall(underRoom.transform, "Under_Wall_L", new Vector3(17f, -13.5f, 0), new Vector2(1f, 8f));
        CreateWall(underRoom.transform, "Under_Wall_R", new Vector3(43f, -13.5f, 0), new Vector2(1f, 8f));

        // Điểm hạ cánh trong hầm
        GameObject underEntranceGo = new GameObject("Underground_Entrance_Point");
        underEntranceGo.transform.SetParent(underRoom.transform);
        underEntranceGo.transform.position = new Vector3(21f, -15.5f, 0);

        // Ống 4 trên mặt đất (ỐNG CHUI ĐƯỢC - WARP PIPE!) tại X: 36
        CreatePipe(pipes.transform, "Pipe_Warp_Enter", new Vector3(36f, -1f, 0), 4f, true, underEntranceGo.transform);

        // Sinh 20 đồng xu xếp thành 2 hàng trong hầm bí mật
        for (int i = 0; i < 10; i++)
        {
            SpawnCoin(underRoom.transform, new Vector3(23f + i * 1.5f, -15f, 0));
            SpawnCoin(underRoom.transform, new Vector3(23f + i * 1.5f, -13f, 0));
        }

        // Ống thoát ra khỏi hầm (Exit Pipe) tại X: 39, Y: -15
        CreatePipe(underRoom.transform, "Under_Exit_Pipe", new Vector3(39f, -15f, 0), 3f, true, exitPointGo.transform);
    }

    // ========================================================================
    // KẺ ĐỊCH GOOMBA & KOOPA
    // ========================================================================
    private static void BuildEnemies(Transform parent)
    {
        GameObject enemyGroup = new GameObject("Enemies_Container");
        enemyGroup.transform.SetParent(parent);

        // Goomba 1 tại X: 15
        CreateGoomba(enemyGroup.transform, new Vector3(15f, -2.4f, 0));

        // Goomba 2 & 3 tại X: 22 và 24 (giữa Pipe 1 & 2)
        CreateGoomba(enemyGroup.transform, new Vector3(22.5f, -2.4f, 0));
        CreateGoomba(enemyGroup.transform, new Vector3(24f, -2.4f, 0));

        // Rùa Koopa Troopa tại X: 50 (Có thể dẫm lấy mai rùa đá!)
        CreateKoopa(enemyGroup.transform, new Vector3(50f, -2.4f, 0));

        // Goomba 4 & 5 xếp sau rùa để người chơi đá mai quét sạch
        CreateGoomba(enemyGroup.transform, new Vector3(54f, -2.4f, 0));
        CreateGoomba(enemyGroup.transform, new Vector3(56f, -2.4f, 0));

        // Koopa 2 tại X: 68
        CreateKoopa(enemyGroup.transform, new Vector3(68f, -2.4f, 0));
    }

    // ========================================================================
    // BẬC THANG, CỘT CỜ (FLAGPOLE) & LÂU ĐÀI (CASTLE)
    // ========================================================================
    private static void BuildFlagpoleAndCastle(Transform parent)
    {
        GameObject endArea = new GameObject("End_Area_Container");
        endArea.transform.SetParent(parent);

        // Kim tự tháp bậc thang bậc 1 đến 8 (X: 74 đến 81)
        for (int step = 1; step <= 8; step++)
        {
            float x = 73f + step;
            for (int h = 0; h < step; h++)
            {
                float y = -2.5f + h * 0.6f;
                CreateSolidBlock(endArea.transform, new Vector3(x, y, 0));
            }
        }

        // Cột cờ tại X: 86
        GameObject flagpoleGo = new GameObject("Mario_Flagpole");
        flagpoleGo.transform.SetParent(endArea.transform);
        flagpoleGo.transform.position = new Vector3(86f, -2.5f, 0);

        // Thân cột cờ
        GameObject pole = new GameObject("Pole");
        pole.transform.SetParent(flagpoleGo.transform);
        pole.transform.position = new Vector3(86f, 1.8f, 0);
        SpriteRenderer poleSr = pole.AddComponent<SpriteRenderer>();
        if (flagpoleSprite != null) poleSr.sprite = flagpoleSprite;
        poleSr.drawMode = SpriteDrawMode.Tiled;
        poleSr.size = new Vector2(0.3f, 8.5f);
        poleSr.sortingLayerName = "Player";
        if (unlitMat != null) poleSr.material = unlitMat;

        // Lá cờ trên đỉnh cột
        GameObject flag = new GameObject("Flag");
        flag.transform.SetParent(flagpoleGo.transform);
        flag.transform.position = new Vector3(85.6f, 5.5f, 0);
        SpriteRenderer flagSr = flag.AddComponent<SpriteRenderer>();
        if (flagpoleSprite != null) flagSr.sprite = flagpoleSprite;
        flagSr.size = new Vector2(0.8f, 0.8f);
        flagSr.sortingLayerName = "Player";
        if (unlitMat != null) flagSr.material = unlitMat;

        // Collider bắt va chạm Mario
        BoxCollider2D col = flagpoleGo.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.8f, 9f);
        col.offset = new Vector2(0f, 4.5f);

        // Điểm đỉnh và đáy
        GameObject topPt = new GameObject("TopPoint");
        topPt.transform.SetParent(flagpoleGo.transform);
        topPt.transform.position = new Vector3(86f, 5.5f, 0);

        GameObject btmPt = new GameObject("BottomPoint");
        btmPt.transform.SetParent(flagpoleGo.transform);
        btmPt.transform.position = new Vector3(86f, -2.5f, 0);

        // Lâu Đài tại X: 94
        GameObject castle = new GameObject("Mario_Castle");
        castle.transform.SetParent(endArea.transform);
        castle.transform.position = new Vector3(94f, -0.6f, 0);

        SpriteRenderer cSr = castle.AddComponent<SpriteRenderer>();
        if (castleSprite != null) cSr.sprite = castleSprite;
        cSr.sortingLayerName = "Default";
        if (unlitMat != null) cSr.material = unlitMat;
        castle.transform.localScale = new Vector3(2.5f, 2.5f, 1f);

        // Cửa lâu đài
        GameObject doorPt = new GameObject("DoorPoint");
        doorPt.transform.SetParent(castle.transform);
        doorPt.transform.position = new Vector3(94f, -2.5f, 0);

        // Script Flagpole
        MarioFlagpole mfp = flagpoleGo.AddComponent<MarioFlagpole>();
        mfp.flagTransform = flag.transform;
        mfp.poleTopPoint = topPt.transform;
        mfp.poleBottomPoint = btmPt.transform;
        mfp.castleDoorPoint = doorPt.transform;
    }

    // ========================================================================
    // HELPER CREATORS
    // ========================================================================
    private static void CreateGround(Transform parent, string name, Vector3 pos, Vector2 size, Color? tint = null)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = pos;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        if (groundSprite != null) sr.sprite = groundSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = size;
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 5;
        if (unlitMat != null) sr.material = unlitMat;
        if (tint.HasValue) sr.color = tint.Value;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = size;

        go.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;
    }

    private static void CreateWall(Transform parent, string name, Vector3 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = pos;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = size;
        go.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;
    }

    private static void CreateQuestionBlock(Transform parent, Vector3 pos, MysteryBlock.ContentType content)
    {
        GameObject qb = new GameObject("Block_Question");
        qb.transform.SetParent(parent);
        qb.transform.position = pos;

        SpriteRenderer sr = qb.AddComponent<SpriteRenderer>();
        if (questionBlockSprite != null) sr.sprite = questionBlockSprite;
        sr.sortingLayerName = "Player";
        if (unlitMat != null) sr.material = unlitMat;

        BoxCollider2D col = qb.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.98f, 0.98f);

        qb.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;

        MysteryBlock mb = qb.AddComponent<MysteryBlock>();
        mb.content = content;
        mb.emptyBlockSprite = emptyBlockSprite;
    }

    private static void CreateBrickBlock(Transform parent, Vector3 pos)
    {
        GameObject bb = new GameObject("Block_Brick");
        bb.transform.SetParent(parent);
        bb.transform.position = pos;

        SpriteRenderer sr = bb.AddComponent<SpriteRenderer>();
        if (brickBlockSprite != null) sr.sprite = brickBlockSprite;
        sr.sortingLayerName = "Player";
        if (unlitMat != null) sr.material = unlitMat;

        BoxCollider2D col = bb.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.98f, 0.98f);

        bb.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;
        bb.AddComponent<BrickBlock>();
    }

    private static void CreateSolidBlock(Transform parent, Vector3 pos)
    {
        GameObject sb = new GameObject("Block_Solid");
        sb.transform.SetParent(parent);
        sb.transform.position = pos;

        SpriteRenderer sr = sb.AddComponent<SpriteRenderer>();
        if (emptyBlockSprite != null) sr.sprite = emptyBlockSprite;
        sr.sortingLayerName = "Player";
        if (unlitMat != null) sr.material = unlitMat;

        BoxCollider2D col = sb.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.98f, 0.98f);
        sb.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;
    }

    private static void CreatePipe(Transform parent, string name, Vector3 pos, float height, bool isWarp, Transform warpDest)
    {
        GameObject pipe = new GameObject(name);
        pipe.transform.SetParent(parent);
        pipe.transform.position = pos;

        // Pipe Top
        GameObject top = new GameObject("Top");
        top.transform.SetParent(pipe.transform);
        top.transform.position = pos + Vector3.up * (height * 0.5f - 0.5f);
        SpriteRenderer srTop = top.AddComponent<SpriteRenderer>();
        if (pipeTopSprite != null) srTop.sprite = pipeTopSprite;
        srTop.sortingLayerName = "Player";
        if (unlitMat != null) srTop.material = unlitMat;
        top.transform.localScale = new Vector3(1.1f, 1f, 1f);

        // Pipe Body
        GameObject body = new GameObject("Body");
        body.transform.SetParent(pipe.transform);
        body.transform.position = pos - Vector3.up * 0.4f;
        SpriteRenderer srBody = body.AddComponent<SpriteRenderer>();
        if (pipeBodySprite != null) srBody.sprite = pipeBodySprite;
        srBody.drawMode = SpriteDrawMode.Tiled;
        srBody.size = new Vector2(1.8f, height - 0.8f);
        srBody.sortingLayerName = "Player";
        if (unlitMat != null) srBody.material = unlitMat;

        // Collider
        BoxCollider2D col = pipe.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.8f, height);
        pipe.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;

        // Warp Pipe script
        if (isWarp)
        {
            // Trigger vùng miệng ống
            GameObject triggerGo = new GameObject("WarpTrigger");
            triggerGo.transform.SetParent(pipe.transform);
            triggerGo.transform.position = pos + Vector3.up * (height * 0.5f);

            BoxCollider2D tCol = triggerGo.AddComponent<BoxCollider2D>();
            tCol.isTrigger = true;
            tCol.size = new Vector2(1.2f, 0.5f);

            WarpPipe wp = triggerGo.AddComponent<WarpPipe>();
            wp.isEnterable = true;
            wp.destinationPoint = warpDest;
        }
    }

    private static void CreateGoomba(Transform parent, Vector3 pos)
    {
        GameObject g = new GameObject("Goomba");
        g.transform.SetParent(parent);
        g.transform.position = pos;

        SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
        if (goombaSprite != null) sr.sprite = goombaSprite;
        sr.sortingLayerName = "Player";
        if (unlitMat != null) sr.material = unlitMat;

        BoxCollider2D col = g.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 0.8f);

        Rigidbody2D rb = g.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        g.AddComponent<GoombaEnemy>();
    }

    private static void CreateKoopa(Transform parent, Vector3 pos)
    {
        GameObject k = new GameObject("Koopa");
        k.transform.SetParent(parent);
        k.transform.position = pos;

        SpriteRenderer sr = k.AddComponent<SpriteRenderer>();
        if (koopaSprite != null) sr.sprite = koopaSprite;
        sr.sortingLayerName = "Player";
        if (unlitMat != null) sr.material = unlitMat;

        BoxCollider2D col = k.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 1f);

        Rigidbody2D rb = k.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        KoopaEnemy ke = k.AddComponent<KoopaEnemy>();
        ke.koopaWalkSprite = koopaSprite;
        ke.shellSprite = koopaShellSprite;
    }

    private static void SpawnCoin(Transform parent, Vector3 pos)
    {
        GameObject coin = new GameObject("Coin");
        coin.transform.SetParent(parent);
        coin.transform.position = pos;

        SpriteRenderer sr = coin.AddComponent<SpriteRenderer>();
        if (coinSprite != null) sr.sprite = coinSprite;
        sr.sortingLayerName = "Player";
        if (unlitMat != null) sr.material = unlitMat;

        CircleCollider2D col = coin.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.2f;

        coin.AddComponent<Coin>();
    }
}
#endif
