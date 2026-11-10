using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CamerasControllerDescend : MonoBehaviour
{
    [Header("Cámaras")]
    [SerializeField] private CinemachineVirtualCameraBase camaraJugador;
    [SerializeField] private CinemachineVirtualCameraBase camaraRecorrido;
    [SerializeField] private int prioridadRecorrido = 20;

    [Header("Referencias de la escena")]
    [SerializeField] private Transform jugador;
    [SerializeField] private MovimientoPersonaje movimientoJugador;
    [SerializeField] private Transform[] palancas = new Transform[3];
    [SerializeField] private Transform placaDestruccion;
    [SerializeField] private DescendLevelController controladorDescenso;

    [Header("Recorrido")]
    [SerializeField, Min(0.01f)] private float duracionMovimiento = 1.25f;
    [SerializeField, Min(0f)] private float tiempoEnCadaPalanca = 0.8f;
    [SerializeField, Min(0f)] private float tiempoEnPlacaAntesShake = 0.5f;
    [SerializeField, Min(0f)] private float tiempoAlVolverJugador = 0.5f;
    [SerializeField] private bool ordenarPalancasDeArribaHaciaAbajo = true;

    [Header("Encuadre")]
    [SerializeField] private Vector3 offsetPalancas;
    [SerializeField] private Vector3 offsetPlaca;
    [SerializeField] private Vector3 offsetJugador;

    [Header("Activación de la placa")]
    [SerializeField, Min(0f)] private float duracionVibracion = 1.5f;
    [SerializeField, Min(0f)] private float intensidadVibracion = 0.12f;

    [Header("Entrada del jugador")]
    [SerializeField, Min(0f)] private float distanciaEntrada = 2f;
    [SerializeField, Min(0.01f)] private float velocidadEntrada = 4f;
    [SerializeField, Min(0f)] private float duracionFrenado = 0.25f;
    [SerializeField, Min(0f)] private float tiempoObservandoEnIdle = 0.7f;

    [Header("Letterboxing")]
    [SerializeField, Min(0f)] private float alturaLetterbox = 90f;
    [SerializeField, Min(0.01f)] private float duracionEntradaLetterbox = 0.6f;
    [SerializeField, Min(0.01f)] private float duracionSalidaLetterbox = 0.45f;
    [SerializeField] private Color colorLetterbox = Color.black;
    [SerializeField] private bool ocultarLetterboxAlFinal = true;

    [Header("Inicio")]
    [SerializeField] private bool reproducirAlIniciar = true;

    private Transform objetivoRecorrido;
    private Coroutine rutinaPresentacion;
    private bool presentacionTerminada;
    private CanvasGroup canvasGroupLetterbox;
    private RectTransform barraSuperior;
    private RectTransform barraInferior;

    public bool PresentacionTerminada => presentacionTerminada;

    private void Awake()
    {
        if (jugador != null && movimientoJugador == null)
            movimientoJugador = jugador.GetComponent<MovimientoPersonaje>();

        if (controladorDescenso == null)
            controladorDescenso = FindObjectOfType<DescendLevelController>();

        CrearObjetivoDeRecorrido();
        CrearLetterbox();

        if (controladorDescenso != null)
            controladorDescenso.DetenerDescenso();
    }

    private void Start()
    {
        if (reproducirAlIniciar)
            IniciarPresentacion();
    }

    public void IniciarPresentacion()
    {
        if (rutinaPresentacion != null || presentacionTerminada)
            return;

        if (!ValidarReferencias())
            return;

        rutinaPresentacion = StartCoroutine(SecuenciaPresentacion());
    }

    private IEnumerator SecuenciaPresentacion()
    {
        BloquearJugador(true);
        controladorDescenso.DetenerDescenso();

        yield return EntradaAutomaticaJugador();
        yield return new WaitForSeconds(tiempoObservandoEnIdle);
        yield return AnimarLetterbox(true, duracionEntradaLetterbox);

        objetivoRecorrido.position = jugador.position + offsetJugador;
        camaraRecorrido.Follow = objetivoRecorrido;
        camaraRecorrido.Priority = Mathf.Max(prioridadRecorrido, camaraJugador.Priority + 1);

        List<Transform> palancasOrdenadas = ObtenerPalancasValidas();

        foreach (Transform palanca in palancasOrdenadas)
        {
            yield return MoverObjetivo(palanca.position + offsetPalancas);
            yield return new WaitForSeconds(tiempoEnCadaPalanca);
        }

        yield return MoverObjetivo(placaDestruccion.position + offsetPlaca);
        yield return new WaitForSeconds(tiempoEnPlacaAntesShake);
        yield return VibrarPlaca();

        yield return MoverObjetivo(jugador.position + offsetJugador);
        yield return new WaitForSeconds(tiempoAlVolverJugador);

        camaraRecorrido.Priority = camaraJugador.Priority - 1;

        if (ocultarLetterboxAlFinal)
            yield return AnimarLetterbox(false, duracionSalidaLetterbox);

        BloquearJugador(false);
        controladorDescenso.IniciarDescenso();

        presentacionTerminada = true;
        rutinaPresentacion = null;
    }

    private IEnumerator EntradaAutomaticaJugador()
    {
        if (movimientoJugador == null || distanciaEntrada <= 0f)
            yield break;

        float posicionInicialX = jugador.position.x;
        float posicionFinalX = posicionInicialX + distanciaEntrada;
        float tiempoLimite = distanciaEntrada / velocidadEntrada + 1f;
        float tiempo = 0f;

        while (jugador.position.x < posicionFinalX && tiempo < tiempoLimite)
        {
            movimientoJugador.EstablecerVelocidadCinematica(velocidadEntrada);
            tiempo += Time.deltaTime;
            yield return null;
        }

        if (duracionFrenado > 0f)
        {
            float velocidadActual = velocidadEntrada;
            float tiempoFrenando = 0f;

            while (tiempoFrenando < duracionFrenado)
            {
                tiempoFrenando += Time.deltaTime;
                velocidadActual = Mathf.Lerp(
                    velocidadEntrada,
                    0f,
                    Mathf.Clamp01(tiempoFrenando / duracionFrenado));
                movimientoJugador.EstablecerVelocidadCinematica(velocidadActual);
                yield return null;
            }
        }

        movimientoJugador.DetenerMovimientoCinematico();
    }

    private IEnumerator MoverObjetivo(Vector3 destino)
    {
        Vector3 origen = objetivoRecorrido.position;
        float tiempo = 0f;

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracionMovimiento);
            float suavizado = progreso * progreso * (3f - 2f * progreso);
            objetivoRecorrido.position = Vector3.LerpUnclamped(origen, destino, suavizado);
            yield return null;
        }

        objetivoRecorrido.position = destino;
    }

    private IEnumerator VibrarPlaca()
    {
        controladorDescenso.EstablecerIntensidadVibracionTemporal(intensidadVibracion);
        yield return new WaitForSeconds(duracionVibracion);
        controladorDescenso.RestaurarIntensidadVibracion();
    }

    private List<Transform> ObtenerPalancasValidas()
    {
        List<Transform> resultado = new List<Transform>();

        if (palancas != null)
        {
            foreach (Transform palanca in palancas)
            {
                if (palanca != null)
                    resultado.Add(palanca);
            }
        }

        if (ordenarPalancasDeArribaHaciaAbajo)
            resultado.Sort((a, b) => b.position.y.CompareTo(a.position.y));

        return resultado;
    }

    private void BloquearJugador(bool bloquear)
    {
        if (movimientoJugador != null)
            movimientoJugador.sePuedeMover = !bloquear;

        if (bloquear && jugador != null)
        {
            Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
                rbJugador.linearVelocity = Vector2.zero;
        }
    }

    private void CrearObjetivoDeRecorrido()
    {
        GameObject objetivo = new GameObject("_ObjetivoCamaraDescend");
        objetivo.transform.SetParent(transform, false);
        objetivoRecorrido = objetivo.transform;

        if (jugador != null)
            objetivoRecorrido.position = jugador.position + offsetJugador;
    }

    private void CrearLetterbox()
    {
        GameObject canvasGO = new GameObject("_LetterboxDescend");
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGroupLetterbox = canvasGO.AddComponent<CanvasGroup>();
        canvasGroupLetterbox.alpha = 0f;
        canvasGroupLetterbox.blocksRaycasts = false;
        canvasGroupLetterbox.interactable = false;

        barraSuperior = CrearBarraLetterbox(canvasGO.transform, "BarraSuperior", true);
        barraInferior = CrearBarraLetterbox(canvasGO.transform, "BarraInferior", false);
        EstablecerLetterbox(0f);
    }

    private RectTransform CrearBarraLetterbox(Transform padre, string nombre, bool superior)
    {
        GameObject barraGO = new GameObject(nombre);
        barraGO.transform.SetParent(padre, false);

        Image imagen = barraGO.AddComponent<Image>();
        imagen.color = colorLetterbox;
        imagen.raycastTarget = false;

        RectTransform rect = barraGO.GetComponent<RectTransform>();
        rect.anchorMin = superior ? new Vector2(0f, 1f) : new Vector2(0f, 0f);
        rect.anchorMax = superior ? Vector2.one : new Vector2(1f, 0f);
        rect.pivot = superior ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        return rect;
    }

    private IEnumerator AnimarLetterbox(bool mostrar, float duracion)
    {
        float inicio = mostrar ? 0f : 1f;
        float final = mostrar ? 1f : 0f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracion);
            float suavizado = progreso * progreso * (3f - 2f * progreso);
            EstablecerLetterbox(Mathf.Lerp(inicio, final, suavizado));
            yield return null;
        }

        EstablecerLetterbox(final);
    }

    private void EstablecerLetterbox(float progreso)
    {
        if (canvasGroupLetterbox == null)
            return;

        canvasGroupLetterbox.alpha = progreso;
        float alturaActual = alturaLetterbox * progreso;

        if (barraSuperior != null)
            barraSuperior.sizeDelta = new Vector2(0f, alturaActual);

        if (barraInferior != null)
            barraInferior.sizeDelta = new Vector2(0f, alturaActual);
    }

    private bool ValidarReferencias()
    {
        bool valido = true;

        if (camaraJugador == null)
        {
            Debug.LogError("[CamerasControllerDescend] Falta asignar Camara Jugador.", this);
            valido = false;
        }

        if (camaraRecorrido == null)
        {
            Debug.LogError("[CamerasControllerDescend] Falta asignar Camara Recorrido.", this);
            valido = false;
        }

        if (jugador == null)
        {
            Debug.LogError("[CamerasControllerDescend] Falta asignar Jugador.", this);
            valido = false;
        }

        if (placaDestruccion == null)
        {
            Debug.LogError("[CamerasControllerDescend] Falta asignar la placa de destrucción.", this);
            valido = false;
        }

        if (controladorDescenso == null)
        {
            Debug.LogError("[CamerasControllerDescend] Falta asignar DescendLevelController.", this);
            valido = false;
        }

        if (ObtenerPalancasValidas().Count == 0)
        {
            Debug.LogError("[CamerasControllerDescend] No hay palancas asignadas.", this);
            valido = false;
        }

        return valido;
    }

    private void OnDisable()
    {
        if (controladorDescenso != null)
            controladorDescenso.RestaurarIntensidadVibracion();
    }

}
