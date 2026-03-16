using System.Collections;
using UnityEngine;

public class DummyBrekeableTutorial2 : MonoBehaviour
{
    [Header("Golpes para caer")]
    [SerializeField] private int golpesParaCaer = 3;

    [Header("Física al caer")]
    [SerializeField] private float gravityScale = 5f;
    [SerializeField] private float fuerzaRotacion = 35f;
    [SerializeField] private float frenradoRotacion = 3f;
    [SerializeField] private float tiempoDestruirse = 4f;

    [Header("Efectos de golpe")]
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeMagnitude = 0.05f;
    [SerializeField] private float flashDuration = 0.08f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector3 posicionLocalOriginal;
    private int golpesRecibidos = 0;
    private bool cayendo = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        posicionLocalOriginal = transform.localPosition;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0f;
        }
    }

    public void ReceiveHit(float daño = 200f)
    {
        if (cayendo) return;

        transform.localPosition = posicionLocalOriginal;

        Vector2 dir = Vector2.right;
        GameObject pj = GameObject.FindGameObjectWithTag("PJ");
        if (pj != null)
            dir = ((Vector2)(transform.position - pj.transform.position)).normalized;
        ControladorCamara.Instance?.ShakeGolpe(dir);

        golpesRecibidos++;

        StopAllCoroutines();
        StartCoroutine(EfectoGolpe());

        if (golpesRecibidos >= golpesParaCaer)
            StartCoroutine(Caer());
    }

    private IEnumerator EfectoGolpe()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.red;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = posicionLocalOriginal + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = posicionLocalOriginal;
        yield return new WaitForSeconds(flashDuration);
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
    }

    private IEnumerator Caer()
    {
        cayendo = true;

        // Esperar a que termine el flash antes de caer
        yield return new WaitForSeconds(shakeDuration + flashDuration + 0.05f);

        transform.localPosition = posicionLocalOriginal;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = gravityScale;
            rb.angularDrag = frenradoRotacion;
            rb.angularVelocity = Random.Range(-fuerzaRotacion, fuerzaRotacion);
        }

        Destroy(gameObject, tiempoDestruirse);
    }
}
