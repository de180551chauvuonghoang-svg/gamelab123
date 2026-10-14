#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Trình tạo thế giới mở Metroidvania tự động:
/// - 4 Phân khu sinh thái (Plains, Crystal Cavern, Citadel, Celestial Peak)
/// - 3 Cổ Ngọc (Emerald, Ruby, Sapphire)
/// - 2 Cổ Vật kỹ năng (Double Jump Boots, Air Dash Rune)
/// - Trạm Lửa Trại Checkpoint & Bia Đá Cổ
/// - Cổng Phong Ấn Cổ Đại hoàn thành trò chơi
/// </summary>
public class MetroidvaniaWorldBuilder
{
    private static Sprite groundSprite;
    private static Sprite wallSprite;
    private static Sprite movingPlatformSprite;
    private static Sprite trapSprite;
    private static Sprite enemySprite;
    private static Sprite coinSprite;
    private static Sprite keySprite;
    private static Material unlitMaterial;

    [MenuItem("Lab 2/🌟 Xây Dựng Thế Giới Mở Metroidvania (140m+)", false, 0)]
    public static void BuildWorldMenu()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "Đang Trong Play Mode",
                "Vui lòng nhấn nút Dừng Play (tắt nút ▶ màu xanh) trước khi tạo thế giới mở!\n\nSau khi tạo xong trong chế độ Editor, bạn nhấn Play (▶) để bắt đầu chơi.",
                "Đã hiểu"
            );
            return;
        }

        BuildEntireOpenWorld();
        EditorUtility.DisplayDialog(
            "Thế Giới Mở Đã Khởi Tạo!",
            "Đã xây dựng thành công Thế Giới Mở Metroidvania rộng lớn hơn 140 mét!\n\n" +
            "Gồm 4 phân khu:\n" +
            "1. 🌲 Đồng Cỏ Khởi Nguyên (Whispering Plains)\n" +
            "2. 🪨 Hang Pha Lê Ngầm (Crystal Cavern) - Nơi giấu Giày Nhảy Kép & Ngọc Lục Bảo\n" +
            "3. 🏰 Thành Cổ Tàn Tích (Citadel) - Nơi giấu Cổ Tự Lướt Gió (Shift) & Ngọc Hồng Bảo\n" +
            "4. ☁️ Đỉnh Tháp Mây (Celestial Peak) - Nơi giấu Ngọc Lam Bảo & Cổng Phong Ấn Cổ Đại\n\n" +
            "Bấm PLAY ▶ để bắt đầu cuộc phiêu lưu!",
            "Bắt Đầu Ngay"
        );
    }

    public static void BuildEntireOpenWorld()
    {
        LoadAssets();

        Scene scene = SceneManager.GetActiveScene();

        // 1. Dọn dẹp các vật thể thế giới mở cũ nếu có
        GameObject oldWorld = GameObject.Find("Metroidvania_World");
        if (oldWorld != null)
        {
            if (EditorApplication.isPlaying) Object.Destroy(oldWorld);
            else Undo.DestroyObjectImmediate(oldWorld);
        }

        // Tạo root container
        GameObject worldRoot = new GameObject("Metroidvania_World");
        if (!EditorApplication.isPlaying)
        {
            Undo.RegisterCreatedObjectUndo(worldRoot, "Build Metroidvania World");
        }

        // 2. Setup Quest Manager & UI HUD
        SetupQuestManager();

        // 3. Đặt vị trí xuất phát cho Player
        SetupPlayerStartPosition();

        // 4. Xây dựng từng phân khu
        BuildZone1_Plains(worldRoot.transform);
        BuildZone2_Cavern(worldRoot.transform);
        BuildZone3_Citadel(worldRoot.transform);
        BuildZone4_Peak(worldRoot.transform);

        // Đánh dấu Scene đã sửa (chỉ khi không trong Play mode)
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }
        Debug.Log("<color=#55FF55><b>[Metroidvania] Thế giới mở 140m+ đã xây dựng thành công!</b></color>");
    }

    private static void LoadAssets()
    {
        groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Ground.png");
        wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Wall.png");
        movingPlatformSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/MovingPlatform.png");
        trapSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Trap.png");
        enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Enemy.png");
        coinSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Coin.png");
        keySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Key.png");

        Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit")
                          ?? Shader.Find("Sprites/Default");
        if (unlitShader != null)
        {
            unlitMaterial = new Material(unlitShader);
        }
    }

    private static void SetupPlayerStartPosition()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(-1f, -1.8f, 0f);

            // Gán layer Ground cho groundCheck
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.canDoubleJump = false; // Phải nhặt relic mới có
                pc.canAirDash = false;    // Phải nhặt relic mới có
            }
        }
    }

    private static void SetupQuestManager()
    {
        GameObject qmGo = GameObject.Find("QuestManager");
        if (qmGo == null)
        {
            qmGo = new GameObject("QuestManager");
        }
        MetroidvaniaQuestManager qm = qmGo.GetComponent<MetroidvaniaQuestManager>();
        if (qm == null) qm = qmGo.AddComponent<MetroidvaniaQuestManager>();
        qm.hasDoubleJump = false;
        qm.hasAirDash = false;
        qm.hasEmerald = false;
        qm.hasRuby = false;
        qm.hasSapphire = false;
    }

    // ========================================================================
    // PHÂN KHU 1: ĐỒNG CỎ KHỞI NGUYÊN (X: -5 -> 30, Y: -3 -> 2)
    // ========================================================================
    private static void BuildZone1_Plains(Transform parent)
    {
        GameObject zone = new GameObject("Zone1_WhisperingPlains");
        zone.transform.SetParent(parent);

        // Mặt đất khởi đầu
        CreateGroundPlatform(zone.transform, "Plains_Ground", new Vector3(12.5f, -3.2f, 0), new Vector2(35f, 1f));

        // Tường chặn bên trái để không đi ngược rơi khỏi map
        CreateWall(zone.transform, "LeftBoundaryWall", new Vector3(-5.5f, 2f, 0), new Vector2(1f, 12f));

        // Cột mốc Bia Đá Cổ #1
        CreateLoreTotem(zone.transform, "Totem_Start", new Vector3(1.5f, -2.2f, 0),
            "Lời sấm cổ xưa: Hãy gieo mình xuống Giếng Thần phía trước để vào Hang Pha Lê và tìm Giày Nhảy Kép!");

        // Trạm Lửa Trại #1
        CreateCampfire(zone.transform, "Campfire_Plains", new Vector3(7f, -2.2f, 0), "Lửa Trại Khởi Nguyên");

        // Bậc nhảy rèn luyện
        CreateGroundPlatform(zone.transform, "Step_1", new Vector3(11f, -1.5f, 0), new Vector2(3.5f, 0.6f));
        CreateGroundPlatform(zone.transform, "Step_2", new Vector3(16f, 0f, 0), new Vector2(3.5f, 0.6f));

        // Quái tuần tra cơ bản
        CreateEnemyPatrol(zone.transform, "Slime_Patrol_1", new Vector3(14f, -2.4f, 0), new Vector3(20f, -2.4f, 0), 2f);

        // Bẫy chông nhẹ
        CreateSpikes(zone.transform, "Spike_Plains", new Vector3(22.5f, -2.55f, 0), 1f);

        // Cụm Coin
        SpawnCoinRing(zone.transform, new Vector3(16f, 1.2f, 0), 4);
        SpawnCoinLine(zone.transform, new Vector3(3f, -2.2f, 0), 3, 1f);

        // Zone trigger chuyển sang Hang Pha Lê Ngầm
        CreateZoneTrigger(zone.transform, "Trigger_Cavern", new Vector3(28f, -3f, 0), new Vector2(2f, 10f),
            "🪨 HANG PHA LÊ NGẦM (Crystal Cavern)");
    }

    // ========================================================================
    // PHÂN KHU 2: HANG PHA LÊ NGẦM (X: 28 -> 72, Y: -18 -> -3)
    // ========================================================================
    private static void BuildZone2_Cavern(Transform parent)
    {
        GameObject zone = new GameObject("Zone2_CrystalCavern");
        zone.transform.SetParent(parent);

        // Giếng trời thả sâu: người chơi rơi từ Y: -3 xuống Y: -14
        CreateWall(zone.transform, "Shaft_LeftWall", new Vector3(28.5f, -8f, 0), new Vector2(1f, 10f));
        CreateWall(zone.transform, "Shaft_RightWall", new Vector3(32.5f, -6f, 0), new Vector2(1f, 6f));

        // Nền hang sâu
        CreateGroundPlatform(zone.transform, "Cavern_Floor_1", new Vector3(38f, -14.5f, 0), new Vector2(16f, 1f), new Color(0.35f, 0.45f, 0.55f));
        CreateGroundPlatform(zone.transform, "Cavern_Floor_2", new Vector3(64f, -14.5f, 0), new Vector2(16f, 1f), new Color(0.35f, 0.45f, 0.55f));

        // Trần hang động ngầm
        CreateGroundPlatform(zone.transform, "Cavern_Ceiling", new Vector3(50f, -6f, 0), new Vector2(36f, 1f), new Color(0.25f, 0.3f, 0.4f));

        // Hố sâu chông nhọn giữa 2 nền hang
        CreateSpikes(zone.transform, "Cavern_Abyss_Spikes", new Vector3(51f, -16f, 0), 10f);

        // Bệ di động bắc qua hố sâu
        CreateMovingPlatform(zone.transform, "Cavern_Mover", new Vector3(47f, -12.5f, 0), new Vector3(55f, -12.5f, 0), 2.2f);

        // Bệ chứa GIÀY NHẢY KÉP (DOUBLE JUMP BOOTS)
        CreateGroundPlatform(zone.transform, "Relic_Altar_DJ", new Vector3(45f, -10f, 0), new Vector2(3.5f, 0.6f), new Color(0.8f, 0.7f, 0.2f));
        CreateAbilityRelic(zone.transform, "Relic_DoubleJump", new Vector3(45f, -9f, 0), AbilityRelicItem.AbilityType.DoubleJump);

        // Trạm Lửa Trại #2
        CreateCampfire(zone.transform, "Campfire_Cavern", new Vector3(58f, -13.5f, 0), "Lửa Trại Hang Pha Lê");

        // Quái đuổi theo trong hang sâu
        CreateEnemyPatrol(zone.transform, "Cavern_Stalker", new Vector3(60f, -13.7f, 0), new Vector3(68f, -13.7f, 0), 3f);

        // NGỌC LỤC BẢO HANG SÂU (EMERALD)
        CreateGroundPlatform(zone.transform, "Emerald_Pillar", new Vector3(70f, -11f, 0), new Vector2(3f, 0.6f), new Color(0.2f, 0.8f, 0.3f));
        CreateAncientGem(zone.transform, "Gem_Emerald", new Vector3(70f, -10f, 0), MetroidvaniaQuestManager.GemType.Emerald);

        // Bia Đá Cổ #2
        CreateLoreTotem(zone.transform, "Totem_Cavern", new Vector3(60f, -13.5f, 0),
            "Dùng Nhảy Kép để bật tường qua ống khói đá phía trước, vươn lên Thành Cổ Tàn Tích!");

        // Ống khói đá leo ngược lên mặt đất (Wall Jump Chimney)
        CreateWall(zone.transform, "Chimney_Wall_L", new Vector3(71.5f, -4f, 0), new Vector2(0.8f, 13f));
        CreateWall(zone.transform, "Chimney_Wall_R", new Vector3(74.5f, -4f, 0), new Vector2(0.8f, 13f));

        // Zone trigger chuyển sang Thành Cổ
        CreateZoneTrigger(zone.transform, "Trigger_Citadel", new Vector3(76f, 1f, 0), new Vector2(3f, 6f),
            "🏰 THÀNH CỔ TÀN TÍCH (Citadel of Eternity)");
    }

    // ========================================================================
    // PHÂN KHU 3: THÀNH CỔ TÀN TÍCH (X: 74 -> 110, Y: 0 -> 12)
    // ========================================================================
    private static void BuildZone3_Citadel(Transform parent)
    {
        GameObject zone = new GameObject("Zone3_Citadel");
        zone.transform.SetParent(parent);

        // Mặt sân thành cổ
        CreateGroundPlatform(zone.transform, "Citadel_Ground_1", new Vector3(85f, 0.5f, 0), new Vector2(22f, 1f), new Color(0.6f, 0.5f, 0.45f));

        // Tháp canh tầng 1
        CreateGroundPlatform(zone.transform, "Citadel_Tower_Fl1", new Vector3(88f, 4f, 0), new Vector2(6f, 0.8f), new Color(0.5f, 0.45f, 0.4f));

        // Bệ chứa CỔ TỰ LƯỚT GIÓ (AIR DASH RUNE)
        CreateGroundPlatform(zone.transform, "Relic_Altar_Dash", new Vector3(83f, 6.5f, 0), new Vector2(3.5f, 0.6f), new Color(0.3f, 0.6f, 0.9f));
        CreateAbilityRelic(zone.transform, "Relic_AirDash", new Vector3(83f, 7.5f, 0), AbilityRelicItem.AbilityType.AirDash);

        // Trạm Lửa Trại #3
        CreateCampfire(zone.transform, "Campfire_Citadel", new Vector3(89f, 4.8f, 0), "Lửa Trại Thành Cổ");

        // Quái lính gác thành truy đuổi
        CreateEnemyPatrol(zone.transform, "Citadel_Guard", new Vector3(82f, 1.3f, 0), new Vector3(92f, 1.3f, 0), 3.2f);

        // Bẫy chông thành cổ
        CreateSpikes(zone.transform, "Spikes_Citadel", new Vector3(94f, 1.15f, 0), 2f);

        // Tháp canh cao tầng 2 & NGỌC HỒNG BẢO (RUBY)
        CreateGroundPlatform(zone.transform, "Citadel_Tower_Fl2", new Vector3(98f, 7.5f, 0), new Vector2(5f, 0.8f));
        CreateGroundPlatform(zone.transform, "Ruby_Altar", new Vector3(104f, 9.5f, 0), new Vector2(3.5f, 0.6f), new Color(0.9f, 0.2f, 0.2f));
        CreateAncientGem(zone.transform, "Gem_Ruby", new Vector3(104f, 10.5f, 0), MetroidvaniaQuestManager.GemType.Ruby);

        // Bia Đá Cổ #3
        CreateLoreTotem(zone.transform, "Totem_Citadel", new Vector3(98f, 8.3f, 0),
            "Hãy kích hoạt [LEFT SHIFT] Lướt Gió kết hợp Nhảy Kép để bay qua các hòn đảo lơ lửng trên Đỉnh Tháp Mây!");

        // Zone trigger chuyển sang Đỉnh Tháp Mây
        CreateZoneTrigger(zone.transform, "Trigger_Peak", new Vector3(107f, 11f, 0), new Vector2(3f, 8f),
            "☁️ ĐỈNH THÁP MÂY & CỔNG PHONG ẤN (Celestial Peak)");
    }

    // ========================================================================
    // PHÂN KHU 4: ĐỈNH THÁP MÂY & CỔNG PHONG ẤN (X: 108 -> 145, Y: 10 -> 22)
    // ========================================================================
    private static void BuildZone4_Peak(Transform parent)
    {
        GameObject zone = new GameObject("Zone4_CelestialPeak");
        zone.transform.SetParent(parent);

        // Chuỗi đảo bay lơ lửng đòi hỏi Dash + Double Jump
        CreateGroundPlatform(zone.transform, "Sky_Island_1", new Vector3(112f, 11f, 0), new Vector2(3.5f, 0.8f), new Color(0.85f, 0.9f, 1f));
        CreateGroundPlatform(zone.transform, "Sky_Island_2", new Vector3(118f, 12.5f, 0), new Vector2(3f, 0.8f), new Color(0.85f, 0.9f, 1f));
        CreateGroundPlatform(zone.transform, "Sky_Island_3", new Vector3(124f, 14f, 0), new Vector2(3.5f, 0.8f), new Color(0.85f, 0.9f, 1f));

        // Đảo chứa NGỌC LAM BẢO ĐỈNH TRỜI (SAPPHIRE)
        CreateGroundPlatform(zone.transform, "Sapphire_Altar", new Vector3(127f, 16.5f, 0), new Vector2(3.5f, 0.6f), new Color(0.2f, 0.5f, 1f));
        CreateAncientGem(zone.transform, "Gem_Sapphire", new Vector3(127f, 17.5f, 0), MetroidvaniaQuestManager.GemType.Sapphire);

        // Đảo chuyển tiếp
        CreateGroundPlatform(zone.transform, "Sky_Island_4", new Vector3(133f, 15f, 0), new Vector2(3.5f, 0.8f), new Color(0.85f, 0.9f, 1f));

        // Trạm Lửa Trại #4
        CreateGroundPlatform(zone.transform, "Temple_Plaza", new Vector3(142f, 15f, 0), new Vector2(14f, 1.2f), new Color(0.9f, 0.85f, 0.7f));
        CreateCampfire(zone.transform, "Campfire_Peak", new Vector3(137f, 16f, 0), "Lửa Trại Đỉnh Tháp Mây");

        // CỔNG PHONG ẤN CỔ ĐẠI (THE ANCIENT GATE)
        CreateAncientGate(zone.transform, new Vector3(144f, 16.8f, 0));

        // Bia đá cổng thần bí
        CreateLoreTotem(zone.transform, "Totem_Gate", new Vector3(140f, 16f, 0),
            "Cổng Phong Ấn Cổ Đại dẫn tới vinh quang vĩnh hằng! Cần khảm đủ 3 Viên Ngọc: Emerald, Ruby và Sapphire.");
    }

    // ========================================================================
    // HELPER FUNCTIONS (TẠO CÁC OBJECT CHUẨN MỰC)
    // ========================================================================
    private static GameObject CreateGroundPlatform(Transform parent, string name, Vector3 pos, Vector2 size, Color? tint = null)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = pos;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        if (groundSprite != null) sr.sprite = groundSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = size;
        sr.sortingLayerName = "Default";
        sr.material = unlitMaterial;
        if (tint.HasValue) sr.color = tint.Value;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = size;

        go.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;
        return go;
    }

    private static GameObject CreateWall(Transform parent, string name, Vector3 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = pos;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        if (wallSprite != null) sr.sprite = wallSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = size;
        sr.sortingLayerName = "Default";
        sr.material = unlitMaterial;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = size;

        go.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;
        return go;
    }

    private static void CreateSpikes(Transform parent, string name, Vector3 pos, float width)
    {
        GameObject trap = new GameObject(name);
        trap.transform.SetParent(parent);
        trap.transform.position = pos;

        SpriteRenderer sr = trap.AddComponent<SpriteRenderer>();
        if (trapSprite != null) sr.sprite = trapSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(width, 0.4f);
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;

        BoxCollider2D col = trap.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(width, 0.35f);

        trap.AddComponent<Hazard>();
    }

    private static void CreateMovingPlatform(Transform parent, string name, Vector3 startPos, Vector3 endPos, float speed)
    {
        GameObject group = new GameObject(name + "_Group");
        group.transform.SetParent(parent);

        GameObject pA = new GameObject("PointA");
        pA.transform.SetParent(group.transform);
        pA.transform.position = startPos;

        GameObject pB = new GameObject("PointB");
        pB.transform.SetParent(group.transform);
        pB.transform.position = endPos;

        GameObject mover = new GameObject(name);
        mover.transform.SetParent(group.transform);
        mover.transform.position = startPos;

        SpriteRenderer sr = mover.AddComponent<SpriteRenderer>();
        if (movingPlatformSprite != null) sr.sprite = movingPlatformSprite;
        else if (groundSprite != null) sr.sprite = groundSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(3.5f, 0.6f);
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;

        BoxCollider2D col = mover.AddComponent<BoxCollider2D>();
        col.size = new Vector2(3.5f, 0.6f);
        mover.layer = LayerMask.NameToLayer("Ground") != -1 ? LayerMask.NameToLayer("Ground") : 0;

        MovingPlatform mp = mover.AddComponent<MovingPlatform>();
        mp.pointA = pA.transform;
        mp.pointB = pB.transform;
        mp.speed = speed;
    }

    private static void CreateEnemyPatrol(Transform parent, string name, Vector3 posA, Vector3 posB, float speed)
    {
        GameObject group = new GameObject(name + "_Group");
        group.transform.SetParent(parent);

        GameObject pA = new GameObject("PointA");
        pA.transform.SetParent(group.transform);
        pA.transform.position = posA;

        GameObject pB = new GameObject("PointB");
        pB.transform.SetParent(group.transform);
        pB.transform.position = posB;

        GameObject enemy = new GameObject(name);
        enemy.transform.SetParent(group.transform);
        enemy.transform.position = posA;

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        if (enemySprite != null) sr.sprite = enemySprite;
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;

        CircleCollider2D col = enemy.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.28f;

        EnemyPatrol patrol = enemy.AddComponent<EnemyPatrol>();
        patrol.pointA = pA.transform;
        patrol.pointB = pB.transform;
        patrol.speed = speed;
        patrol.enableChase = true;
        patrol.detectionRange = 5.5f;
    }

    private static void CreateCampfire(Transform parent, string name, Vector3 pos, string label)
    {
        GameObject cf = new GameObject(name);
        cf.transform.SetParent(parent);
        cf.transform.position = pos;

        SpriteRenderer sr = cf.AddComponent<SpriteRenderer>();
        if (coinSprite != null) sr.sprite = coinSprite; // Biểu tượng ngọn lửa vàng
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;
        sr.color = new Color(1f, 0.6f, 0.1f);
        cf.transform.localScale = new Vector3(1.4f, 1.4f, 1f);

        CircleCollider2D col = cf.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.6f;

        CampfireWaypoint cp = cf.AddComponent<CampfireWaypoint>();
        cp.campfireName = label;
    }

    private static void CreateLoreTotem(Transform parent, string name, Vector3 pos, string message)
    {
        GameObject totem = new GameObject(name);
        totem.transform.SetParent(parent);
        totem.transform.position = pos;

        SpriteRenderer sr = totem.AddComponent<SpriteRenderer>();
        if (wallSprite != null) sr.sprite = wallSprite;
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;
        sr.color = new Color(0.7f, 0.85f, 0.95f);
        totem.transform.localScale = new Vector3(0.5f, 1.2f, 1f);

        BoxCollider2D col = totem.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.8f, 1.8f);

        LoreTotem lt = totem.AddComponent<LoreTotem>();
        lt.loreMessage = message;
    }

    private static void CreateAncientGem(Transform parent, string name, Vector3 pos, MetroidvaniaQuestManager.GemType gemType)
    {
        GameObject gem = new GameObject(name);
        gem.transform.SetParent(parent);
        gem.transform.position = pos;

        SpriteRenderer sr = gem.AddComponent<SpriteRenderer>();
        if (keySprite != null) sr.sprite = keySprite;
        else if (coinSprite != null) sr.sprite = coinSprite;
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;
        gem.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        CircleCollider2D col = gem.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.35f;

        AncientGemItem gemItem = gem.AddComponent<AncientGemItem>();
        gemItem.gemType = gemType;
    }

    private static void CreateAbilityRelic(Transform parent, string name, Vector3 pos, AbilityRelicItem.AbilityType ability)
    {
        GameObject relic = new GameObject(name);
        relic.transform.SetParent(parent);
        relic.transform.position = pos;

        SpriteRenderer sr = relic.AddComponent<SpriteRenderer>();
        if (keySprite != null) sr.sprite = keySprite;
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;
        relic.transform.localScale = new Vector3(1.6f, 1.6f, 1f);

        CircleCollider2D col = relic.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;

        AbilityRelicItem relicItem = relic.AddComponent<AbilityRelicItem>();
        relicItem.abilityType = ability;
    }

    private static void CreateAncientGate(Transform parent, Vector3 pos)
    {
        GameObject gate = new GameObject("The_Ancient_Gate");
        gate.transform.SetParent(parent);
        gate.transform.position = pos;

        // Cột trái
        CreateWall(gate.transform, "Pillar_Left", pos + new Vector3(-1.2f, 0.5f, 0), new Vector2(0.8f, 3.5f));
        // Cột phải
        CreateWall(gate.transform, "Pillar_Right", pos + new Vector3(1.2f, 0.5f, 0), new Vector2(0.8f, 3.5f));
        // Mái vòm
        CreateWall(gate.transform, "Arch_Top", pos + new Vector3(0f, 2.5f, 0), new Vector2(3.2f, 0.8f));

        // Lõi cổng ánh sáng
        GameObject portalCore = new GameObject("Portal_Core");
        portalCore.transform.SetParent(gate.transform);
        portalCore.transform.position = pos + new Vector3(0, 0.5f, 0);

        SpriteRenderer sr = portalCore.AddComponent<SpriteRenderer>();
        if (wallSprite != null) sr.sprite = wallSprite;
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;
        sr.color = new Color(0.2f, 0.8f, 1f, 0.6f);
        portalCore.transform.localScale = new Vector3(1.6f, 3f, 1f);

        BoxCollider2D col = portalCore.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.5f, 2.8f);

        portalCore.AddComponent<AncientGate>();
    }

    private static void CreateZoneTrigger(Transform parent, string name, Vector3 pos, Vector2 size, string zoneName)
    {
        GameObject triggerGo = new GameObject(name);
        triggerGo.transform.SetParent(parent);
        triggerGo.transform.position = pos;

        BoxCollider2D col = triggerGo.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = size;

        ZoneTrigger zt = triggerGo.AddComponent<ZoneTrigger>();
        zt.zoneName = zoneName;
    }

    private static void SpawnCoinRing(Transform parent, Vector3 center, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * 1.2f, Mathf.Sin(angle) * 1.2f, 0);
            SpawnCoin(parent, center + offset);
        }
    }

    private static void SpawnCoinLine(Transform parent, Vector3 start, int count, float spacing)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnCoin(parent, start + Vector3.right * (i * spacing));
        }
    }

    private static void SpawnCoin(Transform parent, Vector3 pos)
    {
        GameObject coin = new GameObject("Coin");
        coin.transform.SetParent(parent);
        coin.transform.position = pos;

        SpriteRenderer sr = coin.AddComponent<SpriteRenderer>();
        if (coinSprite != null) sr.sprite = coinSprite;
        sr.sortingLayerName = "Player";
        sr.material = unlitMaterial;

        CircleCollider2D col = coin.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.16f;

        coin.AddComponent<Coin>();
    }
}
#endif
