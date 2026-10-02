using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("--- ĐIỂM MỐC DI CHUYỂN ---")]
    [Tooltip("Điểm mốc A")]
    public Transform pointA;

    [Tooltip("Điểm mốc B")]
    public Transform pointB;

    [Header("--- THIẾT LẬP TỐC ĐỘ ---")]
    [Tooltip("Tốc độ di chuyển của sàn")]
    public float speed = 2.5f;

    [Tooltip("Thời gian chờ tại mỗi điểm dừng (giây)")]
    public float waitTime = 0.5f;

    private Transform currentTarget;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private void Start()
    {
        // Khởi đầu di chuyển hướng về điểm B
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
        if (pointA == null || pointB == null) return;

        // Nếu đang ở trạng thái dừng chờ đổi chiều
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        // Tự động tịnh tiến sàn về phía mục tiêu
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Khi sàn đã chạm đến điểm mốc mục tiêu
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.05f)
        {
            // Bắt đầu chờ một chút trước khi quay đầu (tạo cảm giác tự nhiên)
            isWaiting = true;
            waitTimer = waitTime;

            // Đổi chiều mục tiêu (A <-> B)
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    // Khi Player bước lên sàn di chuyển
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kiểm tra tiếp xúc: Chỉ gắn làm con khi Player đứng trên mặt sàn (chân đạp lên sàn)
            // (tránh trường hợp Player húc đầu vào dưới đáy sàn mà bị dính theo)
            if (collision.contactCount > 0 && collision.contacts[0].normal.y < -0.3f)
            {
                // Gán Player làm con (Child) của Platform để di chuyển đồng bộ
                collision.transform.SetParent(transform);
            }
        }
    }

    // Khi Player nhảy ra hoặc rời khỏi sàn di chuyển
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tách Player ra khỏi Platform (trả về trạng thái độc lập)
            if (collision.transform.parent == transform)
            {
                collision.transform.SetParent(null);
            }
        }
    }

    // Vẽ đường đi giữa A và B trong cửa sổ Scene để trực quan hóa
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }
    }
}
