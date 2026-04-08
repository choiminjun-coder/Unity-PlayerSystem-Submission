using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("Refs")]
    public Transform characterBody;
    public Transform cameraArm;
    public Rigidbody rigid;
    public PlayerStats stats;
    public PlayerAnimBridge animBridge;

    [Header("LockOn")]
    public bool isLockOn;
    public Transform lockOnTarget;

    [Header("Air Control")]
    [Range(0f, 1f)]
    public float airControl = 0.3f;

    public bool IsBusy => isJump || isDodge;

    bool isRun;
    bool isJump;
    bool isDodge;

    float currentSpeed;

    Vector3 moveDirection;
    Vector3 desiredVelocity;

    Vector3 dodgeDirection; // 🔥 회피 방향 고정

    Vector2 input;
    bool runInput;

    void Awake()
    {
        if (!rigid) rigid = GetComponent<Rigidbody>();
        if (!stats) stats = GetComponent<PlayerStats>();
        if (!animBridge) animBridge = GetComponent<PlayerAnimBridge>();
        if (!cameraArm) cameraArm = transform.Find("CameraArm");
        if (!characterBody) characterBody = GetComponentInChildren<Animator>().transform;
        if (!lockOnTarget) lockOnTarget = GameObject.FindGameObjectWithTag("Enemy")?.transform;

        currentSpeed = stats.MoveSpeed;
    }

    public void TickMove(Vector2 moveInput, bool runHeld)
    {
        input = moveInput;
        runInput = runHeld;
    }

    void Update()
    {
        if (isLockOn && !lockOnTarget)
            lockOnTarget = GameObject.FindGameObjectWithTag("Enemy")?.transform;

        if (IsBusy)
        {
            desiredVelocity = Vector3.zero;
            return;
        }

        float h = input.x;
        float v = input.y;
        bool isMove = input.sqrMagnitude > 0.01f;

        isRun = runInput;
        currentSpeed = isRun ? stats.SprintSpeed : stats.MoveSpeed;

        Vector3 camForward = cameraArm.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraArm.right;
        camRight.y = 0;
        camRight.Normalize();

        moveDirection = (camForward * v + camRight * h).normalized;

        // 🔥 🔥 락온 / 일반 분기
        if (isLockOn && lockOnTarget != null)
        {
            UpdateLockOnMove(h, v, isMove, camForward, camRight);
        }
        else
        {
            UpdateFreeMove(isMove);
        }
    }

    // =========================
    // 🔓 일반 이동
    // =========================
    void UpdateFreeMove(bool isMove)
    {
        if (isMove)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);

            characterBody.rotation = Quaternion.RotateTowards(
                characterBody.rotation,
                targetRot,
                720f * Time.deltaTime
            );
        }

        desiredVelocity = moveDirection * currentSpeed;

        // 🔥 단일 애니메이션
        animBridge?.SetMove(0, isMove ? 1 : 0, isMove, isRun);
    }

    // =========================
    // 🔒 락온 이동
    // =========================
    void UpdateLockOnMove(float h, float v, bool isMove, Vector3 camForward, Vector3 camRight)
    {
        // 🔥 타겟 바라보기
        Vector3 dir = lockOnTarget.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);

            characterBody.rotation = Quaternion.RotateTowards(
                characterBody.rotation,
                targetRot,
                720f * Time.deltaTime
            );
        }

        desiredVelocity = moveDirection * currentSpeed;

        // 🔥 스트레이프 애니메이션
        animBridge?.SetMove(h, v, isMove, isRun);
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        Vector3 velocity = rigid.linearVelocity;

        if (isDodge)
        {
            velocity.x = dodgeDirection.x * currentSpeed;
            velocity.z = dodgeDirection.z * currentSpeed;
        }
        else if (isJump)
        {
            velocity.x = Mathf.Lerp(velocity.x, desiredVelocity.x, airControl);
            velocity.z = Mathf.Lerp(velocity.z, desiredVelocity.z, airControl);
        }
        else
        {
            velocity.x = desiredVelocity.x;
            velocity.z = desiredVelocity.z;
        }

        rigid.linearVelocity = velocity;
    }

    public bool TryJump(bool jumpPressed)
    {
        if (!jumpPressed || isJump || isDodge) return false;

        isJump = true;

        animBridge?.JumpStart(moveDirection.magnitude);

        Vector3 velocity = rigid.linearVelocity;
        velocity.y = stats.JumpPower;
        rigid.linearVelocity = velocity;

        return true;
    }

    public void NotifyLanded()
    {
        isJump = false;
        animBridge?.Land();
    }

    public void TryDodge(bool dodgePressed)
    {
        if (!dodgePressed || isJump || isDodge) return;

        StartCoroutine(DodgeRoutine());
    }

    IEnumerator DodgeRoutine()
    {
        isDodge = true;

        Vector3 inputDir;

        // 🔥 입력 방향 가져오기
        float h = input.x;
        float v = input.y;

        if (isLockOn)
        {
            // 🔒 락온 → 캐릭터 기준 방향
            inputDir = (characterBody.forward * v + characterBody.right * h).normalized;
        }
        else
        {
            // 🔓 일반 → 이동 방향 그대로
            inputDir = moveDirection;
        }

        // 🔥 입력 없으면 앞으로 회피
        dodgeDirection = inputDir.sqrMagnitude > 0.01f
            ? inputDir
            : characterBody.forward;

        animBridge?.Dodge(dodgeDirection);

        float originalMove = stats.MoveSpeed;
        float originalSprint = stats.SprintSpeed;

        stats.MoveSpeed *= 2f;
        stats.SprintSpeed *= 2f;

        yield return new WaitForSeconds(0.5f);

        stats.MoveSpeed = originalMove;
        stats.SprintSpeed = originalSprint;

        isDodge = false;
    }

    // 🔥 외부에서 호출
    public void SetLockOnTarget(Transform target)
    {
        lockOnTarget = target;
        isLockOn = target != null;
    }
}