using UnityEngine;
using System.Collections;

/// <summary>
/// Camera chuyên nghiệp với: Smooth Follow, Look-Ahead, Dead Zone, Camera Shake.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("--- MỤC TIÊU THEO DÕI ---")]
    [Tooltip("Mục tiêu camera theo dõi (Player)")]
    public Transform target;

    [Header("--- SMOOTH FOLLOW ---")]
    [Tooltip("Tốc độ bám theo chung (tương thích ngược với Lab 2 và Scene cũ)")]
    public float smoothSpeed = 0.12f;

    [Tooltip("Tốc độ bám theo chiều ngang (0.05 = rất mượt, 0.3 = nhanh)")]
    public float smoothSpeedX = 0.12f;

    [Tooltip("Tốc độ bám theo chiều dọc")]
    public float smoothSpeedY = 0.1f;

    [Tooltip("Khoảng cách lệch vị trí (mặc định Z = -10 cho camera 2D)")]
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    [Header("--- LOOK-AHEAD (NHÌN TRƯỚC) ---")]
    [Tooltip("Camera dịch trước theo hướng Player đang di chuyển")]
    public float lookAheadDistance = 1.8f;

    [Tooltip("Tốc độ dịch chuyển look-ahead")]
    public float lookAheadSmooth = 0.08f;

    [Header("--- DEAD ZONE (VÙNG CHẾT) ---")]
    [Tooltip("Camera không di chuyển khi Player ở trong vùng chết (chiều ngang)")]
    public float deadZoneX = 0.3f;

    [Tooltip("Dead zone chiều dọc")]
    public float deadZoneY = 0.2f;

    // --- NỘI BỘ ---
    private float currentLookAhead;
    private float targetLookAhead;
    private Vector3 velocity = Vector3.zero;
    private Rigidbody2D targetRb;

    // Camera Shake
    private float shakeIntensity;
    private float shakeDuration;
    private float shakeTimer;
    private Vector3 shakeOffset;

    private void Awake()
    {
        // Đồng bộ nếu smoothSpeed được gán từ script bên ngoài (ví dụ Lab2Automator) hoặc Inspector cũ
        if (Mathf.Abs(smoothSpeed - 0.12f) > 0.001f)
        {
            smoothSpeedX = smoothSpeed;
            smoothSpeedY = smoothSpeed;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            // Tự động tìm Player nếu chưa được gán
            GameObject playerGo = GameObject.FindWithTag("Player");
            if (playerGo != null)
            {
                target = playerGo.transform;
                targetRb = playerGo.GetComponent<Rigidbody2D>();
            }
            return;
        }

        if (targetRb == null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
        }

        // ----- LOOK-AHEAD -----
        // Dịch camera trước theo hướng di chuyển ngang của Player
        float playerVelX = targetRb != null ? targetRb.linearVelocity.x : 0f;
        if (Mathf.Abs(playerVelX) > 0.5f)
        {
            targetLookAhead = Mathf.Sign(playerVelX) * lookAheadDistance;
        }
        else
        {
            targetLookAhead = 0f;
        }
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, lookAheadSmooth);

        // ----- VỊ TRÍ MỤC TIÊU -----
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x += currentLookAhead;

        // ----- DEAD ZONE -----
        // Camera chỉ di chuyển khi Player ra ngoài vùng chết
        Vector3 currentPos = transform.position;
        float deltaX = desiredPosition.x - currentPos.x;
        float deltaY = desiredPosition.y - currentPos.y;

        float newX = currentPos.x;
        float newY = currentPos.y;

        if (Mathf.Abs(deltaX) > deadZoneX)
        {
            newX = Mathf.Lerp(currentPos.x, desiredPosition.x, smoothSpeedX);
        }

        if (Mathf.Abs(deltaY) > deadZoneY)
        {
            newY = Mathf.Lerp(currentPos.y, desiredPosition.y, smoothSpeedY);
        }

        Vector3 smoothedPosition = new Vector3(newX, newY, desiredPosition.z);

        // ----- CAMERA SHAKE -----
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.unscaledDeltaTime;
            float progress = shakeTimer / shakeDuration;
            float currentIntensity = shakeIntensity * progress; // Giảm dần

            shakeOffset = new Vector3(
                Random.Range(-currentIntensity, currentIntensity),
                Random.Range(-currentIntensity, currentIntensity),
                0f
            );
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        transform.position = smoothedPosition + shakeOffset;
    }

    /// <summary>
    /// Rung camera với cường độ và thời gian chỉ định.
    /// Gọi từ bất kỳ script nào: FindFirstObjectByType<CameraFollow>().Shake(0.15f, 0.2f);
    /// </summary>
    public void Shake(float intensity, float duration)
    {
        // Chỉ cho phép shake mạnh hơn ghi đè shake yếu hơn
        if (intensity > shakeIntensity * (shakeTimer / Mathf.Max(shakeDuration, 0.01f)))
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
            shakeTimer = duration;
        }
    }
}
