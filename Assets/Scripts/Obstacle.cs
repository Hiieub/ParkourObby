using UnityEngine;

public class Obstacle : MonoBehaviour, IObstacle
{
    [SerializeField] private float rotateSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            OnHitPlayer(player);
        }
    }

    public void OnHitPlayer(Player player)
    {
        Debug.Log($"Va vao {transform.name}");
        player.Die();
    }
}
