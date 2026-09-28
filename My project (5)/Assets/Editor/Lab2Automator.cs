#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[InitializeOnLoad]
public class Lab2Automator
{
    static Lab2Automator()
    {
        EditorApplication.delayCall += () => {
            EnsureGroundExtended();
            FixAllBlackSprites();
        };
    }

    [MenuItem("Lab 2/Sửa Lỗi Sàn Đất (Fix Ground Length & Texture)", false, 3)]
    public static void FixGroundMenu()
    {
        EnsureGroundExtended();
        EditorUtility.DisplayDialog("Thành công", "Đã kéo dài sàn đất 36 mét qua khỏi cột tường và sửa lỗi lặp vân cỏ thành công!\n\nBây giờ toàn bộ bản đồ đã có sàn phẳng liền mạch.", "OK");
    }

    [MenuItem("Lab 2/Sửa Lỗi Vật Thể Bị Đen (Fix All Black Sprites)", false, 2)]
    public static void FixLightingMenu()
    {
        FixAllBlackSprites();
        EditorUtility.DisplayDialog("Thành công", "Đã bật ánh sáng cho toàn bộ các layer và chuyển tất cả Sprite sang Unlit!\n\nBây giờ toàn bộ Player, Coin, Bẫy gai, Quái vật, Chìa khóa đều sáng đẹp rực rỡ ngay cả khi chưa bấm Play.", "OK");
    }

    [MenuItem("Lab 2/Chạy Cài Đặt Tự Động Lab 2 (One-Click Setup)", false, 1)]
    public static void RunLab2Setup()
    {
        SetupAllLab2();
        EditorUtility.DisplayDialog("Lab 2 Setup", "Đã thiết lập toàn bộ nội dung Lab 2 thành công!\n\nBao gồm:\n- Camera Follow theo dõi Player\n- Hệ thống Coin Prefab & Điểm số\n- Bẫy gai (Trap) & Quái vật tuần tra (Enemy Patrol A-B)\n- Chìa khóa chiến thắng (Key Item)\n- Giao diện UI (Score, Game Over Panel, Win Panel)\n\nNhấn nút Play (▶) để test ngay!", "Tuyệt vời");
    }

    public static void SetupAllLab2()
    {
        Debug.Log("<color=cyan><b>[Lab 2]</b> Bắt đầu tự động thiết lập toàn bộ nội dung Lab 2...</color>");

        // 1. Cấu hình Sprite Import cho các ảnh mới của Lab 2
        ConfigureLab2Sprites();

        // 2. Đảm bảo Player có Tag = 'Player'
        EnsurePlayerTag();

        // 3. Cài đặt Camera Follow
        SetupCameraFollow();

        // 4. Tạo thư mục Prefabs nếu chưa có
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // 5. Mở Scene hiện tại
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.isLoaded || string.IsNullOrEmpty(activeScene.path))
        {
            if (System.IO.File.Exists("Assets/Scenes/SampleScene.unity"))
            {
                activeScene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            }
        }
        if (!activeScene.isLoaded) return;

        // 6. Mở rộng sàn đất để đủ chỗ cho màn chơi Lab 2
        EnsureGroundExtended();

        // 7. Tạo cấu trúc Tilemap (Grid, Ground, Foreground, Background, Composite Collider 2D)
        SetupTilemapStructure();

        // 8. Tạo Coin Prefab & đặt các đồng Coin vào Scene
        SetupCoins(activeScene);

        // 9. Tạo Bẫy gai (Trap)
        SetupTrap(activeScene);

        // 10. Tạo Quái vật tuần tra (Enemy Patrol)
        SetupEnemyPatrol(activeScene);

        // 11. Tạo Chìa khóa chiến thắng (Key Item)
        SetupKeyItem(activeScene);

        // 12. Tạo Hệ thống UI (ScoreText, GameOverPanel, WinPanel) & GameManager
        SetupUIAndGameManager(activeScene);

        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);
        AssetDatabase.SaveAssets();

        Debug.Log("<color=green><b>[Lab 2]</b> HOÀN TẤT THIẾT LẬP LAB 2! Bạn có thể nhấn PLAY để test ngay.</color>");
    }

    private static void ConfigureLab2Sprites()
    {
        string[] spritePaths = new string[]
        {
            "Assets/Sprites/Coin.png",
            "Assets/Sprites/Enemy.png",
            "Assets/Sprites/Trap.png",
            "Assets/Sprites/Key.png"
        };

        foreach (string path in spritePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool changed = false;
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    changed = true;
                }
                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    changed = true;
                }
                if (changed)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
        }
    }

    private static void EnsurePlayerTag()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.tag = "Player";
        }
    }

    private static void SetupCameraFollow()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraFollow follow = mainCam.GetComponent<CameraFollow>();
            if (follow == null)
            {
                follow = mainCam.gameObject.AddComponent<CameraFollow>();
            }

            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                follow.target = player.transform;
            }
            follow.smoothSpeed = 0.15f;
            follow.offset = new Vector3(0f, 1f, -10f);
        }
    }

    private static void SetupCoins(Scene scene)
    {
        Sprite coinSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Coin.png");
        string prefabPath = "Assets/Prefabs/Coin.prefab";

        // Tạo Coin Prefab
        GameObject coinObj = new GameObject("Coin_Prefab");
        SpriteRenderer sr = coinObj.AddComponent<SpriteRenderer>();
        if (coinSprite != null) sr.sprite = coinSprite;
        sr.sortingLayerName = "Player";

        Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        Material unlitMat = unlitShader != null ? new Material(unlitShader) : null;
        if (unlitMat != null) sr.material = unlitMat;

        CircleCollider2D col = coinObj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.16f;

        coinObj.AddComponent<Coin>();

        PrefabUtility.SaveAsPrefabAsset(coinObj, prefabPath);
        GameObject.DestroyImmediate(coinObj);

        // Sinh các đồng Coin trong Scene
        GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (coinPrefab != null)
        {
            // Xóa các coin cũ nếu có
            GameObject existingCoins = GameObject.Find("Coins_Container");
            if (existingCoins != null) GameObject.DestroyImmediate(existingCoins);

            GameObject container = new GameObject("Coins_Container");

            Vector3[] coinPositions = new Vector3[]
            {
                new Vector3(2.5f, -2.2f, 0),
                new Vector3(4.5f, -2.2f, 0),
                new Vector3(7.0f, 0.2f, 0),   // Trên đỉnh tường
                new Vector3(9.5f, -2.2f, 0),
                new Vector3(11.5f, -2.2f, 0)
            };

            for (int i = 0; i < coinPositions.Length; i++)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(coinPrefab);
                instance.name = "Coin_" + (i + 1);
                instance.transform.position = coinPositions[i];
                instance.transform.SetParent(container.transform);
            }
        }
    }

    private static void SetupTrap(Scene scene)
    {
        Sprite trapSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Trap.png");

        GameObject trap = GameObject.Find("Trap_Spikes");
        if (trap == null)
        {
            trap = new GameObject("Trap_Spikes");
        }

        trap.transform.position = new Vector3(3.5f, -2.65f, 0);
        trap.transform.localScale = Vector3.one;

        SpriteRenderer sr = trap.GetComponent<SpriteRenderer>();
        if (sr == null) sr = trap.AddComponent<SpriteRenderer>();
        if (trapSprite != null) sr.sprite = trapSprite;
        sr.sortingLayerName = "Player";
        Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        if (unlitShader != null) sr.material = new Material(unlitShader);

        BoxCollider2D col = trap.GetComponent<BoxCollider2D>();
        if (col == null) col = trap.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.4f, 0.35f);
        col.offset = new Vector2(0, 0f);

        Hazard hazard = trap.GetComponent<Hazard>();
        if (hazard == null) hazard = trap.AddComponent<Hazard>();
    }

    private static void SetupEnemyPatrol(Scene scene)
    {
        Sprite enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Enemy.png");

        // Tạo container và 2 điểm tuần tra A - B
        GameObject enemyGroup = GameObject.Find("Enemy_Patrol_Group");
        if (enemyGroup == null)
        {
            enemyGroup = new GameObject("Enemy_Patrol_Group");
        }

        Transform pointA = enemyGroup.transform.Find("PointA");
        if (pointA == null)
        {
            GameObject pAGo = new GameObject("PointA");
            pAGo.transform.SetParent(enemyGroup.transform);
            pointA = pAGo.transform;
        }
        pointA.position = new Vector3(10f, -2.65f, 0);

        Transform pointB = enemyGroup.transform.Find("PointB");
        if (pointB == null)
        {
            GameObject pBGo = new GameObject("PointB");
            pBGo.transform.SetParent(enemyGroup.transform);
            pointB = pBGo.transform;
        }
        pointB.position = new Vector3(14.5f, -2.65f, 0);

        // Tạo GameObject Enemy
        GameObject enemy = GameObject.Find("Enemy");
        if (enemy == null)
        {
            enemy = new GameObject("Enemy");
            enemy.transform.SetParent(enemyGroup.transform);
        }

        enemy.transform.position = pointA.position;
        enemy.transform.localScale = Vector3.one;

        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        if (sr == null) sr = enemy.AddComponent<SpriteRenderer>();
        if (enemySprite != null) sr.sprite = enemySprite;
        sr.sortingLayerName = "Player";
        Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        if (unlitShader != null) sr.material = new Material(unlitShader);

        CircleCollider2D col = enemy.GetComponent<CircleCollider2D>();
        if (col == null) col = enemy.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.25f;

        EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();
        if (patrol == null) patrol = enemy.AddComponent<EnemyPatrol>();
        patrol.pointA = pointA;
        patrol.pointB = pointB;
        patrol.speed = 2f;
    }

    private static void SetupKeyItem(Scene scene)
    {
        Sprite keySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Key.png");

        GameObject key = GameObject.Find("Key_WinItem");
        if (key == null)
        {
            key = new GameObject("Key_WinItem");
        }

        key.transform.position = new Vector3(16.5f, -2.3f, 0);
        key.transform.localScale = Vector3.one;

        SpriteRenderer sr = key.GetComponent<SpriteRenderer>();
        if (sr == null) sr = key.AddComponent<SpriteRenderer>();
        if (keySprite != null) sr.sprite = keySprite;
        sr.sortingLayerName = "Player";
        Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        if (unlitShader != null) sr.material = new Material(unlitShader);

        CircleCollider2D col = key.GetComponent<CircleCollider2D>();
        if (col == null) col = key.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.2f;

        KeyItem keyItem = key.GetComponent<KeyItem>();
        if (keyItem == null) keyItem = key.AddComponent<KeyItem>();
    }

    private static void SetupUIAndGameManager(Scene scene)
    {
        // 1. Tạo Canvas
        GameObject canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        // Tạo EventSystem nếu chưa có
        if (GameObject.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // 2. ScoreText
        Transform scoreTextTrans = canvasGo.transform.Find("ScoreText");
        TextMeshProUGUI scoreTmp;
        if (scoreTextTrans == null)
        {
            GameObject stGo = new GameObject("ScoreText");
            stGo.transform.SetParent(canvasGo.transform, false);
            scoreTmp = stGo.AddComponent<TextMeshProUGUI>();
            RectTransform rt = stGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(30, -30);
            rt.sizeDelta = new Vector2(300, 60);
        }
        else
        {
            scoreTmp = scoreTextTrans.GetComponent<TextMeshProUGUI>();
        }
        scoreTmp.text = "Score: 0";
        scoreTmp.fontSize = 36;
        scoreTmp.fontStyle = FontStyles.Bold;
        scoreTmp.color = Color.yellow;

        // 3. GameOverPanel
        GameObject gameOverPanel = CreateUIPanel(canvasGo.transform, "GameOverPanel", "GAME OVER", new Color(0.9f, 0.2f, 0.2f), "Thử Lại");
        // 4. WinPanel
        GameObject winPanel = CreateUIPanel(canvasGo.transform, "WinPanel", "YOU WIN!", new Color(1f, 0.85f, 0.1f), "Chơi Lại");

        // 5. Setup GameManager
        GameObject gmGo = GameObject.Find("GameManager");
        if (gmGo == null)
        {
            gmGo = new GameObject("GameManager");
        }
        GameManager gm = gmGo.GetComponent<GameManager>();
        if (gm == null) gm = gmGo.AddComponent<GameManager>();

        gm.scoreText = scoreTmp;
        gm.gameOverPanel = gameOverPanel;
        gm.winPanel = winPanel;

        // Ẩn panel khi ban đầu
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    private static GameObject CreateUIPanel(Transform parent, string panelName, string titleText, Color titleColor, string playAgainBtnText)
    {
        Transform existing = parent.Find(panelName);
        if (existing != null)
        {
            return existing.gameObject;
        }

        // Panel Container
        GameObject panel = new GameObject(panelName);
        panel.transform.SetParent(parent, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.85f);
        RectTransform panelRt = panel.GetComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = Vector2.zero;
        panelRt.offsetMax = Vector2.zero;

        // Title Text
        GameObject titleGo = new GameObject("Title");
        titleGo.transform.SetParent(panel.transform, false);
        TextMeshProUGUI titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = titleText;
        titleTmp.fontSize = 64;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = titleColor;
        RectTransform titleRt = titleGo.GetComponent<RectTransform>();
        titleRt.anchoredPosition = new Vector2(0, 100);
        titleRt.sizeDelta = new Vector2(600, 100);

        // Button Play Again
        CreateButton(panel.transform, "Btn_PlayAgain", playAgainBtnText, new Vector2(0, -20), UIButtonAction.ActionType.PlayAgain);

        // Button Main Menu
        CreateButton(panel.transform, "Btn_MainMenu", "Menu Chính", new Vector2(0, -100), UIButtonAction.ActionType.MainMenu);

        return panel;
    }

    private static void CreateButton(Transform parent, string btnName, string label, Vector2 pos, UIButtonAction.ActionType actionType)
    {
        GameObject btnGo = new GameObject(btnName);
        btnGo.transform.SetParent(parent, false);
        Image img = btnGo.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.9f);
        btnGo.AddComponent<Button>();

        UIButtonAction action = btnGo.AddComponent<UIButtonAction>();
        action.actionType = actionType;

        RectTransform rt = btnGo.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(240, 60);

        // Label
        GameObject labelGo = new GameObject("Text");
        labelGo.transform.SetParent(btnGo.transform, false);
        TextMeshProUGUI labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
        labelTmp.text = label;
        labelTmp.fontSize = 26;
        labelTmp.fontStyle = FontStyles.Bold;
        labelTmp.alignment = TextAlignmentOptions.Center;
        labelTmp.color = Color.white;
        RectTransform labelRt = labelGo.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;
    }

    private static void EnsureGroundExtended()
    {
        // 1. Cấu hình Texture Ground chuẩn 64 PPU và Repeat
        TextureImporter importer = AssetImporter.GetAtPath("Assets/Sprites/Ground.png") as TextureImporter;
        if (importer != null)
        {
            bool needReimport = false;
            if (importer.spritePixelsPerUnit != 64)
            {
                importer.spritePixelsPerUnit = 64;
                needReimport = true;
            }
            if (importer.wrapMode != TextureWrapMode.Repeat)
            {
                importer.wrapMode = TextureWrapMode.Repeat;
                needReimport = true;
            }
            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                needReimport = true;
            }
            if (needReimport)
            {
                AssetDatabase.ImportAsset("Assets/Sprites/Ground.png", ImportAssetOptions.ForceUpdate);
            }
        }

        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
            // Đặt sàn ở toạ độ X = 9 để bao phủ từ X = -9 đến X = +27 (sau cả Key Item ở 16.5)
            // Đặt Y = -3.4f với chiều cao 1m để mặt cỏ nằm ở Y = -2.9f, chuẩn xác tuyệt đối
            ground.transform.position = new Vector3(9f, -3.4f, 0);
            ground.transform.localScale = Vector3.one;

            SpriteRenderer sr = ground.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.drawMode = SpriteDrawMode.Tiled;
                sr.size = new Vector2(36f, 1f); // Dài 36 mét, cao đúng 1m (chỉ có 1 tầng cỏ trên mặt)
                sr.sortingLayerName = "Ground";
            }
            BoxCollider2D col = ground.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.size = new Vector2(36f, 1f);
                col.offset = Vector2.zero;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveScene(activeScene);
            }
        }
    }

    private static void SetupTilemapStructure()
    {
        GameObject gridGo = GameObject.Find("Grid");
        if (gridGo == null)
        {
            gridGo = new GameObject("Grid");
            gridGo.AddComponent<Grid>();
        }

        CreateTilemapLayer(gridGo.transform, "Background", "Default", 0, false);
        CreateTilemapLayer(gridGo.transform, "Ground_Tilemap", "Ground", 0, true);
        CreateTilemapLayer(gridGo.transform, "Foreground", "Default", 1, false);
    }

    private static void CreateTilemapLayer(Transform parent, string layerName, string sortingLayer, int order, bool hasCompositeCollider)
    {
        Transform existing = parent.Find(layerName);
        if (existing == null)
        {
            GameObject tmGo = new GameObject(layerName);
            tmGo.transform.SetParent(parent, false);
            tmGo.AddComponent<UnityEngine.Tilemaps.Tilemap>();
            var tmRenderer = tmGo.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();
            tmRenderer.sortingLayerName = sortingLayer;
            tmRenderer.sortingOrder = order;

            if (hasCompositeCollider)
            {
                int groundLayerIndex = LayerMask.NameToLayer("Ground");
                if (groundLayerIndex == -1) groundLayerIndex = 8;
                tmGo.layer = groundLayerIndex;

                var tmCol = tmGo.AddComponent<UnityEngine.Tilemaps.TilemapCollider2D>();
                tmCol.usedByComposite = true;
                var compCol = tmGo.AddComponent<CompositeCollider2D>();
                compCol.geometryType = CompositeCollider2D.GeometryType.Polygons;
                var rb = tmGo.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Static;
                }
            }
        }
    }

    public static void FixAllBlackSprites()
    {
        // 1. Tải Material Sprite-Unlit chính thức từ gói URP của Unity
        Material unlitMat = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat");
        if (unlitMat == null)
        {
            Shader s = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
            if (s != null) unlitMat = new Material(s);
        }

        if (unlitMat == null) return;

        // 2. Gán Material Unlit cho các vật thể SpriteRenderer trong Scene (trừ Ground và Wall_Test)
        SpriteRenderer[] renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in renderers)
        {
            if (sr.gameObject.name != "Ground" && sr.gameObject.name != "Wall_Test")
            {
                sr.sharedMaterial = unlitMat;
                EditorUtility.SetDirty(sr);
            }
        }

        // 3. Cập nhật Prefab Coin
        string coinPrefabPath = "Assets/Prefabs/Coin.prefab";
        GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(coinPrefabPath);
        if (coinPrefab != null)
        {
            SpriteRenderer cSr = coinPrefab.GetComponent<SpriteRenderer>();
            if (cSr != null)
            {
                cSr.sharedMaterial = unlitMat;
                EditorUtility.SetDirty(coinPrefab);
            }
        }

        // 4. Lưu Scene để thay đổi có hiệu lực vĩnh viễn
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
        }
    }
}
#endif
