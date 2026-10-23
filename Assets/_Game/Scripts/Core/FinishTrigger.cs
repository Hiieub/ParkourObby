using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FinishTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem confettiFx;
    private bool hasFinished = false;

    private void Reset()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasFinished) return;

        if (other.TryGetComponent(out Player _) || other.GetComponentInParent<Player>() != null)
        {
            TriggerFinish();
        }
    }

    public void TriggerFinish()
    {
        if (hasFinished) return;
        hasFinished = true;

        if (confettiFx != null)
        {
            confettiFx.Play();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelFinished();
        }
    }
}