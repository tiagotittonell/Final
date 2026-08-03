using System.Collections;
using UnityEngine;

public class TrheeEnemy : EnemigoBase
{
    [Header("Vida")]
    [SerializeField, Min(1f)] private float vidaMaxima = 100f;
    [SerializeField] private BarraVidaEnemigo barraVida;

    [Header("Shake al recibir daño")]
    [SerializeField, Min(0f)] private float duracionShake = 0.15f;
    [SerializeField, Min(0f)] private float intensidadShake = 0.08f;
    [SerializeField] private Transform objetoVisual;

    [Header("Muerte")]
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerMuerte = "Muerte";
    [SerializeField] private GameObject prefabEfectoMuerte;
    [SerializeField] private Transform puntoEfectoMuerte;
    [SerializeField, Min(0f)] private float demoraAntesDeDesaparecer;

    private float vidaActual;
    private bool estaMuerto;
    private bool muerteFinalizada;
    private Vector3 posicionVisualInicial;
    private Coroutine rutinaShake;

    private void Awake()
    {
        vidaActual = vidaMaxima;

        if (objetoVisual == null)
            objetoVisual = transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (puntoEfectoMuerte == null)
            puntoEfectoMuerte = transform;

        posicionVisualInicial = objetoVisual.localPosition;
    }

    private void Start()
    {
        if (barraVida != null)
            barraVida.Inicializar(vidaMaxima);
    }

    public override void TomarDaño(float daño)
    {
        if (estaMuerto || daño <= 0f)
            return;

        base.TomarDaño(daño);

        vidaActual = Mathf.Max(0f, vidaActual - daño);

        if (barraVida != null)
            barraVida.ActualizarVida(vidaActual);

        IniciarShake();

        if (vidaActual <= 0f)
            Morir();
    }

    private void IniciarShake()
    {
        if (rutinaShake != null)
            StopCoroutine(rutinaShake);

        objetoVisual.localPosition = posicionVisualInicial;
        rutinaShake = StartCoroutine(ShakeGolpe());
    }

    private IEnumerator ShakeGolpe()
    {
        float tiempo = 0f;

        while (tiempo < duracionShake)
        {
            Vector2 desplazamiento = Random.insideUnitCircle * intensidadShake;
            objetoVisual.localPosition =
                posicionVisualInicial + new Vector3(desplazamiento.x, desplazamiento.y, 0f);

            tiempo += Time.deltaTime;
            yield return null;
        }

        objetoVisual.localPosition = posicionVisualInicial;
        rutinaShake = null;
    }

    private void Morir()
    {
        if (estaMuerto)
            return;

        estaMuerto = true;

        if (rutinaShake != null)
        {
            StopCoroutine(rutinaShake);
            rutinaShake = null;
            objetoVisual.localPosition = posicionVisualInicial;
        }

        DesactivarColliders();

        if (animator != null && !string.IsNullOrWhiteSpace(triggerMuerte))
            animator.SetTrigger(triggerMuerte);

        if (demoraAntesDeDesaparecer > 0f)
            StartCoroutine(EsperarParaFinalizarMuerte());
        else
            FinalizarMuerte();
    }

    private void DesactivarColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D colliderEncontrado in colliders)
            colliderEncontrado.enabled = false;
    }

    private IEnumerator EsperarParaFinalizarMuerte()
    {
        yield return new WaitForSeconds(demoraAntesDeDesaparecer);
        FinalizarMuerte();
    }

    // También puede llamarse desde un Animation Event al final de una animación de muerte.
    public void FinalizarMuerte()
    {
        if (muerteFinalizada)
            return;

        muerteFinalizada = true;
        StopAllCoroutines();
        objetoVisual.localPosition = posicionVisualInicial;

        if (prefabEfectoMuerte != null)
        {
            Instantiate(
                prefabEfectoMuerte,
                puntoEfectoMuerte.position,
                puntoEfectoMuerte.rotation);
        }

        Destroy(gameObject);
    }
}
