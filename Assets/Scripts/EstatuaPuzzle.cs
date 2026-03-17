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

    private SpriteRenderer[] outlineRenderers;

    private void Start()
    {
        MedallonActual = medallonInicial;
        ActualizarVisuales();
        if (indicadorF != null) indicadorF.SetActive(false);
        CrearOutlines();
    }

    private void CrearOutlines()
    {
        SpriteRenderer[] todos = GetComponentsInChildren<SpriteRenderer>();
        var estatuaSRs = new System.Collections.Generic.List<SpriteRenderer>();

        foreach (SpriteRenderer sr in todos)
        {
            Transform t = sr.transform;
            bool esMedallon = false;
            while (t != null && t != transform)
            {
                if ((medallon_Sol     && t == medallon_Sol.transform)     ||
                    (medallon_Luna    && t == medallon_Luna.transform)    ||
                    (medallon_Estrella && t == medallon_Estrella.transform))
                {
                    esMedallon = true;
                    break;
                }
                t = t.parent;
            }
            if (!esMedallon) estatuaSRs.Add(sr);
        }

        Shader fillShader = Resources.Load<Shader>("SpriteFill");
        if (fillShader == null) fillShader = Shader.Find("Custom/SpriteFill");
        Material fillMat  = fillShader != null ? new Material(fillShader) { color = Color.white } : null;

        outlineRenderers = new SpriteRenderer[estatuaSRs.Count];
        for (int i = 0; i < estatuaSRs.Count; i++)
        {
            SpriteRenderer sr = estatuaSRs[i];

            GameObject outlineGO = new GameObject("Outline");
            outlineGO.transform.SetParent(sr.transform, false);
            outlineGO.transform.localScale = Vector3.one * 1.06f;

            SpriteRenderer outlineSR = outlineGO.AddComponent<SpriteRenderer>();
            outlineSR.sprite = sr.sprite;
            outlineSR.color = Color.white;
            outlineSR.sortingLayerID = sr.sortingLayerID;
            outlineSR.sortingOrder = sr.sortingOrder - 1;
            if (fillMat != null) outlineSR.material = fillMat;
            outlineSR.enabled = false;

            outlineRenderers[i] = outlineSR;
        }
    }

    private void SetOutline(bool activo)
    {
        if (outlineRenderers == null) return;
        foreach (SpriteRenderer sr in outlineRenderers)
            if (sr != null) sr.enabled = activo;
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
        SetOutline(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("PJ")) return;
        jugadorCerca = true;
        if (!bloqueado && indicadorF != null) indicadorF.SetActive(true);
        if (!bloqueado) SetOutline(true);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("PJ")) return;
        jugadorCerca = false;
        if (indicadorF != null) indicadorF.SetActive(false);
        SetOutline(false);
    }
}
