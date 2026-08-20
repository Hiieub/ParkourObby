using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private float laneChangeSpeed = 10f;

    private int currentLane = 1;
    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLane(-1);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLane(1);
        }

        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(transform.position.x, targetPosition.x, laneChangeSpeed * Time.deltaTime);
        transform.position = newPosition;
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
        GameManager.Instance.GameOver();
    }
}