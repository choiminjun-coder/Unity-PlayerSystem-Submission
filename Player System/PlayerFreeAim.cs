using UnityEngine;

public class PlayerFreeAim : MonoBehaviour
{
    public Transform cameraArm;
    public float sensitivity = 0.1f;

    float pitch;
    float yaw;

    void Awake()
    {
        if (!cameraArm)
            cameraArm = transform.Find("CameraArm");
    }

    void Start()
    {
        Vector3 angles = cameraArm.rotation.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    public void Tick(Vector2 lookDelta)
    {
        lookDelta *= sensitivity;

        yaw += lookDelta.x;
        pitch -= lookDelta.y;

        pitch = Mathf.Clamp(pitch, -40f, 70f);

        cameraArm.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}