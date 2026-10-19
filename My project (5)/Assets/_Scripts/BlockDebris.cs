using UnityEngine;

/// <summary>
/// Mảnh vỡ gạch khi Super Mario húc đầu vỡ khối gạch chuẩn Mario.
/// Bay theo quỹ đạo parabol và xoay tít rồi biến mất.
/// </summary>
public class BlockDebris : MonoBehaviour
{
    public float lifetime = 2.5f;

    public static void SpawnDebris(Vector3 blockPosition, Sprite brickSprite, Material mat)
    {
        Vector2[] velocities = new Vector2[]
        {
            new Vector2(-3.5f, 9f),
            new Vector2(3.5f, 9f),
            new Vector2(-2.5f, 6.5f),
            new Vector2(2.5f, 6.5f)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject shard = new GameObject("Brick_Shard_" + i);
            shard.transform.position = blockPosition + new Vector3((i % 2 == 0 ? -0.2f : 0.2f), (i < 2 ? 0.2f : -0.2f), 0);
            shard.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

            SpriteRenderer sr = shard.AddComponent<SpriteRenderer>();
            sr.sprite = brickSprite;
            if (mat != null) sr.material = mat;
            sr.sortingLayerName = "Player";

            Rigidbody2D rb = shard.AddComponent<Rigidbody2D>();
            rb.linearVelocity = velocities[i];
            rb.angularVelocity = (i % 2 == 0 ? 500f : -500f);
            rb.gravityScale = 3f;

            // Không va chạm với player
            CircleCollider2D col = shard.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            shard.AddComponent<BlockDebris>();
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
