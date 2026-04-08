using UnityEngine;

public class PlayerLockOnAim : MonoBehaviour
{
    public Transform cameraArm;
    public Transform target;

    public float rotationSpeed = 10f;
    public float pitchOffset = -30f;

    void Awake()
    {
        if (!cameraArm) cameraArm = transform.Find("CameraArm");
        if (!target) target = GameObject.FindGameObjectWithTag("Enemy")?.transform;
    }

    public void Tick()
    {

        if (target == null) return;

        Vector3 dir = target.position - cameraArm.position;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        Vector3 euler = targetRot.eulerAngles;
        euler.x += pitchOffset;

        targetRot = Quaternion.Euler(euler);

        cameraArm.rotation = Quaternion.Slerp(
            cameraArm.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }
}