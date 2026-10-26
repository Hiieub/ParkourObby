using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Order")]
    [SerializeField] private int orderIndex = 0;

    public int OrderIndex
    {
        get => orderIndex;
        set => orderIndex = value;
    }

    [Header("Visual & Effects")]
    [SerializeField] private ParticleSystem activateFx;
    [SerializeField] private Renderer flagRenderer;
    [SerializeField] private Color activeColor = Color.green;

    private bool isActivated = false;

    private void Awake()
    {
        if (orderIndex <= 0)
        {
            string n = gameObject.name;
            if (transform.parent != null && (n == "Checkpoint" || !n.Contains("(")))
            {
                n = transform.parent.name;
            }

            if (n.Contains("(") && n.Contains(")"))
            {
                int start = n.IndexOf('(') + 1;
                int end = n.IndexOf(')');
                if (int.TryParse(n.Substring(start, end - start), out int parsedNum))
                {
                    orderIndex = parsedNum + 1;
                }
            }
            else
            {
                orderIndex = 1;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) || other.GetComponentInParent<Player>() != null)
        {
            Activate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player) || collision.gameObject.GetComponentInParent<Player>() != null)
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (isActivated) return;
        isActivated = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCheckpointReached(this);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCheckpoint();
        }

        if (activateFx != null)
        {
            if (flagRenderer != null)
            {
                activateFx.transform.position = flagRenderer.bounds.center + Vector3.up * flagRenderer.bounds.extents.y;
            }
            activateFx.Play();
        }

        if (flagRenderer != null)
        {
            flagRenderer.material.color = activeColor;
        }
    }

    public Vector3 GetRespawnPosition()
    {
        if (flagRenderer != null)
        {
            return new Vector3(
                flagRenderer.bounds.center.x,
                flagRenderer.bounds.max.y + 0.15f,
                flagRenderer.bounds.center.z
            );
        }

        return transform.position + Vector3.up;
    }
}
