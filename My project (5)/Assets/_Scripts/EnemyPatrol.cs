using UnityEngine;
using System.Collections;

/// <summary>
/// EnemyPatrol nâng cấp:
/// - Tuần tra A-B (giữ nguyên)
/// - Phát hiện Player → Chase Mode (chạy nhanh hơn đuổi theo)
/// - Dẫm bẹp (Stomp) với combo scoring
/// - Chết mượt: fade out + particle thay vì biến mất tức thì
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    // ========================================================================
    // TUẦN TRA
    // ========================================================================
    [Header("--- TUẦN TRA A - B ---")]
    [Tooltip("Điểm mốc bên trái")]
    public Transform pointA;

    [Tooltip("Điểm mốc bên phải")]
    public Transform pointB;

    [Tooltip("Tốc độ di chuyển bình thường")]
    public float speed = 2.5f;

    // ========================================================================
    // CHASE MODE (ĐUỔI THEO PLAYER)
    // ========================================================================
    [Header("--- CHASE MODE ---")]
    [Tooltip("Bật/tắt hành vi đuổi theo Player")]
    public bool enableChase = true;

    [Tooltip("Bán kính phát hiện Player")]
    public float detectionRange = 5f;

    [Tooltip("Tốc độ đuổi theo (nhanh hơn tuần tra)")]
    public float chaseSpeed = 4f;

    [Tooltip("Bán kính mất dấu (dừng đuổi khi Player ra xa)")]
    public float loseRange = 7f;

    // ========================================================================
    // DẪM BẸP (STOMP)
    // ========================================================================
    [Header("--- CƠ CHẾ DẪM BẸP (STOMP) ---")]
    [Tooltip("Lực nảy của Player khi dẫm trúng đầu quái")]
    public float bounceForce = 11f;

    [Tooltip("Điểm thưởng cơ bản khi tiêu diệt quái")]
    public int scoreReward = 5;

    // ========================================================================
    // BIẾN NỘI BỘ
    // ========================================================================
    private Transform currentTarget;
    private bool isFacingRight = true;
    private bool isDead = false;
    private bool isChasing = false;
    private Transform playerTransform;
    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Đảm bảo quái hiển thị đúng màu (không bị đen do URP 2D)
        if (sr != null)
        {
            Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                        ?? Shader.Find("Sprites/Default");
            if (unlit != null)
            {
                sr.material = new Material(unlit);
            }
            originalColor = sr.color;
        }
    }

    private void Start()
    {
        // Ban đầu tuần tra về phía điểm B
        if (pointB != null)
        {
            currentTarget = pointB;
        }
        else if (pointA != null)
        {
            currentTarget = pointA;
        }

        // Tìm Player
        GameObject playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null)
        {
            playerTransform = playerGo.transform;
        }
    }

    private void Update()
    {
        if (isDead || currentTarget == null) return;

        // Tìm lại Player nếu cần
        if (playerTransform == null)
        {
            GameObject playerGo = GameObject.FindWithTag("Player");
            if (playerGo != null) playerTransform = playerGo.transform;
        }

        // --- CHASE MODE ---
        if (enableChase && playerTransform != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (!isChasing && distToPlayer <= detectionRange)
            {
                // Phát hiện Player!
                isChasing = true;

                // Đổi màu hơi đỏ để báo hiệu đang tức giận
                if (sr != null)
                {
                    sr.color = new Color(1f, 0.7f, 0.7f, 1f);
                }
            }
            else if (isChasing && distToPlayer > loseRange)
            {
                // Mất dấu Player → quay về tuần tra
                isChasing = false;
                if (sr != null)
                {
                    sr.color = originalColor;
                }
            }
        }

        float currentSpeed;
        Vector3 moveTarget;

        if (isChasing && playerTransform != null)
        {
            // --- ĐUỔI THEO PLAYER ---
            currentSpeed = chaseSpeed;
            // Chỉ đuổi theo chiều ngang (không bay lên)
            moveTarget = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);

            // Flip hướng theo Player
            if (playerTransform.position.x > transform.position.x + 0.1f)
            {
                Flip(true);
            }
            else if (playerTransform.position.x < transform.position.x - 0.1f)
            {
                Flip(false);
            }
        }
        else
        {
            // --- TUẦN TRA BÌNH THƯỜNG ---
            currentSpeed = speed;
            moveTarget = currentTarget.position;

            // Khi đến điểm đích → đổi chiều
            if (Vector2.Distance(transform.position, currentTarget.position) < 0.05f)
            {
                if (currentTarget == pointB)
                {
                    currentTarget = pointA;
                    Flip(false);
                }
                else
                {
                    currentTarget = pointB;
                    Flip(true);
                }
            }
        }

        // Di chuyển
        transform.position = Vector2.MoveTowards(transform.position, moveTarget, currentSpeed * Time.deltaTime);
    }

    // ========================================================================
    // QUAY MẶT
    // ========================================================================
    private void Flip(bool faceRight)
    {
        isFacingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // ========================================================================
    // XỬ LÝ VA CHẠM VỚI PLAYER
    // ========================================================================
    private void HandlePlayerInteraction(GameObject playerObj)
    {
        if (isDead) return;

        PlayerController player = playerObj.GetComponent<PlayerController>();

        // Kiểm tra bất tử
        if (player != null && player.isInvincible) return;

        Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();

        // Kiểm tra góc va chạm — dẫm từ trên xuống
        bool isAboveEnemy = playerObj.transform.position.y > transform.position.y + 0.15f;
        bool isFallingDown = playerRb != null && playerRb.linearVelocity.y <= 0.5f;

        if (isAboveEnemy && isFallingDown)
        {
            // DẪM BẸP THÀNH CÔNG!
            Stomp(player);
        }
        else
        {
            // Va chạm ngang → mất máu (TakeDamage thay vì GameOver trực tiếp)
            if (GameManager.instance != null)
            {
                GameManager.instance.TakeDamage();
            }
        }
    }

    // ========================================================================
    // DẪM BẸP (STOMP) — với combo scoring
    // ========================================================================
    private void Stomp(PlayerController player)
    {
        isDead = true;

        // 1. Tắt Collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. Player nảy lên
        if (player != null)
        {
            player.Bounce(bounceForce);
        }

        // 3. Cộng điểm COMBO (nhân hệ số combo)
        if (GameManager.instance != null)
        {
            GameManager.instance.AddStompScore(scoreReward, transform.position);
        }

        // 4. Âm thanh
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayJumpSFX();
        }

        // 5. Hiệu ứng chết mượt: Squash + Fade Out + Particle
        StartCoroutine(SmoothDeathRoutine());
    }

    /// <summary>
    /// Hiệu ứng chết mượt: Bẹp dần → Fade out → Particle nổ → Hủy
    /// </summary>
    private IEnumerator SmoothDeathRoutine()
    {
        // Bước 1: Squash (bẹp dí) dần dần trong 0.15s
        Vector3 originalScale = transform.localScale;
        Vector3 squashedScale = new Vector3(
            originalScale.x * 1.4f,
            originalScale.y * 0.25f,
            originalScale.z
        );

        float squashDuration = 0.15f;
        float elapsed = 0f;

        while (elapsed < squashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / squashDuration;
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, t);

            // Hạ vị trí xuống sát sàn
            transform.position += Vector3.down * 0.8f * Time.deltaTime;
            yield return null;
        }

        transform.localScale = squashedScale;

        // Bước 2: Fade out dần trong 0.3s
        float fadeDuration = 0.3f;
        elapsed = 0f;
        Color startColor = sr != null ? sr.color : Color.white;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            if (sr != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(startColor.a, 0f, t);
                c.r = Mathf.Lerp(startColor.r, 1f, t * 0.5f); // Hơi đỏ lên
                sr.color = c;
            }

            yield return null;
        }

        // Bước 3: Particle nổ tại vị trí quái
        ParticleSpawner.SpawnEnemyDeath(transform.position);

        // Bước 4: Hủy
        Destroy(gameObject);
    }

    // ========================================================================
    // VA CHẠM
    // ========================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerInteraction(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandlePlayerInteraction(collision.gameObject);
        }
    }

    // ========================================================================
    // GIZMOS
    // ========================================================================
    private void OnDrawGizmos()
    {
        // Đường tuần tra
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }

        // Vùng phát hiện Player
        if (enableChase)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
            Gizmos.DrawWireSphere(transform.position, loseRange);
        }
    }
}
