using System;
using UnityEngine;
using UnityEngine.UI;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private InputActions inputActions;

    [SerializeField] private Joystick joystick;

    [SerializeField] private Button btnJump;
    [SerializeField] private Button btnSprint;

    public event Action OnJumpEvent;

    private bool isSprintToggled = false;

    public bool IsSprinting
    {
        get
        {
            bool keyboardSprint = false;
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                keyboardSprint = UnityEngine.InputSystem.Keyboard.current.leftShiftKey.isPressed ||
                                 UnityEngine.InputSystem.Keyboard.current.rightShiftKey.isPressed;
            }
            return isSprintToggled || keyboardSprint;
        }
    }

    public Vector2 MoveInput
    {
        get
        {
            Vector2 keyboardInput = Vector2.zero;
            if (inputActions != null)
            {
                keyboardInput = inputActions.Player.Move.ReadValue<Vector2>();
            }

            if (joystick != null && joystick.Direction.sqrMagnitude > 0.01f)
            {
                return joystick.Direction.normalized;
            }

            return keyboardInput.normalized;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        inputActions = new InputActions();
    }

    private void Start()
    {
        inputActions.Player.Jump.performed += Context => OnJumpEvent?.Invoke();

        if (btnJump != null)
        {
            btnJump.onClick.AddListener(() => OnJumpEvent?.Invoke());
        }

        if (btnSprint != null)
        {
            btnSprint.onClick.AddListener(ToggleSprint);
        }
    }

    private void ToggleSprint()
    {
        isSprintToggled = !isSprintToggled;
    }

    private void OnEnable()
    {
        inputActions?.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (inputActions == null) return;

        inputActions?.Dispose();
    }
}
