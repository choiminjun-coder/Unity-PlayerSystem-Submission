using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input;

public class PlayerInput : MonoBehaviour, IPlayerInput
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }

    public bool RunHeld { get; private set; }

    public bool JumpDown { get; private set; }
    public bool DodgeDown { get; private set; }

    public bool AttackDown { get; private set; }
    public bool SkillDown { get; private set; }
    public bool InteractDown { get; private set; }

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public void Tick()
    {
        Move = controls.Player.Move.ReadValue<Vector2>();
        Look = controls.Player.Look.ReadValue<Vector2>();

        RunHeld = controls.Player.Run.IsPressed();

        JumpDown = controls.Player.Jump.triggered;
        DodgeDown = controls.Player.Dodge.triggered;

        AttackDown = controls.Player.Attack.triggered;
        SkillDown = controls.Player.Skill.triggered;
        InteractDown = controls.Player.Interact.triggered;
    }
}