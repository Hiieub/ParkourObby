using UnityEngine;

public class SwingingObstacle : MonoBehaviour
{
    [Header("Swing Motion")]
    [SerializeField] private float swingSpeed = 2.5f;
    [SerializeField] private float maxAngle = 55f;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    [Header("Hazard")]
    [SerializeField] private bool killsOnTouch = true;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        float angle = Mathf.Sin(Time.time * swingSpeed) * maxAngle;
        transform.localRotation = initialRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (killsOnTouch && other.TryGetComponent(out Player player))
        {
            player.Respawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (killsOnTouch && collision.gameObject.TryGetComponent(out Player player))
        {
            player.Respawn();
        }
    }
}
