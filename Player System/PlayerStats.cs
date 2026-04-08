using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float MaxHP = 100f;          // 최대 체력
    public float MaxStamina = 100f;     // 최대 스태미나

    public float Attack = 10f;          // 공격력
    public float Defense = 0f;          // 방어력

    public float MoveSpeed = 4f;        // 기본 이동속도
    public float SprintSpeed = 6f;      // 달리기 속도
    public float JumpPower = 5f;        // 점프 힘

    public float DodgeStaminaCost = 25f; // 회피 스태미나 소모
    public float JumpStaminaCost = 10f;  // 점프 스태미나 소모

    public float StaminaRegenPerSec = 20f; // 초당 스태미나 회복량

    public float CurrentHP;             // 현재 체력
    public float CurrentStamina;        // 현재 스태미나
}
