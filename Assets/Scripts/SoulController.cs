using System.Collections;
using UnityEngine;

public class SoulController : MonoBehaviour
{
    [Header("Flotación")]
    [SerializeField] private float amplitudY = 0.2f;
    [SerializeField] private float velocidadY = 1.5f;
    [SerializeField] private float amplitudX = 0.06f;
    [SerializeField] private float velocidadX = 0.9f;

    [Header("Desaparición")]
    [SerializeField] private float duracionFade = 0.6f;

    private Vector3 posicionBase;
    private Vector3 escalaInicial;
    private SpriteRenderer sr;
    private float desfase;
    private bool desapareciendo = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        escalaInicial = transform.localScale;
        desfase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Start()
    {
        posicionBase = transform.position;
    }

    private void Update()
    {
        if (desapareciendo) return;

        float t = Time.time + desfase;
        transform.position = posicionBase + new Vector3(
            Mathf.Sin(t * velocidadX) * amplitudX,
            Mathf.Sin(t * velocidadY) * amplitudY,
            0f
        );
    }

    public void Desaparecer()
    {
        if (desapareciendo) return;
        desapareciendo = true;
        StartCoroutine(FadeYDestruir());
    }

    private IEnumerator FadeYDestruir()
    {
        float elapsed = 0f;
        Color colorInicial = sr != null ? sr.color : Color.white;

        while (elapsed < duracionFade)
        {
            elapsed += Time.deltaTime;
            float progreso = elapsed / duracionFade;
            if (sr != null)
                sr.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, Mathf.Lerp(1f, 0f, progreso));
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, progreso);
            yield return null;
        }

        Destroy(gameObject);
    }
}
