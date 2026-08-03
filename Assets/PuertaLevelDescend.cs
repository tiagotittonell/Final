using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class PuertaLevelDescend : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private string nombreEscenaSiguiente;
    [SerializeField] private string tagJugador = "PJ";

    [Header("Trigger de salida")]
    [SerializeField] private BoxCollider2D triggerSalida;

    private bool salidaHabilitada;

    private void Awake()
    {
        if (triggerSalida == null)
            triggerSalida = GetComponent<BoxCollider2D>();

        triggerSalida.isTrigger = true;
        triggerSalida.enabled = false;
    }

    public void HabilitarSalida()
    {
        salidaHabilitada = true;

        if (triggerSalida != null)
            triggerSalida.enabled = true;

        Debug.Log($"[PuertaLevelDescend] Salida habilitada hacia '{nombreEscenaSiguiente}'.", this);
    }

    public void ConfigurarEscenaSiguiente(string nombreEscena)
    {
        nombreEscenaSiguiente = nombreEscena;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(
            $"[PuertaLevelDescend] Trigger detectó '{other.name}' con tag '{other.tag}'. " +
            $"Salida habilitada: {salidaHabilitada}.",
            this);

        if (!salidaHabilitada || !other.CompareTag(tagJugador))
            return;

        if (string.IsNullOrWhiteSpace(nombreEscenaSiguiente))
        {
            Debug.LogError("[PuertaLevelDescend] No se configuró el nombre de la siguiente escena.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nombreEscenaSiguiente))
        {
            Debug.LogError(
                $"[PuertaLevelDescend] La escena '{nombreEscenaSiguiente}' no existe o no fue agregada a Build Settings.",
                this);
            return;
        }

        salidaHabilitada = false;
        triggerSalida.enabled = false;
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }
}
