using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Timers")]
    [SerializeField] private float shakeDuration = 0.6f;
    [SerializeField] private float respawnDelay = 2.5f;

    [Header("Visual Feedback")]
    [SerializeField] private float shakeMagnitude = 0.08f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Collider[] colliders;
    private Renderer[] renderers;
    private bool isTriggered = false;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTriggered) return;

        if (collision.gameObject.TryGetComponent(out Player _))
        {
            StartCoroutine(FallSequence());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.TryGetComponent(out Player _))
        {
            StartCoroutine(FallSequence());
        }
    }

    private IEnumerator FallSequence()
    {
        isTriggered = true;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 offset = Random.insideUnitSphere * shakeMagnitude;
            offset.y = 0f;
            transform.position = initialPosition + offset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetPlatformActive(false);

        yield return new WaitForSeconds(respawnDelay);

        transform.position = initialPosition;
        transform.rotation = initialRotation;
        SetPlatformActive(true);

        isTriggered = false;
    }

    private void SetPlatformActive(bool active)
    {
        if (colliders != null)
        {
            foreach (var c in colliders)
            {
                if (c != null) c.enabled = active;
            }
        }

        if (renderers != null)
        {
            foreach (var r in renderers)
            {
                if (r != null) r.enabled = active;
            }
        }
    }
}
