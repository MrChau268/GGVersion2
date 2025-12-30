using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Target")]
    public Transform target; // Player

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0, 2f, -4f);

    [Header("Camera Settings")]
    public float smoothSpeed = 5f;
    public float rotationSpeed = 3f;

    private float yaw;
    private float pitch;

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse input (optional)
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -20f, 60f);

        // Rotation around player
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
