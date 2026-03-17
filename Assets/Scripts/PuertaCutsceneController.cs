using System.Collections;
using UnityEngine;
using Cinemachine;

public class PuertaCutsceneController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera camJugador;
    [SerializeField] private CinemachineVirtualCamera camPuerta;

    [Header("Tiempos (segundos)")]
    [Tooltip("Debe coincidir con el blend time del CinemachineBrain")]
    [SerializeField] private float tiempoBlend    = 1.5f;
    [Tooltip("Pausa con la cámara en la puerta antes de volver")]
    [SerializeField] private float tiempoEnPuerta = 1.2f;
    [Tooltip("Espera antes de que la cámara empiece a moverse")]
    [SerializeField] private float tiempoEspera   = 0.5f;

    [Header("Puerta")]
    [SerializeField] private GameObject puertaCerrada;
    [SerializeField] private GameObject puertaAbierta;

    [Header("Personaje")]
    [SerializeField] private MovimientoPersonaje movimiento;


    public void IniciarCutscene()
    {
        Debug.Log("[PuertaCutscene] IniciarCutscene llamado");

        if (camJugador == null) { Debug.LogError("[PuertaCutscene] camJugador no asignada");  return; }
        if (camPuerta  == null) { Debug.LogError("[PuertaCutscene] camPuerta no asignada");   return; }
        if (movimiento == null) { Debug.LogError("[PuertaCutscene] movimiento no asignado");  return; }

        StartCoroutine(Cutscene());
    }

    private IEnumerator Cutscene()
    {
        movimiento.sePuedeMover = false;
        Debug.Log("[PuertaCutscene] Jugador bloqueado — deslizando hacia puerta");

        yield return new WaitForSeconds(tiempoEspera);

        camPuerta.Priority = camJugador.Priority + 10;
        yield return new WaitForSeconds(tiempoBlend);

        Debug.Log("[PuertaCutscene] Cambiando sprites de puerta");
        if (puertaCerrada != null) puertaCerrada.SetActive(false);
        else Debug.LogWarning("[PuertaCutscene] puertaCerrada no asignada");
        if (puertaAbierta  != null) puertaAbierta.SetActive(true);
        else Debug.LogWarning("[PuertaCutscene] puertaAbierta no asignada");

        yield return new WaitForSeconds(tiempoEnPuerta);

        Debug.Log("[PuertaCutscene] Volviendo al jugador");
        camPuerta.Priority = 0;
        yield return new WaitForSeconds(tiempoBlend);

        movimiento.sePuedeMover = true;
        Debug.Log("[PuertaCutscene] Cinemática terminada");
    }
}
