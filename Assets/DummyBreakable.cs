using System.Collections;
using UnityEngine;

public class DummyBreakable : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;

    [Header("Shake")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.05f;

    [Header("Flash")]
    public float flashDuration = 0.08f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.localPosition;
    }

    public void ReceiveHit()
    {
        StopAllCoroutines();
        StartCoroutine(HitEffect());
    }

    private IEnumerator HitEffect()
    {
        // Flash blanco
        spriteRenderer.color = Color.red;

        // Shake
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset
        transform.localPosition = originalPosition;
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = Color.white; // puedes cambiar a otro color si quieres
    }
}