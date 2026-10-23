using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private int coinValue = 1;
    [SerializeField] private GameObject collectVfx;

    private bool isCollected = false;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.TryGetComponent(out Player player))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin(coinValue);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoin();
        }

        if (collectVfx != null)
        {
            PlayCollectVfx();
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.enabled = false;
        }

        Destroy(gameObject, 0.1f);
    }

    private void PlayCollectVfx()
    {
        VfxPool.Instance.Spawn(collectVfx, transform.position, Quaternion.identity, 2f);
    }
}
