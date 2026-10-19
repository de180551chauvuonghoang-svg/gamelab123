using UnityEngine;

/// <summary>
/// Hiệu ứng Parallax Background nhiều lớp chuẩn game Mario hiện đại:
/// - Mây trời trôi nhẹ nhàng và dịch chuyển chậm theo camera.
/// - Đồi núi phía xa cuộn cảnh với chiều sâu 3D ấn tượng.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("--- THIẾT LẬP PARALLAX ---")]
    [Tooltip("Hệ số dịch chuyển theo camera (0 = đứng yên, 1 = đi theo camera)")]
    public Vector2 parallaxEffectMultiplier = new Vector2(0.25f, 0.05f);

    [Tooltip("Tốc độ tự trôi liên tục (ví dụ cho mây trời lững lờ trôi)")]
    public float autoScrollSpeedX = 0.2f;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            lastCameraPosition = cameraTransform.position;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
                lastCameraPosition = cameraTransform.position;
            }
            return;
        }

        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Dịch chuyển parallax + tự trôi
        float moveX = deltaMovement.x * parallaxEffectMultiplier.x + (autoScrollSpeedX * Time.deltaTime);
        float moveY = deltaMovement.y * parallaxEffectMultiplier.y;

        transform.position += new Vector3(moveX, moveY, 0f);

        lastCameraPosition = cameraTransform.position;
    }
}
