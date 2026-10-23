using System.Collections;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 13f;
    [SerializeField] private ParticleSystem bounceFx;
    [SerializeField] private float squashAmount = 0.25f;

    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            BouncePlayer(player);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            BouncePlayer(player);
        }
    }

    private void BouncePlayer(Player player)
    {
        player.ApplyBounce(bounceForce);

        if (bounceFx != null)
        {
            bounceFx.Play();
        }

        StopAllCoroutines();
        StartCoroutine(SquashRoutine());
    }

    private IEnumerator SquashRoutine()
    {
        Vector3 squashed = new Vector3(
            initialScale.x * (1f + squashAmount),
            initialScale.y * (1f - squashAmount),
            initialScale.z * (1f + squashAmount)
        );

        transform.localScale = squashed;

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(squashed, initialScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale;
    }
}
