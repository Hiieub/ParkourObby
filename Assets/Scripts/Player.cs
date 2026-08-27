using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private float laneChangeSpeed = 10f;
    [SerializeField] private float swipeThreshold = 50f;

    private int currentLane = 1;
    private Vector3 targetPosition;

    private Vector2 touchStartPos;
    private bool isSwiping;

    private bool isAlive = true;
    public float MoveSpeed => moveSpeed;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isAlive || !GameManager.Instance.HasStarted) return;

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        HandleKeyboardInput();
        HandleTouchInput();

        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(transform.position.x, targetPosition.x, laneChangeSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLane(-1);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLane(1);
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = touch.position;
                isSwiping = true;
                break;
            case TouchPhase.Moved:
                if (!isSwiping) return;
                isSwiping = false;

                float deltaX = touch.position.x - touchStartPos.x;
                if (Mathf.Abs(deltaX) < swipeThreshold) return;
                MoveLane(deltaX > 0 ? 1 : -1);

                break;
        }
    }

    private void MoveLane(int direction)
    {
        int targetLane = currentLane + direction;

        if (targetLane < 0 || targetLane > 2) return;

        currentLane = targetLane;

        targetPosition.x = (currentLane - 1) * laneDistance;
    }

    public void Die()
    {
        isAlive = false;

        GameManager.Instance.OnPlayerDied();
        //GameManager.Instance.GameOver();
    }

    public void IncreaseSpeed(float amount)
    {
        moveSpeed += amount;
    }
}