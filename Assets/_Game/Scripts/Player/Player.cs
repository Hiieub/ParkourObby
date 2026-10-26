using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private Transform mainCamera;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8.5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Jump & Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private int maxJump = 2;
    
    [Header("Parkour Mechanics")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Visuals & Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem runParticles;

    [Header("Skin Configuration")]
    [SerializeField] private SkinDatabaseSO skinDatabase;
    [SerializeField] private Transform modelRoot;

    private GameObject currentSkin;

    private int jumpCount;
    private Vector3 velocity;
    
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (runParticles == null)
        {
            runParticles = GetComponentInChildren<ParticleSystem>();
        }
    }

    private void Start()
    {
        LoadSavedSkin();

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnJumpEvent += OnJumpInput;
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
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
        {
            if (runParticles != null && runParticles.isPlaying)
            {
                runParticles.Stop();
            }
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
            return;
        }

        MovePlayer();
        HandleJumpAndGravity();
    }

    private void MovePlayer()
    {
        if (GameInput.Instance == null) return;

        Vector2 input = GameInput.Instance.MoveInput;
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        bool isMoving = direction.sqrMagnitude > 0.01f;
        bool isSprinting = GameInput.Instance.IsSprinting && isMoving;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.speed = isSprinting ? 1.5f : 1.0f;
        }

        if (runParticles != null)
        {
            bool shouldEmit = controller.isGrounded && isMoving;
            if (shouldEmit && !runParticles.isPlaying)
            {
                runParticles.Play();
            }
            else if (!shouldEmit && runParticles.isPlaying)
            {
                runParticles.Stop();
            }
        }

        if (!isMoving) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + (mainCamera != null ? mainCamera.eulerAngles.y : 0f);

        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            turnSmoothTime
        );

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
    }

    private void OnJumpInput()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;
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

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayJump();
                }

                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void ApplyBounce(float bounceForce)
    {
        velocity.y = bounceForce;
        jumpCount = 0;
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
    }

    // Tác dụng lực đè xuống vật thể khi Player đứng lên cầu bấp bênh
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        if (hit.moveDirection.y < -0.3f)
        {
            Vector3 pushForce = Vector3.down * 120f;
            rb.AddForceAtPosition(pushForce, hit.point, ForceMode.Force);
        }
    }

    public void Respawn()
    {
        velocity = Vector3.zero;
        jumpCount = 0;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReSpawnPlayer(this);
        }
    }

    public void SetStartPos(Vector3 newPos, Quaternion newRotation)
    {
        velocity = Vector3.zero;
        jumpCount = 0;

        if (controller != null)
        {
            controller.enabled = false;
            transform.position = newPos;
            transform.rotation = newRotation;
            controller.enabled = true;
        }
        else
        {
            transform.position = newPos;
            transform.rotation = newRotation;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCheckpoint(newPos);
        }
    }

    public void ChangeSkin(GameObject skinPrefab)
    {
        if (skinPrefab == null) return;

        if (currentSkin != null)
        {
            Destroy(currentSkin);
        }
        else if (modelRoot  != null && modelRoot.childCount > 0)
        {
            for (int i = modelRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(modelRoot.GetChild(i).gameObject);
            }
        }

        Transform parent = modelRoot != null ? modelRoot : transform;
        currentSkin = Instantiate(skinPrefab, parent.position, parent.rotation, parent);

        animator = currentSkin.GetComponentInChildren<Animator>();
    }

    public void LoadSavedSkin()
    {
        if (skinDatabase == null) return;

        string savedSkinId = DataManager.SelectedSkin;
        SkinData skinData = skinDatabase.GetSkinById(savedSkinId);

        if (skinData != null && skinData.skinPrefab != null)
        {
            ChangeSkin(skinData.skinPrefab);
        }
    }
}