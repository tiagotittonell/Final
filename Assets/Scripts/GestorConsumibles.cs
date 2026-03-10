using UnityEngine;
using UnityEngine.UI;

public class GestorConsumibles : MonoBehaviour
{
    public static GestorConsumibles Instance { get; private set; }

    public event System.Action OnConsumibleAgregado;

    [SerializeField] private Image[] iconosConsumibles;
    [SerializeField] private float vidaPorConsumible = 150f;

    private int cantidad = 0;
    private const int maxConsumibles = 5;
    private VidaPersonaje vidaPersonaje;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        cantidad = GestorJugador.consumiblesGuardados;
        vidaPersonaje = FindObjectOfType<VidaPersonaje>();
        ActualizarUI();
    }

    public bool AgregarConsumible()
    {
        if (cantidad >= maxConsumibles) return false;
        cantidad++;
        ActualizarUI();
        OnConsumibleAgregado?.Invoke();
        return true;
    }

    public void UsarConsumible()
    {
        if (cantidad <= 0 || vidaPersonaje == null) return;
        cantidad--;
        vidaPersonaje.Curar(vidaPorConsumible);
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        for (int i = 0; i < iconosConsumibles.Length; i++)
            if (iconosConsumibles[i] != null)
                iconosConsumibles[i].gameObject.SetActive(i < cantidad);
    }

    private void OnDestroy()
    {
        GestorJugador.consumiblesGuardados = cantidad;
    }
}
