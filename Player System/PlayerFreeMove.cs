using UnityEngine;

public class PlayerFreeMove : MonoBehaviour
{
    public Transform characterBody;
    public Transform cameraArm;
    public PlayerAnimBridge animBridge;

    public float rotationSpeed = 720f;

    public Vector3 Move(Vector2 input, float speed, bool isRun)
    {
        float h = input.x;
        float v = input.y;

        bool isMove = input.sqrMagnitude > 0.01f;

        Vector3 camForward = cameraArm.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraArm.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        //  이동 방향 회전
        if (isMove)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);

            characterBody.rotation = Quaternion.RotateTowards(
                characterBody.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        //  단일 forward 애니메이션
        animBridge?.SetMove(0, isMove ? 1 : 0, isMove, isRun);

        return moveDir * speed;
    }
}