using UnityEngine;

public class Player : MonoBehaviour
{
    private IPlayerInput input;
    private PlayerMotor motor;

    private PlayerFreeAim freeAim;
    private PlayerLockOnAim lockOnAim;

    private PlayerAnimBridge animBridge;

    void Awake()
    {
        input = GetComponent<IPlayerInput>();
        if (input == null)
        {
            Debug.LogError("[Player] IPlayerInput 없음", this);
            enabled = false;
            return;
        }

        motor = GetComponent<PlayerMotor>();
        freeAim = GetComponent<PlayerFreeAim>();
        lockOnAim = GetComponent<PlayerLockOnAim>();
        animBridge = GetComponent<PlayerAnimBridge>();

        if (motor == null) Debug.LogError("[Player] PlayerMotor 없음", this);
        if (freeAim == null) Debug.LogError("[Player] PlayerFreeAim 없음", this);
        if (lockOnAim == null) Debug.LogError("[Player] PlayerLockOnAim 없음", this);
        if (animBridge == null) Debug.LogError("[Player] PlayerAnimBridge 없음", this);
    }

    void Start()
    {
        animBridge?.InitDefaults();
    }

    void Update()
    {
        if (motor == null || animBridge == null) return;

        input.Tick();

        //  입력 전달
        motor.TickMove(input.Move, input.RunHeld);

        //  카메라 처리 (모터 상태 기준)
        if (motor.isLockOn)
        {
            lockOnAim.Tick();
        }
        else
        {
            freeAim.Tick(input.Look);
        }

        //  점프
        if (motor.TryJump(input.JumpDown))
        {
        }

        //  회피
        motor.TryDodge(input.DodgeDown);

        //  락온 토글 (예시)
        if (input.InteractDown)
        {
            ToggleLockOn();
        }
    }

    void ToggleLockOn()
    {
        if (motor.isLockOn)
        {
            motor.SetLockOnTarget(null);
        }
        else
        {
            //  테스트용 (가장 가까운 적 찾기 로직으로 바꿔야 함)
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
            {
                motor.SetLockOnTarget(enemy.transform);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (motor == null) return;

        if (col.gameObject.CompareTag("ground"))
        {
            motor.NotifyLanded();
        }
    }
}