using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void Awake()
    {
        // Đảm bảo bẫy chông luôn sáng rõ, không bị bóng đen do ánh sáng URP 2D
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi Player rơi trúng bẫy gai (Trap) -> Kích hoạt Game Over
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }
}
