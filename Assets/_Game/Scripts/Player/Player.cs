using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private Transform mainCamera;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    private int maxJump = 2;
    private int jumpCount;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpEvent += PlayerJump;
        }
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpEvent -= PlayerJump;
        }
    }

    private void Update()
    {
        MovePlayer();
        ApplyGravity();
    }

    private void MovePlayer()
    {
        if (GameInput.Instance == null) return;

        Vector2 input = GameInput.Instance.MoveInput;

        Vector3 direction = new Vector3(input.x, 0f, input.y);

        if (direction.sqrMagnitude < 0.01f) return;

        float targetAngle = Mathf.Atan2(
            direction.x,
            direction.z
        ) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;

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

    private void ApplyGravity()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;

            jumpCount = 0;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void PlayerJump()
    {
        if (jumpCount < maxJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpCount++;
        }
    }
}