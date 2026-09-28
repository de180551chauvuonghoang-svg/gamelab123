#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class Lab1Automator
{
    private const string SETUP_FLAG_KEY = "Lab1_Setup_Completed_V1";

    static Lab1Automator()
    {
        EditorApplication.delayCall += OnEditorLoaded;
    }

    private static void OnEditorLoaded()
    {
        if (!SessionState.GetBool(SETUP_FLAG_KEY, false))
        {
            SetupAllLab1();
            SessionState.SetBool(SETUP_FLAG_KEY, true);
        }
    }

    [MenuItem("Lab 1/Chạy Cài Đặt Tự Động (One-Click Setup)", false, 1)]
    public static void RunSetupMenu()
    {
        SetupAllLab1();
        EditorUtility.DisplayDialog("Lab 1 Setup", "Đã thiết lập toàn bộ Lab 1 thành công!\n\nBây giờ bạn chỉ cần nhấn nút Play (▶) để bắt đầu kiểm tra checklist.", "OK");
    }

    public static void SetupAllLab1()
    {
        Debug.Log("<color=cyan><b>[Lab 1]</b> Bắt đầu tự động thiết lập toàn bộ nội dung Lab 1...</color>");

        // 1. Cấu hình Sprite Import cho các ảnh đã tạo
        ConfigureSprites();

        // 2. Cấu hình TagManager (Layer 'Ground' và Sorting Layer 'Player')
        SetupLayersAndSortingLayers();

        // 3. Tạo Physics Material 2D (ZeroFriction)
        PhysicsMaterial2D zeroFriction = SetupPhysicsMaterial();

        // 4. Tạo Animations & Animator Controller
        AnimatorController animController = SetupAnimator();

        // 5. Thiết lập các đối tượng trong Scene (Ground, Wall, Player, GroundCheck)
        SetupSceneObjects(zeroFriction, animController);

        AssetDatabase.SaveAssets();
        Debug.Log("<color=green><b>[Lab 1]</b> HOÀN TẤT SETUP! Bạn đã có thể nhấn PLAY để test checklist.</color>");
    }

    private static void ConfigureSprites()
    {
        string[] spritePaths = new string[]
        {
            "Assets/Sprites/Player.png",
            "Assets/Sprites/Ground.png",
            "Assets/Sprites/Wall.png"
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

    private static void SetupLayersAndSortingLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Cài đặt Layer 8: "Ground"
        SerializedProperty layers = tagManager.FindProperty("layers");
        if (layers != null && layers.arraySize > 8)
        {
            SerializedProperty layer8 = layers.GetArrayElementAtIndex(8);
            if (layer8.stringValue != "Ground")
            {
                layer8.stringValue = "Ground";
            }
        }

        // Cài đặt Sorting Layer: "Player"
        SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");
        if (sortingLayers != null)
        {
            bool hasPlayerSorting = false;
            for (int i = 0; i < sortingLayers.arraySize; i++)
            {
                if (sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == "Player")
                {
                    hasPlayerSorting = true;
                    break;
                }
            }

            if (!hasPlayerSorting)
            {
                int newIndex = sortingLayers.arraySize;
                sortingLayers.InsertArrayElementAtIndex(newIndex);
                SerializedProperty newLayer = sortingLayers.GetArrayElementAtIndex(newIndex);
                newLayer.FindPropertyRelative("name").stringValue = "Player";
                newLayer.FindPropertyRelative("uniqueID").intValue = 10001 + newIndex;
            }
        }

        tagManager.ApplyModifiedProperties();
    }

    private static PhysicsMaterial2D SetupPhysicsMaterial()
    {
        string matPath = "Assets/PhysicsMaterials/ZeroFriction.physicsMaterial2D";
        PhysicsMaterial2D mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(matPath);
        if (mat == null)
        {
            mat = new PhysicsMaterial2D("ZeroFriction")
            {
                friction = 0f,
                bounciness = 0f
            };
            AssetDatabase.CreateAsset(mat, matPath);
        }
        else
        {
            mat.friction = 0f;
            mat.bounciness = 0f;
            EditorUtility.SetDirty(mat);
        }
        return mat;
    }

    private static AnimatorController SetupAnimator()
    {
        string controllerPath = "Assets/Animations/PlayerAnimatorController.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            // Thêm parameters
            controller.AddParameter("isRunning", AnimatorControllerParameterType.Bool);
            controller.AddParameter("isJumping", AnimatorControllerParameterType.Bool);

            // Tạo Animation Clips
            AnimationClip idleClip = GetOrCreateClip("Assets/Animations/Player_Idle.anim");
            AnimationClip runClip = GetOrCreateClip("Assets/Animations/Player_Run.anim");
            AnimationClip jumpClip = GetOrCreateClip("Assets/Animations/Player_Jump.anim");

            var rootStateMachine = controller.layers[0].stateMachine;

            // States
            var idleState = rootStateMachine.AddState("Player_Idle");
            idleState.motion = idleClip;
            rootStateMachine.defaultState = idleState;

            var runState = rootStateMachine.AddState("Player_Run");
            runState.motion = runClip;

            var jumpState = rootStateMachine.AddState("Player_Jump");
            jumpState.motion = jumpClip;

            // Transition: Idle -> Run
            var tIdleToRun = idleState.AddTransition(runState);
            tIdleToRun.hasExitTime = false;
            tIdleToRun.duration = 0f;
            tIdleToRun.AddCondition(AnimatorConditionMode.If, 0, "isRunning");

            // Transition: Run -> Idle
            var tRunToIdle = runState.AddTransition(idleState);
            tRunToIdle.hasExitTime = false;
            tRunToIdle.duration = 0f;
            tRunToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isRunning");

            // Transition: Any State -> Jump
            var tAnyToJump = rootStateMachine.AddAnyStateTransition(jumpState);
            tAnyToJump.hasExitTime = false;
            tAnyToJump.duration = 0f;
            tAnyToJump.AddCondition(AnimatorConditionMode.If, 0, "isJumping");

            // Transition: Jump -> Idle
            var tJumpToIdle = jumpState.AddTransition(idleState);
            tJumpToIdle.hasExitTime = false;
            tJumpToIdle.duration = 0f;
            tJumpToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isJumping");
        }
        return controller;
    }

    private static AnimationClip GetOrCreateClip(string path)
    {
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, path);
        }
        return clip;
    }

    private static void SetupSceneObjects(PhysicsMaterial2D zeroFriction, AnimatorController animController)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.isLoaded || string.IsNullOrEmpty(activeScene.path))
        {
            if (System.IO.File.Exists("Assets/Scenes/SampleScene.unity"))
            {
                activeScene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            }
        }
        if (!activeScene.isLoaded) return;

        Sprite groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Ground.png");
        Sprite wallSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Wall.png");
        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Player.png");

        int groundLayerIndex = LayerMask.NameToLayer("Ground");
        if (groundLayerIndex == -1) groundLayerIndex = 8;

        // 1. Setup Ground
        GameObject ground = GameObject.Find("Ground");
        if (ground == null)
        {
            ground = new GameObject("Ground");
        }
        ground.layer = groundLayerIndex;
        ground.transform.position = new Vector3(0, -3.5f, 0);
        ground.transform.localScale = new Vector3(18, 1.2f, 1);

        SpriteRenderer groundSr = ground.GetComponent<SpriteRenderer>();
        if (groundSr == null) groundSr = ground.AddComponent<SpriteRenderer>();
        if (groundSprite != null) groundSr.sprite = groundSprite;
        groundSr.drawMode = SpriteDrawMode.Tiled;
        groundSr.size = new Vector2(18, 1.2f);
        ground.transform.localScale = Vector3.one;

        BoxCollider2D groundCol = ground.GetComponent<BoxCollider2D>();
        if (groundCol == null) groundCol = ground.AddComponent<BoxCollider2D>();
        groundCol.size = new Vector2(18, 1.2f);

        // 2. Setup Wall (cho phép test tính năng trượt tường không ma sát và nhảy vượt chướng ngại vật)
        GameObject wall = GameObject.Find("Wall_Test");
        if (wall == null)
        {
            wall = new GameObject("Wall_Test");
        }
        wall.layer = groundLayerIndex;
        wall.transform.position = new Vector3(7f, -1.9f, 0);
        wall.transform.localScale = Vector3.one;

        SpriteRenderer wallSr = wall.GetComponent<SpriteRenderer>();
        if (wallSr == null) wallSr = wall.AddComponent<SpriteRenderer>();
        if (wallSprite != null) wallSr.sprite = wallSprite;
        wallSr.drawMode = SpriteDrawMode.Tiled;
        wallSr.size = new Vector2(1.2f, 2.5f);

        BoxCollider2D wallCol = wall.GetComponent<BoxCollider2D>();
        if (wallCol == null) wallCol = wall.AddComponent<BoxCollider2D>();
        wallCol.size = new Vector2(1.2f, 2.5f);

        // 3. Setup Player
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = new GameObject("Player");
        }
        player.transform.position = new Vector3(0, 0, 0);
        player.transform.localScale = Vector3.one;

        // SpriteRenderer
        SpriteRenderer playerSr = player.GetComponent<SpriteRenderer>();
        if (playerSr == null) playerSr = player.AddComponent<SpriteRenderer>();
        if (playerSprite != null) playerSr.sprite = playerSprite;
        playerSr.sortingLayerName = "Player";

        // Rigidbody2D
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null) rb = player.AddComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // BoxCollider2D
        BoxCollider2D playerCol = player.GetComponent<BoxCollider2D>();
        if (playerCol == null) playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.size = new Vector2(0.55f, 0.62f);
        playerCol.offset = new Vector2(0, 0f);
        playerCol.sharedMaterial = zeroFriction;

        // Animator
        Animator anim = player.GetComponent<Animator>();
        if (anim == null) anim = player.AddComponent<Animator>();
        anim.runtimeAnimatorController = animController;

        // GroundCheck child
        Transform groundCheck = player.transform.Find("GroundCheck");
        if (groundCheck == null)
        {
            GameObject gcGo = new GameObject("GroundCheck");
            gcGo.transform.SetParent(player.transform);
            groundCheck = gcGo.transform;
        }
        groundCheck.localPosition = new Vector3(0, -0.32f, 0);

        // PlayerController script
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc == null) pc = player.AddComponent<PlayerController>();
        pc.groundCheck = groundCheck;
        pc.checkRadius = 0.15f;
        pc.groundLayer = 1 << groundLayerIndex;
        pc.moveSpeed = 7f;
        pc.jumpForce = 7.5f;

        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);
    }
}
#endif
