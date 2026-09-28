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

    private Transform currentTarget;
    private bool isFacingRight = true;

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
        if (currentTarget == null) return;

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

    // Khi quái chạm vào Player qua Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }

    // Hỗ trợ cả trường hợp Collider của quái là Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
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
