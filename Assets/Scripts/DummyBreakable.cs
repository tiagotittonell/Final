using System.Collections;
using UnityEngine;

public class DummyBreakable : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;

    [Header("Vida")]
    public float vida = 1200f;

    [Header("Shake")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.05f;

    [Header("Flash")]
    public float flashDuration = 0.08f;

    private bool destruido = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.localPosition;
    }

    public void ReceiveHit(float daño = 200f)
    {
        if (destruido) return;

        Vector2 dir = Vector2.right;
        GameObject pj = GameObject.FindGameObjectWithTag("PJ");
        if (pj != null)
            dir = ((Vector2)(transform.position - pj.transform.position)).normalized;
        ControladorCamara.Instance?.ShakeGolpe(dir);

        vida -= daño;

        StopAllCoroutines();
        StartCoroutine(HitEffect());

        if (vida <= 0)
        {
            destruido = true;
            StartCoroutine(DestroyDummy());
        }
    }

    private IEnumerator HitEffect()
    {
        // Flash rojo
        spriteRenderer.color = Color.red;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = Color.white;
    }

    private IEnumerator DestroyDummy()
    {
        yield return new WaitForSeconds(0.05f);

        // aca voy a agregar:
        // partículas, sonido, animación, loot, etc.

        Destroy(gameObject);
    }
}