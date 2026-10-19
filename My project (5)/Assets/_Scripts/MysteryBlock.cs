using UnityEngine;
using System.Collections;

/// <summary>
/// Khối Chấm Hỏi [?] (Mystery Block) chuẩn Super Mario:
/// - Khi Mario húc đầu từ dưới lên: Khối nảy tưng lên, biến thành khối đá xám (Empty Block).
/// - Sinh ra vật phẩm tương ứng: Coin bay lên, Nấm lớn (Mushroom), Hoa lửa (Fire Flower), hoặc Sao bất tử (Starman)!
/// </summary>
public class MysteryBlock : MonoBehaviour
{
    public enum ContentType { Coin, SuperMushroom, FireFlower, Starman }

    [Header("--- VẬT PHẨM TRONG KHỐI ---")]
    public ContentType content = ContentType.Coin;

    [Header("--- SPRITES ---")]
    public Sprite emptyBlockSprite;

    private SpriteRenderer sr;
    private Vector3 originalPos;
    private bool isOpened = false;
    private bool isBumping = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPos = transform.position;

        if (emptyBlockSprite == null)
        {
#if UNITY_EDITOR
            emptyBlockSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Block_Empty.png");
#endif
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBumping) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Mario nhảy từ dưới lên húc vào đáy khối
            if (contact.normal.y > 0.5f && (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerController>() != null))
            {
                HitBlock();
                break;
            }
        }
    }

    private void HitBlock()
    {
        if (!isOpened)
        {
            isOpened = true;
            StartCoroutine(OpenBlockRoutine());
        }
        else
        {
            StartCoroutine(BumpRoutine());
        }
    }

    private IEnumerator OpenBlockRoutine()
    {
        isBumping = true;
        float bumpHeight = 0.28f;
        float duration = 0.08f;

        // Đổi sang sprite khối rỗng
        if (emptyBlockSprite != null && sr != null)
        {
            sr.sprite = emptyBlockSprite;
        }

        // 1. Nảy khối lên
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = originalPos + Vector3.up * Mathf.Lerp(0, bumpHeight, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Tiêu diệt kẻ địch đang đứng ngay trên nóc khối
        CheckHitEnemyAbove();

        // 2. Nhả vật phẩm ra
        SpawnReward();

        // 3. Rớt về vị trí ban đầu
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = originalPos + Vector3.up * Mathf.Lerp(bumpHeight, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        isBumping = false;
    }

    private IEnumerator BumpRoutine()
    {
        isBumping = true;
        float bumpHeight = 0.15f;
        float duration = 0.06f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = originalPos + Vector3.up * Mathf.Lerp(0, bumpHeight, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        CheckHitEnemyAbove();

        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = originalPos + Vector3.up * Mathf.Lerp(bumpHeight, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        isBumping = false;
    }

    private void CheckHitEnemyAbove()
    {
        Collider2D hitAbove = Physics2D.OverlapBox(originalPos + Vector3.up * 0.7f, new Vector2(0.8f, 0.4f), 0);
        if (hitAbove != null)
        {
            EnemyPatrol patrol = hitAbove.GetComponentInParent<EnemyPatrol>();
            if (patrol != null) patrol.StompKill();

            GoombaEnemy goomba = hitAbove.GetComponentInParent<GoombaEnemy>();
            if (goomba != null) goomba.Stomp();

            KoopaEnemy koopa = hitAbove.GetComponentInParent<KoopaEnemy>();
            if (koopa != null) koopa.DefeatByFireball();
        }
    }

    private void SpawnReward()
    {
        switch (content)
        {
            case ContentType.Coin:
                StartCoroutine(PopUpCoinRoutine());
                break;
            case ContentType.SuperMushroom:
                SpawnMushroom();
                break;
            case ContentType.FireFlower:
                SpawnFireFlower();
                break;
            case ContentType.Starman:
                SpawnStarman();
                break;
        }
    }

    private IEnumerator PopUpCoinRoutine()
    {
        // Tạo đồng xu bay lên
        GameObject coin = new GameObject("BlockCoin");
        coin.transform.position = originalPos + Vector3.up * 0.5f;

        SpriteRenderer cSr = coin.AddComponent<SpriteRenderer>();
        Sprite cSprite = null;
#if UNITY_EDITOR
        cSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Coin.png");
#endif
        if (cSprite != null) cSr.sprite = cSprite;
        cSr.sortingLayerName = "Player";

        Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        if (unlit != null) cSr.material = new Material(unlit);

        // Hiệu ứng đồng xu nhảy vút lên rồi rớt xuống
        float duration = 0.45f;
        float elapsed = 0f;
        Vector3 startCoinPos = coin.transform.position;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Quỹ đạo nảy parabol
            float yOffset = Mathf.Sin(t * Mathf.PI) * 1.8f;
            coin.transform.position = startCoinPos + Vector3.up * yOffset;
            coin.transform.Rotate(0, 720f * Time.deltaTime, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Cộng điểm & coin cho người chơi
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(200);
        }
        if (MarioGameManager.instance != null)
        {
            MarioGameManager.instance.AddCoin();
        }

        if (ParticleSpawner.instance != null)
        {
            ParticleSpawner.instance.SpawnDust(coin.transform.position, 12, new Color(1f, 0.85f, 0.15f));
        }

        Destroy(coin);
    }

    private void SpawnMushroom()
    {
        GameObject shroom = new GameObject("SuperMushroom");
        SpriteRenderer sSr = shroom.AddComponent<SpriteRenderer>();
#if UNITY_EDITOR
        sSr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Item_Mushroom.png");
#endif
        sSr.sortingLayerName = "Player";
        Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        if (unlit != null) sSr.material = new Material(unlit);

        BoxCollider2D col = shroom.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 0.8f);

        Rigidbody2D rb = shroom.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        SuperMushroom sm = shroom.AddComponent<SuperMushroom>();
        sm.EmergeFromBlock(originalPos);
    }

    private void SpawnFireFlower()
    {
        GameObject flower = new GameObject("FireFlower");
        SpriteRenderer fSr = flower.AddComponent<SpriteRenderer>();
#if UNITY_EDITOR
        fSr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Item_FireFlower.png");
#endif
        fSr.sortingLayerName = "Player";
        Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        if (unlit != null) fSr.material = new Material(unlit);

        BoxCollider2D col = flower.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.8f, 0.8f);

        FireFlowerItem ffi = flower.AddComponent<FireFlowerItem>();
        ffi.EmergeFromBlock(originalPos);
    }

    private void SpawnStarman()
    {
        GameObject star = new GameObject("Starman");
        SpriteRenderer stSr = star.AddComponent<SpriteRenderer>();
#if UNITY_EDITOR
        stSr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mario/Item_Star.png");
#endif
        stSr.sortingLayerName = "Player";
        Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit") ?? Shader.Find("Sprites/Default");
        if (unlit != null) stSr.material = new Material(unlit);

        CircleCollider2D col = star.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;

        Rigidbody2D rb = star.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        StarmanItem starItem = star.AddComponent<StarmanItem>();
        starItem.EmergeFromBlock(originalPos);
    }
}
