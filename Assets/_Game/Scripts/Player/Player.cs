using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private Transform mainCamera;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Jump & Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private int maxJump = 2;
    
    [Header("Parkour Mechanics")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private int jumpCount;
    private Vector3 velocity;
    
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpEvent += OnJumpInput;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCheckpoint(transform.position);
        }
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpEvent -= OnJumpInput;
        }
    }

    private void Update()
    {
        MovePlayer();
        HandleJumpAndGravity();
    }

    private void MovePlayer()
    {
        if (GameInput.Instance == null) return;

        Vector2 input = GameInput.Instance.MoveInput;
        Vector3 direction = new Vector3(input.x, 0f, input.y);

        if (direction.sqrMagnitude < 0.01f) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            turnSmoothTime
        );

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        controller.Move(moveDirection.normalized * speed * Time.deltaTime);
    }

    private void OnJumpInput()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void HandleJumpAndGravity()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0; 
            velocity.y = -2f;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f || jumpCount < maxJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (coyoteTimeCounter > 0f)
                {
                    jumpCount = 1;
                }
                else
                {
                    jumpCount++;
                }

                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}