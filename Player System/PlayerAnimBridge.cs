using UnityEngine;

public class PlayerAnimBridge : MonoBehaviour
{
    public Animator anim;
    public Transform characterBody;

    void Awake()
    {
        if (!anim)
            anim = GetComponentInChildren<Animator>();

        if (!characterBody)
            characterBody = anim.transform;
    }

    public void InitDefaults()
    {
        anim.SetFloat("MoveX", 0);
        anim.SetFloat("MoveY", 0);
        anim.SetBool("isJump", false);
    }

    public void SetMove(float h, float v, bool isMove, bool isRun)
    {
        anim.SetFloat("MoveX", h, 0.1f, Time.deltaTime);
        anim.SetFloat("MoveY", v, 0.1f, Time.deltaTime);

        if (!isMove)
        {
            anim.speed = 1f;
        }
        else
        {
            anim.speed = isRun ? 1.5f : 1f;
        }
    }

    public void JumpStart(float moveAmount)
    {
        anim.SetBool("isJump", true);

        if (moveAmount > 0.1f)
            anim.SetTrigger("doJumpRun");
        else
            anim.SetTrigger("doJump");
    }

    public void Land()
    {
        anim.SetBool("isJump", false);
        anim.SetTrigger("doLand");
    }

    public void Dodge(Vector3 dir)
    {
        // 핵심: 캐릭터 기준으로 변환
        Vector3 localDir = characterBody.InverseTransformDirection(dir);

        // 방향 판단
        if (Mathf.Abs(localDir.z) > Mathf.Abs(localDir.x))
        {
            if (localDir.z > 0)
                anim.SetTrigger("doDodgeFor");
            else
                anim.SetTrigger("doDodgeBack");
        }
        else
        {
            if (localDir.x > 0)
                anim.SetTrigger("doDodgeRig");
            else
                anim.SetTrigger("doDodgeLef");
        }
    }
}