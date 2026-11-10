using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BarraVidaEnemigo : MonoBehaviour
{
    [Header("Nombre")]
    [SerializeField] private string nombreEnemigo  = "Enemy";
    [SerializeField] private TMP_FontAsset fuente;
    [SerializeField] private Color colorTexto      = Color.white;
    public float tamañoFuente    = 10f;

    [Header("Barra")]
    [SerializeField] private Color colorRelleno    = new Color(0.85f, 0.15f, 0.15f);
    [SerializeField] private Color colorFondo      = new Color(0.15f, 0.15f, 0.15f);
    [SerializeField] private float anchoTotal      = 2f;    
    [SerializeField] private float altoBarra       = 0.25f; 
    [SerializeField] private float grosorBorde     = 0.05f; 
    [SerializeField] private Vector3 offset        = new Vector3(0f, 1.8f, 0f);

    [Header("Visibilidad")]
    [SerializeField] private float duracionMostrar = 3f;

    private Canvas _canvas;
    private Image  _fill;
    private float  _vidaMax    = 1f;
    private float  _timerVisible;

    private const float Escala = 0.01f;


    private void Awake() => ConstruirUI();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_canvas != null) { Destroy(_canvas.gameObject); _canvas = null; }
        if (!Application.isPlaying) return;
        ConstruirUI();
    }
#endif

    private void LateUpdate()
    {
        if (_canvas == null) return;

        _canvas.transform.position = transform.position + offset;

        if (Camera.main != null)
            _canvas.transform.rotation = Camera.main.transform.rotation;

        _timerVisible -= Time.deltaTime;
        _canvas.gameObject.SetActive(_timerVisible > 0f);
    }

    private void OnDestroy()
    {
        if (_canvas != null) Destroy(_canvas.gameObject);
    }


    public void Inicializar(float vidaMaxima)
    {
        _vidaMax = Mathf.Max(1f, vidaMaxima);
        if (_fill != null) _fill.fillAmount = 1f;
    }

    public void ActualizarVida(float vidaActual)
    {
        if (_fill != null)
            _fill.fillAmount = Mathf.Clamp01(vidaActual / _vidaMax);
        _timerVisible = duracionMostrar;
    }


    private void ConstruirUI()
    {
        float bw  = anchoTotal    / Escala;   
        float bh  = altoBarra     / Escala;   
        float bd  = grosorBorde   / Escala;   
        float th  = tamañoFuente  / Escala;
        float gap = 5f;                     

        float totalW = bw + bd * 2f;
        float totalH = th + gap + bh + bd * 2f;

        float yBar  = -(th + gap) * 0.5f;
        float yText =  (bh + bd * 2f + gap) * 0.5f;

        var go = new GameObject("_BarraVida_" + gameObject.name);
        _canvas             = go.AddComponent<Canvas>();
        _canvas.renderMode  = RenderMode.WorldSpace;
        _canvas.sortingOrder = 10;

        var cRT = go.GetComponent<RectTransform>();
        cRT.sizeDelta = new Vector2(totalW, totalH);
        go.transform.localScale = Vector3.one * Escala;
        go.SetActive(false);

        MakeImage("Border", cRT, Color.black,
                  new Vector2(0f, yBar),
                  new Vector2(bw + bd * 2f, bh + bd * 2f));

        var bgRT = MakeImage("BarBackground", cRT, colorFondo,
                             new Vector2(0f, yBar),
                             new Vector2(bw, bh));

        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(bgRT, false);
        _fill            = fillGO.AddComponent<Image>();
        _fill.color      = colorRelleno;
        _fill.type       = Image.Type.Filled;
        _fill.fillMethod = Image.FillMethod.Horizontal;
        _fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        _fill.fillAmount = 1f;
        var rtFill = fillGO.GetComponent<RectTransform>();
        rtFill.anchorMin = Vector2.zero;
        rtFill.anchorMax = Vector2.one;
        rtFill.offsetMin = rtFill.offsetMax = Vector2.zero;

        var textGO = new GameObject("Nombre");
        textGO.transform.SetParent(cRT, false);
        var tmp                = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text               = nombreEnemigo;
        tmp.fontSize           = tamañoFuente / Escala;
        tmp.color              = colorTexto;
        tmp.alignment          = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.overflowMode       = TextOverflowModes.Overflow;
        if (fuente != null) tmp.font = fuente;

        var rtText = textGO.GetComponent<RectTransform>();
        SetRT(rtText, new Vector2(0f, yText), new Vector2(totalW, th));
    }


    private static RectTransform MakeImage(string name, Transform parent,
                                           Color color, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<Image>().color = color;
        var rt = go.GetComponent<RectTransform>();
        SetRT(rt, pos, size);
        return rt;
    }

    private static void SetRT(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
    }
}
