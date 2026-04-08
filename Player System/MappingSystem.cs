using UnityEngine;
using UnityEngine.InputSystem;

public class MappingSystem : MonoBehaviour
{
    private InputActionMap map;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction dodgeAction;
    private InputAction interactAction;

    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }

    public bool RunHeld { get; private set; }
    public bool JumpDown { get; private set; }
    public bool DodgeDown { get; private set; }
    public bool InteractDown { get; private set; }

    public bool AttackDown { get; private set; }
    public bool SkillDown { get; private set; }

    private void Awake()
    {
        BuildPlayerMap();
    }

    private void OnEnable() => map.Enable();
    private void OnDisable() => map.Disable();

    void BuildPlayerMap()
    {
        map = new InputActionMap("Player");

        var move = map.AddAction("Move", InputActionType.Value, "", "Vector2");
        var composite = move.AddCompositeBinding("2DVector");
        composite.With("Up", "<Keyboard>/w");
        composite.With("Down", "<Keyboard>/s");
        composite.With("Left", "<Keyboard>/a");
        composite.With("Right", "<Keyboard>/d");
        move.AddBinding("<Gamepad>/leftStick");

        var look = map.AddAction("Look", InputActionType.Value, "", "Vector2");
        look.AddBinding("<Mouse>/delta");
        look.AddBinding("<Gamepad>/rightStick");

        var run = map.AddAction("Run", InputActionType.Button);
        run.AddBinding("<Keyboard>/leftShift");

        var jump = map.AddAction("Jump", InputActionType.Button);
        jump.AddBinding("<Keyboard>/space");

        var dodge = map.AddAction("Dodge", InputActionType.Button);
        dodge.AddBinding("<Keyboard>/leftCtrl");

        var interact = map.AddAction("Interact", InputActionType.Button);
        interact.AddBinding("<Keyboard>/e");

        // Ä³½Ì
        moveAction = move;
        lookAction = look;
        runAction = run;
        jumpAction = jump;
        dodgeAction = dodge;
        interactAction = interact;
    }

    public void Tick()
    {
        Move = moveAction.ReadValue<Vector2>();
        Look = lookAction.ReadValue<Vector2>();

        RunHeld = runAction.IsPressed();

        JumpDown = jumpAction.WasPressedThisFrame();
        DodgeDown = dodgeAction.WasPressedThisFrame();
        InteractDown = interactAction.WasPressedThisFrame();

        AttackDown = false;
        SkillDown = false;
    }
}