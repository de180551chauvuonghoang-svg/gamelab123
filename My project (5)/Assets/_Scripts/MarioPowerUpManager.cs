using UnityEngine;
using System.Collections;

/// <summary>
/// Quản lý trạng thái biến hình và sức mạnh chuẩn Mario:
/// - Small Mario: 1 khối, ăn Nấm hóa Super Mario.
/// - Super Mario: 2 khối, có thể húc vỡ gạch, trúng đòn teo nhỏ lại Small Mario (kèm bất tử tạm thời).
/// - Fire Mario: Áo đổi màu, bắn được Cầu Lửa (Fireball) bằng phím F / X / Chuột Trái.
/// - Starman: Bất tử 10 giây, đổi màu cầu vồng, chạm vào bất kỳ quái nào cũng tiêu diệt ngay.
/// </summary>
public class MarioPowerUpManager : MonoBehaviour
{
    public static MarioPowerUpManager instance;

    public enum MarioForm { Small, Super, Fire }

    [Header("--- TRẠNG THÁI HIỆN TẠI ---")]
    public MarioForm currentForm = MarioForm.Small;
    public bool isStarman = false;

    [Header("--- THIẾT LẬP KÍCH THƯỚC COLLIDER ---")]
    public Vector2 smallColliderSize = new Vector2(0.55f, 0.62f);
    public Vector2 smallColliderOffset = new Vector2(0f, 0f);

    public Vector2 bigColliderSize = new Vector2(0.55f, 1.25f);
    public Vector2 bigColliderOffset = new Vector2(0f, 0.32f);

    [Header("--- BẮN CẦU LỬA (FIREBALL) ---")]
    public GameObject fireballPrefab;
    public float fireballCooldown = 0.3f;
    private float lastFireballTime = -10f;

    // Thành phần
    private SpriteRenderer sr;
    private BoxCollider2D boxCol;
    private PlayerController playerCtrl;
    private Coroutine starmanCoroutine;
    private Coroutine transformCoroutine;

    private void Awake()
    {
        instance = this;
        sr = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        playerCtrl = GetComponent<PlayerController>();
    }

    private void Start()
    {
        ApplyFormDimensions(currentForm, false);
    }

    private void Update()
    {
        // Phím bắn Cầu Lửa khi ở dạng Fire Mario
        if (currentForm == MarioForm.Fire)
        {
            bool firePressed = false;

#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (kb != null)
            {
                if (kb.fKey.wasPressedThisFrame || kb.xKey.wasPressedThisFrame || kb.jKey.wasPressedThisFrame)
                    firePressed = true;
            }
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                firePressed = true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
            {
                firePressed = true;
            }
#endif

            if (firePressed && Time.time - lastFireballTime >= fireballCooldown)
            {
                ShootFireball();
            }
        }
    }

    // ========================================================================
    // BIẾN HÌNH NẤM LỚN (SUPER MUSHROOM)
    // ========================================================================
    public void CollectMushroom()
    {
        if (GameManager.instance != null) GameManager.instance.AddScore(1000);

        if (currentForm == MarioForm.Small)
        {
            SetForm(MarioForm.Super);
        }
    }

    // ========================================================================
    // BIẾN HÌNH HOA LỬA (FIRE FLOWER)
    // ========================================================================
    public void CollectFireFlower()
    {
        if (GameManager.instance != null) GameManager.instance.AddScore(1000);
        SetForm(MarioForm.Fire);
    }

    // ========================================================================
    // SAO BẤT TỬ (STARMAN)
    // ========================================================================
    public void ActivateStarman(float duration = 10f)
    {
        if (GameManager.instance != null) GameManager.instance.AddScore(1000);

        if (starmanCoroutine != null) StopCoroutine(starmanCoroutine);
        starmanCoroutine = StartCoroutine(StarmanRoutine(duration));
    }

    private IEnumerator StarmanRoutine(float duration)
    {
        isStarman = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Hiệu ứng màu cầu vồng
            if (sr != null)
            {
                float hue = Mathf.Repeat(elapsed * 4f, 1f);
                sr.color = Color.HSVToRGB(hue, 0.8f, 1f);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isStarman = false;
        // Trả lại màu dạng hiện tại
        ResetCostumeColor();
        starmanCoroutine = null;
    }

    // ========================================================================
    // THAY ĐỔI FORM (SMALL -> SUPER -> FIRE)
    // ========================================================================
    public void SetForm(MarioForm targetForm)
    {
        if (transformCoroutine != null) StopCoroutine(transformCoroutine);
        transformCoroutine = StartCoroutine(TransformationRoutine(targetForm));
    }

    private IEnumerator TransformationRoutine(MarioForm targetForm)
    {
        // Hiệu ứng nhấp nháy lớn lên / thu nhỏ
        Time.timeScale = 0.1f; // Dừng hình chớp nhoáng kiểu Mario

        for (int i = 0; i < 6; i++)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSecondsRealtime(0.06f);
        }

        if (sr != null) sr.enabled = true;
        Time.timeScale = 1f;

        currentForm = targetForm;
        ApplyFormDimensions(currentForm, true);
        ResetCostumeColor();

        // Âm thanh / hạt
        if (ParticleSpawner.instance != null)
        {
            Color burstColor = targetForm == MarioForm.Fire ? Color.red : Color.yellow;
            ParticleSpawner.instance.SpawnDust(transform.position + Vector3.up * 0.5f, 20, burstColor);
        }
    }

    private void ApplyFormDimensions(MarioForm form, bool animate)
    {
        if (boxCol == null) boxCol = GetComponent<BoxCollider2D>();

        if (form == MarioForm.Small)
        {
            if (boxCol != null)
            {
                boxCol.size = smallColliderSize;
                boxCol.offset = smallColliderOffset;
            }
            transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * 1f, 1f, 1f);
        }
        else
        {
            // Super hoặc Fire Mario cao gấp 1.4 lần
            if (boxCol != null)
            {
                boxCol.size = bigColliderSize;
                boxCol.offset = bigColliderOffset;
            }
            transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * 1.35f, 1.35f, 1f);
        }
    }

    private void ResetCostumeColor()
    {
        if (sr == null) return;

        if (currentForm == MarioForm.Fire)
        {
            // Trang phục Mario Lửa: Áo trắng tinh, quần yếm đỏ
            sr.color = new Color(1f, 0.95f, 0.95f);
        }
        else
        {
            sr.color = Color.white;
        }
    }

    // ========================================================================
    // BẮN CẦU LỬA (SHOOT FIREBALL)
    // ========================================================================
    private void ShootFireball()
    {
        lastFireballTime = Time.time;

        // Tính hướng nhìn của Mario
        float dir = (transform.localScale.x >= 0) ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(dir * 0.5f, 0.4f, 0);

        GameObject fb = null;
        if (fireballPrefab != null)
        {
            fb = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            // Tự sinh cầu lửa bằng code nếu chưa gán prefab
            fb = CreateFireballRuntime(spawnPos);
        }

        Fireball fireballScript = fb.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.Launch(dir);
        }

        if (MarioVisualController.instance != null)
        {
            MarioVisualController.instance.TriggerFireShootPose();
        }

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(spawnPos, 6, new Color(1f, 0.6f, 0.1f));
        }
    }

    private GameObject CreateFireballRuntime(Vector3 pos)
    {
        GameObject fb = new GameObject("Fireball");
        fb.transform.position = pos;

        SpriteRenderer fbSr = fb.AddComponent<SpriteRenderer>();
        Sprite fbSprite = Resources.Load<Sprite>("Assets/Sprites/Mario/Fireball.png");
#if UNITY_EDITOR
        if (fbSprite == null)
        {
            fbSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Fireball.png");
        }
#endif
        if (fbSprite != null) fbSr.sprite = fbSprite;
        fbSr.sortingLayerName = "Player";

        Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        if (unlit != null) fbSr.material = new Material(unlit);

        CircleCollider2D col = fb.AddComponent<CircleCollider2D>();
        col.radius = 0.18f;

        Rigidbody2D rb = fb.AddComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        fb.AddComponent<Fireball>();
        return fb;
    }

    // ========================================================================
    // XỬ LÝ KHI BỊ TRÚNG ĐÒN (TAKE DAMAGE / HIT)
    // ========================================================================
    /// <summary>
    /// Trả về true nếu Mario đã đỡ đòn bằng form (thu nhỏ) và còn sống.
    /// Trả về false nếu Mario ở dạng Small và phải mất mạng.
    /// </summary>
    public bool TakeHit()
    {
        if (isStarman) return true; // Sao bất tử bỏ qua mọi sát thương

        if (playerCtrl != null && playerCtrl.isInvincible) return true;

        if (currentForm == MarioForm.Fire)
        {
            SetForm(MarioForm.Super);
            if (playerCtrl != null) playerCtrl.StartInvincibility(2f);
            return true;
        }
        else if (currentForm == MarioForm.Super)
        {
            SetForm(MarioForm.Small);
            if (playerCtrl != null) playerCtrl.StartInvincibility(2f);
            return true;
        }

        // Dạng Small Mario -> chết / mất mạng
        return false;
    }
}
