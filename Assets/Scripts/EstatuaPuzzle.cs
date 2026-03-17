using UnityEngine;

public enum TipoMedallon { Sol, Luna, Estrella }


public class EstatuaPuzzle : MonoBehaviour
{
    [Header("Medallones (un GameObject por tipo)")]
    [SerializeField] private GameObject medallon_Sol;
    [SerializeField] private GameObject medallon_Luna;
    [SerializeField] private GameObject medallon_Estrella;

    [Header("Solución")]
    [SerializeField] private TipoMedallon medallonCorrecto = TipoMedallon.Sol;

    [Header("Estado inicial")]
    [SerializeField] private TipoMedallon medallonInicial = TipoMedallon.Sol;

    [Header("UI Hint (opcional)")]
    [SerializeField] private GameObject indicadorF; 

    public TipoMedallon MedallonActual { get; private set; }
    public bool EsCorrecto => fueModificado && MedallonActual == medallonCorrecto;

    public System.Action OnMedallonCambiado;

    private bool jugadorCerca   = false;
    private bool bloqueado      = false;
    private bool fueModificado  = false;

    private void Start()
    {
        MedallonActual = medallonInicial;
        ActualizarVisuales();
        if (indicadorF != null) indicadorF.SetActive(false);
    }

    private void Update()
    {
        if (!bloqueado && jugadorCerca && Input.GetKeyDown(KeyCode.F))
            CiclarMedallon();
    }


    private void CiclarMedallon()
    {
        fueModificado = true;
        MedallonActual = (TipoMedallon)(((int)MedallonActual + 1) % 3);
        ActualizarVisuales();
        OnMedallonCambiado?.Invoke();
    }

    private void ActualizarVisuales()
    {
        if (medallon_Sol) medallon_Sol.SetActive(MedallonActual  == TipoMedallon.Sol);
        if (medallon_Luna) medallon_Luna.SetActive(MedallonActual == TipoMedallon.Luna);
        if (medallon_Estrella) medallon_Estrella.SetActive(MedallonActual == TipoMedallon.Estrella);
    }

    public void Bloquear()
    {
        bloqueado = true;
        if (indicadorF != null) indicadorF.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("PJ")) return;
        jugadorCerca = true;
        if (!bloqueado && indicadorF != null) indicadorF.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("PJ")) return;
        jugadorCerca = false;
        if (indicadorF != null) indicadorF.SetActive(false);
    }
}
