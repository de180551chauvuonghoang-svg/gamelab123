using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Tooltip("Tốc độ bập bồng lên xuống nhẹ nhàng để tạo sự chú ý")]
    public float floatSpeed = 3f;
    public float floatHeight = 0.15f;

    private Vector3 startPos;

    private void Awake()
    {
        // Đảm bảo chìa khóa luôn sáng rõ vàng óng, không bị đen do ánh sáng URP 2D
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Shader unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") 
                        ?? Shader.Find("Sprites/Default");
            if (unlitShader != null)
            {
                sr.material = new Material(unlitShader);
            }
        }
    }

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Hiệu ứng bập bồng nhấp nhô của chìa khóa vàng
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi Player nhặt được Key -> Kích hoạt màn hình You Win
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.WinGame();
            }

            Destroy(gameObject);
        }
    }
}
