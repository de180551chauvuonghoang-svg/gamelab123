using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("--- TUẦN TRA A - B ---")]
    [Tooltip("Điểm mốc bên trái")]
    public Transform pointA;

    [Tooltip("Điểm mốc bên phải")]
    public Transform pointB;

    [Tooltip("Tốc độ di chuyển của quái")]
    public float speed = 2.5f;

    [Header("--- CƠ CHẾ DẪM BẸP (STOMP) ---")]
    [Tooltip("Lực nảy của Player khi dẫm trúng đầu quái")]
    public float bounceForce = 11f;

    [Tooltip("Điểm thưởng khi tiêu diệt quái")]
    public int scoreReward = 5;

    private Transform currentTarget;
    private bool isFacingRight = true;
    private bool isDead = false;

    private void Awake()
    {
        // Đảm bảo quái vật luôn hiển thị đầy đủ màu sắc, không bị đen
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") 
                        ?? Shader.Find("Sprites/Default");
            if (unlit != null)
            {
                sr.material = new Material(unlit);
            }
        }
    }

    private void Start()
    {
        // Ban đầu quái di chuyển về phía điểm B
        if (pointB != null)
        {
            currentTarget = pointB;
        }
        else if (pointA != null)
        {
            currentTarget = pointA;
        }
    }

    private void Update()
    {
        // Nếu quái đã bị dẫm chết thì dừng di chuyển
        if (isDead || currentTarget == null) return;

        // Di chuyển dần dần về phía điểm đích
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Khi đã đến rất gần điểm đích (khoảng cách < 0.05m) -> Quay đầu
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.05f)
        {
            if (currentTarget == pointB)
            {
                currentTarget = pointA;
                Flip(false); // Quay mặt sang trái
            }
            else
            {
                currentTarget = pointB;
                Flip(true);  // Quay mặt sang phải
            }
        }
    }

    private void Flip(bool faceRight)
    {
        isFacingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // Xử lý tương tác va chạm giữa Player và Quái vật
    private void HandlePlayerInteraction(GameObject playerObj)
    {
        if (isDead) return;

        PlayerController player = playerObj.GetComponent<PlayerController>();
        Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();

        // Kiểm tra góc va chạm:
        // 1. Vị trí Player cao hơn đỉnh quái (đang nhảy dẫm từ trên xuống)
        // 2. Vận tốc Player đang rơi xuống hoặc không bay lên quá nhanh (linearVelocity.y <= 0.5f)
        bool isAboveEnemy = playerObj.transform.position.y > transform.position.y + 0.15f;
        bool isFallingDown = playerRb != null && playerRb.linearVelocity.y <= 0.5f;

        if (isAboveEnemy && isFallingDown)
        {
            // DẪM BẸP THÀNH CÔNG!
            Stomp(player);
        }
        else
        {
            // Va chạm ngang sườn hoặc quái cắn từ phía sau -> Game Over
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }

    private void Stomp(PlayerController player)
    {
        isDead = true;

        // 1. Tắt Collider ngay để quái không gây sát thương thêm
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. Cho Player nảy ngược lên không trung
        if (player != null)
        {
            player.Bounce(bounceForce);
        }

        // 3. Cộng điểm thưởng tiêu diệt quái
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreReward);
        }

        // 4. Phát âm thanh nhảy / dẫm quái
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayJumpSFX();
        }

        // 5. Hiệu ứng bẹp dí: Dẹp chiều cao xuống 25%, bè rộng sang hai bên
        Vector3 squashedScale = transform.localScale;
        squashedScale.y *= 0.25f;
        squashedScale.x *= 1.4f;
        transform.localScale = squashedScale;

        // Hạ nhẹ vị trí xuống sát sàn
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.12f, transform.position.z);

        // Đổi màu đỏ sẫm báo hiệu quái bị bẹp
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 0.3f, 0.3f, 0.85f);
        }

        // 6. Tự hủy sau 0.4 giây
        Destroy(gameObject, 0.4f);
    }

    // Khi quái chạm vào Player qua Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerInteraction(collision.gameObject);
        }
    }

    // Khi quái chạm vào Player qua Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandlePlayerInteraction(collision.gameObject);
        }
    }

    // Vẽ đường tuần tra trong cửa sổ Scene để trực quan hoá
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }
    }
}
