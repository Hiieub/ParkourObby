using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private bool reverseDirection = false;

    private void Update()
    {
        float dir = reverseDirection ? -1f : 1f;
        transform.Rotate(rotationAxis * rotationSpeed * dir * Time.deltaTime, Space.Self);
    }
}
