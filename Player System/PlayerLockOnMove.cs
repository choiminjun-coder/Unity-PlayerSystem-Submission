using UnityEngine;

public class PlayerLockOnMove : MonoBehaviour
{
    public Transform characterBody;
    public Transform cameraArm;
    public PlayerAnimBridge animBridge;

    public Transform target;
    public float rotationSpeed = 720f;

    public Vector3 Move(Vector2 input, float speed, bool isRun)
    {
        float h = input.x;
        float v = input.y;

        bool isMove = input.sqrMagnitude > 0.01f;

        // 🔥 타겟 바라보기
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);

                characterBody.rotation = Quaternion.RotateTowards(
                    characterBody.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        // 🔥 이동 (카메라 기준)
        Vector3 camForward = cameraArm.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraArm.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 🔥 스트레이프 애니메이션
        animBridge?.SetMove(h, v, isMove, isRun);

        return moveDir * speed;
    }
}