using System;
using UnityEngine;
using UnityEngine.UI;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private InputActions inputActions;

    [SerializeField] private Joystick joystick;

    [SerializeField] private Button btnJump;

    public event Action OnJumpEvent;

    public Vector2 MoveInput
    {
        get
        {
            Vector2 keyboardInput = inputActions.Player.Move.ReadValue<Vector2>();

            if (joystick != null &&
                joystick.Direction.sqrMagnitude > 0.01f)
            {
                return joystick.Direction;
            }

            return keyboardInput;
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
