using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Mục tiêu camera theo dõi (Player)")]
    public Transform target;

    [Tooltip("Độ trễ mượt của camera")]
    public float smoothSpeed = 0.125f;

    [Tooltip("Khoảng cách lệch vị trí (mặc định Z = -10 cho camera 2D)")]
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    private void LateUpdate()
    {
        if (target == null)
        {
            // Tự động tìm Player nếu chưa được gán
            GameObject playerGo = GameObject.FindWithTag("Player");
            if (playerGo != null)
            {
                target = playerGo.transform;
            }
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
