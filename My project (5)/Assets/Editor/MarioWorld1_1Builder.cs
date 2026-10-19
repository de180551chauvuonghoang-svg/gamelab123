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
    private static Material unlitMat;

    [MenuItem("Lab 2/🍄 Xây Dựng Màn Chơi Super Mario Bros (World 1-1 Hoàn Chỉnh)", false, 1)]
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

    public static void BuildWorld1_1()
    {
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

        // 2. Chuyển Camera sang màu trời xanh Mario kinh điển
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

        // 5. Xây dựng địa hình
        BuildGroundAndPits(worldRoot.transform);

        // 6. Xây dựng Khối gạch & Khối chấm hỏi
        BuildBlocksAndItems(worldRoot.transform);

        // 7. Xây dựng các ống nước & hầm bí mật
        BuildPipesAndUnderground(worldRoot.transform);

        // 8. Bố trí kẻ địch Goomba & Koopa
        BuildEnemies(worldRoot.transform);

        // 9. Xây dựng Bậc thang, Cột cờ & Lâu đài chiến thắng
        BuildFlagpoleAndCastle(worldRoot.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("<color=#55FF55><b>[Super Mario] Màn chơi World 1-1 hoàn chỉnh đã sẵn sàng!</b></color>");
    }

    private static void LoadSprites()
    {
        questionBlockSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Block_Question.png");
        brickBlockSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Block_Brick.png");
        emptyBlockSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Block_Empty.png");
        goombaSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Enemy_Goomba.png");
        koopaSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Enemy_Koopa.png");
        koopaShellSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Enemy_KoopaShell.png");
        pipeTopSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Pipe_Top.png");
        pipeBodySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Pipe_Body.png");
        flagpoleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Flagpole_Flag.png");
        castleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Castle.png");
        coinSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Coin.png");
        groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Ground.png");

        Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit")
                          ?? Shader.Find("Sprites/Default");
        if (unlitShader != null) unlitMat = new Material(unlitShader);
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
