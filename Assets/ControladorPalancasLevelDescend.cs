using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControladorPalancasLevelDescend : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField, Min(1)] private int palancasNecesarias = 3;

    [Header("Puerta")]
    [SerializeField] private DoorController puerta;
    [SerializeField] private Animator animatorPuerta;
    [SerializeField] private string estadoAnimacionApertura = "OpenDoorLevelDescend";
    [SerializeField, Min(0f)] private float transicionAnimacionPuerta = 0.05f;
    [SerializeField] private Collider2D colliderBloqueoPuerta;
    [SerializeField] private PuertaLevelDescend salidaSiguienteNivel;

    [Header("Eventos opcionales")]
    [SerializeField] private UnityEvent alActivarTodasLasPalancas;

    private readonly HashSet<PalancaLevelDescend> palancasActivadas =
        new HashSet<PalancaLevelDescend>();

    private bool completado;

    public int CantidadActivadas => palancasActivadas.Count;
    public bool Completado => completado;

    private void Awake()
    {
        if (salidaSiguienteNivel == null)
            salidaSiguienteNivel = FindObjectOfType<PuertaLevelDescend>(true);
    }

    public void RegistrarPalanca(PalancaLevelDescend palanca)
    {
        if (completado || palanca == null || !palancasActivadas.Add(palanca))
            return;

        Debug.Log($"[PalancasDescend] Activadas: {CantidadActivadas}/{palancasNecesarias}", this);

        if (CantidadActivadas >= palancasNecesarias)
            CompletarPuzzle();
    }

    private void CompletarPuzzle()
    {
        if (completado)
            return;

        completado = true;

        if (puerta != null)
            puerta.OpenDoor();

        ReproducirAnimacionPuerta();

        if (colliderBloqueoPuerta != null)
            colliderBloqueoPuerta.enabled = false;

        if (salidaSiguienteNivel != null)
            salidaSiguienteNivel.HabilitarSalida();
        else
            Debug.LogWarning("[PalancasDescend] No hay una salida al siguiente nivel asignada.", this);

        alActivarTodasLasPalancas?.Invoke();
    }

    private void ReproducirAnimacionPuerta()
    {
        if (animatorPuerta == null)
        {
            if (puerta == null)
                Debug.LogWarning("[PalancasDescend] No hay puerta ni Animator de puerta asignados.", this);

            return;
        }

        if (string.IsNullOrWhiteSpace(estadoAnimacionApertura))
        {
            Debug.LogWarning("[PalancasDescend] El nombre del estado de apertura está vacío.", this);
            return;
        }

        string rutaEstado = estadoAnimacionApertura.Contains(".")
            ? estadoAnimacionApertura
            : $"Base Layer.{estadoAnimacionApertura}";
        int estadoHash = Animator.StringToHash(rutaEstado);
        if (!animatorPuerta.HasState(0, estadoHash))
        {
            Debug.LogWarning(
                $"[PalancasDescend] El Animator no contiene el estado '{estadoAnimacionApertura}' en la capa Base Layer.",
                animatorPuerta);
            return;
        }

        animatorPuerta.CrossFade(estadoHash, transicionAnimacionPuerta, 0, 0f);
    }
}
