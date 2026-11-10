using System.Collections;
using Unity.Cinemachine;
using TMPro;
using UnityEngine;

public class Palanca : MonoBehaviour
{
    [Header("Palanca")]
    [SerializeField] private Animator palancaAnimator;
    [SerializeField] private string parametroAnimacion = "PalancaActivada";
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.F;
    [SerializeField] private bool soloUnaActivacion = true;

    [Header("Puertas")]
    [SerializeField] private GameObject puertaCerrada;
    [SerializeField] private GameObject puertaAbierta;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCameraBase camaraJugador;
    [SerializeField] private CinemachineVirtualCameraBase camaraPuerta;
    [SerializeField] private int prioridadCamaraPuerta = 20;

    [Header("Tiempos de la cinemática")]
    [SerializeField, Min(0f)] private float demoraAntesDeMostrar = 0.35f;
    [SerializeField, Min(0f)] private float duracionBlend = 1.25f;
    [SerializeField, Min(0f)] private float demoraAntesDeCambiarPuerta = 0.35f;
    [SerializeField, Min(0f)] private float tiempoMostrandoPuerta = 1.25f;

    [Header("Jugador")]
    [SerializeField] private MovimientoPersonaje movimientoJugador;
    [SerializeField] private string tagJugador = "PJ";

    [Header("Interfaz")]
    [SerializeField] private TextMeshProUGUI textoInteraccion;

    private bool jugadorCerca;
    private bool palancaActivada;
    private bool ejecutandoCinematica;
    private int prioridadOriginalCamaraPuerta;
    private Coroutine rutinaCinematica;

    private void Awake()
    {
        if (palancaAnimator == null)
            palancaAnimator = GetComponentInChildren<Animator>();

        if (camaraPuerta != null)
            prioridadOriginalCamaraPuerta = camaraPuerta.Priority;

        if (textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!jugadorCerca || ejecutandoCinematica)
            return;

        if (soloUnaActivacion && palancaActivada)
            return;

        if (Input.GetKeyDown(teclaInteraccion))
            Interactuar();
    }

    private void Interactuar()
    {
        if (!ValidarReferencias())
            return;

        palancaActivada = !palancaActivada;

        if (palancaAnimator != null && !string.IsNullOrWhiteSpace(parametroAnimacion))
            palancaAnimator.SetBool(parametroAnimacion, palancaActivada);

        if (textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(false);

        rutinaCinematica = StartCoroutine(MostrarCambioDePuerta());
    }

    private IEnumerator MostrarCambioDePuerta()
    {
        ejecutandoCinematica = true;
        BloquearJugador(true);

        yield return new WaitForSeconds(demoraAntesDeMostrar);

        camaraPuerta.Priority = Mathf.Max(
            prioridadCamaraPuerta,
            camaraJugador.Priority + 1);

        yield return new WaitForSeconds(duracionBlend);
        yield return new WaitForSeconds(demoraAntesDeCambiarPuerta);

        AplicarEstadoPuerta();

        yield return new WaitForSeconds(tiempoMostrandoPuerta);

        camaraPuerta.Priority = prioridadOriginalCamaraPuerta;
        yield return new WaitForSeconds(duracionBlend);

        BloquearJugador(false);
        ejecutandoCinematica = false;
        rutinaCinematica = null;

        if (jugadorCerca && (!soloUnaActivacion || !palancaActivada) && textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(true);
    }

    private void AplicarEstadoPuerta()
    {
        if (puertaCerrada != null)
            puertaCerrada.SetActive(!palancaActivada);

        if (puertaAbierta != null)
            puertaAbierta.SetActive(palancaActivada);
    }

    private void BloquearJugador(bool bloquear)
    {
        if (movimientoJugador == null)
            return;

        movimientoJugador.sePuedeMover = !bloquear;

        if (bloquear)
            movimientoJugador.DetenerMovimientoCinematico();
    }

    private bool ValidarReferencias()
    {
        if (camaraJugador == null || camaraPuerta == null)
        {
            Debug.LogError("[Palanca] Faltan Camara Jugador o Camara Puerta.", this);
            return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagJugador))
            return;

        jugadorCerca = true;

        if (movimientoJugador == null)
            movimientoJugador = other.GetComponentInParent<MovimientoPersonaje>();

        if ((!soloUnaActivacion || !palancaActivada) && !ejecutandoCinematica && textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(tagJugador))
            return;

        jugadorCerca = false;

        if (textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (rutinaCinematica != null)
            StopCoroutine(rutinaCinematica);

        if (camaraPuerta != null)
            camaraPuerta.Priority = prioridadOriginalCamaraPuerta;

        BloquearJugador(false);
        ejecutandoCinematica = false;
        rutinaCinematica = null;
    }
}
