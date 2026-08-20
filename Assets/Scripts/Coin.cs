using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private float rotateSpeed = 90f;
    private int score = 1;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Collect();
        }
    }

    public void Collect()
    {
        GameManager.Instance.AddScore(score);

        Destroy(gameObject);
    }
}
