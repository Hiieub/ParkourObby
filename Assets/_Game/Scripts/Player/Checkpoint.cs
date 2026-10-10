using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Vector3 respawnPos = transform.position + Vector3.up;

            GameManager.Instance.SetCheckpoint(respawnPos);
        }
    }
}
