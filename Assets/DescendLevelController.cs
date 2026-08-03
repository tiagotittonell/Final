using UnityEngine;

public class DescendLevelController : MonoBehaviour
{
    [Header("Placa de destrucción")]
    [SerializeField] private GameObject placaDestruccion;
    [SerializeField] private Collider2D colliderPlaca;
    [SerializeField, Min(0f)] private float velocidadDeBajada = 1f;

    [Header("Vibración continua")]
    [SerializeField] private Transform objetoVisualPlaca;
    [SerializeField] private bool vibracionActiva = true;
    [SerializeField, Min(0f)] private float intensidadVibracion = 0.035f;
    [SerializeField, Min(0f)] private float frecuenciaVibracion = 30f;

    [Header("Estado")]
    [SerializeField] private bool placaEnMovimiento = true;

    private readonly Collider2D[] contactos = new Collider2D[16];
    private ContactFilter2D filtroContactos;
    private bool jugadorAplastado;
    private Vector3 posicionLocalVisualInicial;
    private float intensidadTemporal = -1f;

    private void Awake()
    {
        if (placaDestruccion != null && colliderPlaca == null)
            colliderPlaca = placaDestruccion.GetComponentInChildren<Collider2D>();

        if (objetoVisualPlaca == null && placaDestruccion != null)
        {
            SpriteRenderer[] sprites = placaDestruccion.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sprite in sprites)
            {
                if (sprite.transform == placaDestruccion.transform)
                    continue;

                objetoVisualPlaca = sprite.transform;
                break;
            }
        }

        if (objetoVisualPlaca != null && placaDestruccion != null &&
            objetoVisualPlaca == placaDestruccion.transform)
        {
            Debug.LogWarning(
                "[DescendLevelController] Objeto Visual Placa debe ser un hijo, no la raíz que se desplaza.",
                this);
            objetoVisualPlaca = null;
        }

        if (objetoVisualPlaca != null)
            posicionLocalVisualInicial = objetoVisualPlaca.localPosition;
        else
            Debug.LogWarning(
                "[DescendLevelController] Asigná un hijo visual de la placa para aplicar la vibración continua sin mover su collider.",
                this);

        filtroContactos = new ContactFilter2D();
        filtroContactos.NoFilter();
    }

    private void FixedUpdate()
    {
        if (placaDestruccion == null)
            return;

        if (placaEnMovimiento)
        {
            placaDestruccion.transform.position +=
                Vector3.down * velocidadDeBajada * Time.fixedDeltaTime;
        }

        ComprobarJugadorAplastado();
    }

    private void LateUpdate()
    {
        if (objetoVisualPlaca == null)
            return;

        if (!vibracionActiva)
        {
            objetoVisualPlaca.localPosition = posicionLocalVisualInicial;
            return;
        }

        float intensidadActual = intensidadTemporal >= 0f
            ? intensidadTemporal
            : intensidadVibracion;
        float pulso = Mathf.Abs(Mathf.Sin(Time.time * frecuenciaVibracion));
        Vector2 desplazamiento = Random.insideUnitCircle * intensidadActual * pulso;

        objetoVisualPlaca.localPosition = posicionLocalVisualInicial +
            new Vector3(desplazamiento.x, desplazamiento.y, 0f);
    }

    private void ComprobarJugadorAplastado()
    {
        if (jugadorAplastado || colliderPlaca == null)
            return;

        int cantidadContactos = colliderPlaca.OverlapCollider(filtroContactos, contactos);

        for (int i = 0; i < cantidadContactos; i++)
        {
            Collider2D contacto = contactos[i];
            if (contacto == null)
                continue;

            VidaPersonaje vidaPersonaje = contacto.GetComponentInParent<VidaPersonaje>();
            if (vidaPersonaje == null)
                continue;

            jugadorAplastado = true;
            vidaPersonaje.MatarInstantaneamente(placaDestruccion);
            break;
        }
    }

    public void IniciarDescenso()
    {
        placaEnMovimiento = true;
    }

    public void DetenerDescenso()
    {
        placaEnMovimiento = false;
    }

    public void CambiarVelocidad(float nuevaVelocidad)
    {
        velocidadDeBajada = Mathf.Max(0f, nuevaVelocidad);
    }

    public void EstablecerIntensidadVibracionTemporal(float nuevaIntensidad)
    {
        intensidadTemporal = Mathf.Max(0f, nuevaIntensidad);
    }

    public void RestaurarIntensidadVibracion()
    {
        intensidadTemporal = -1f;
    }

    private void OnDisable()
    {
        if (objetoVisualPlaca != null)
            objetoVisualPlaca.localPosition = posicionLocalVisualInicial;
    }
}
