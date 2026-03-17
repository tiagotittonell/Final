using TMPro;
using UnityEngine;

public class InteractorPuzzleAnimales : MonoBehaviour
{
    [Header("Prefab del puzzle (arrastrá desde Project, NO desde Hierarchy)")]
    [SerializeField] private GameObject panelPuzzle;

    [Header("Texto hint en escena (TextMeshProUGUI dentro de un Canvas)")]
    [SerializeField] private TextMeshProUGUI textoHint;
    [SerializeField] private string mensajeHint = "Presionar F";

    [Header("Referencia al jugador")]
    [SerializeField] private MovimientoPersonaje movimientoJugador;

    [Header("Tecla de interacción")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.F;

    private bool _jugadorCerca   = false;
    private bool _puzzleAbierto  = false;
    private bool _puzzleResuelto = false;
    private GameObject _instancia;

    private SpriteRenderer[] outlineRenderers;


    private void Awake()
    {
        if (textoHint != null) textoHint.gameObject.SetActive(false);
    }

    private void Start()
    {
        CrearOutlines();
    }

    private void CrearOutlines()
    {
        SpriteRenderer[] todos = GetComponentsInChildren<SpriteRenderer>();

        Shader fillShader = Resources.Load<Shader>("SpriteFill");
        if (fillShader == null) fillShader = Shader.Find("Custom/SpriteFill");
        Material fillMat  = fillShader != null ? new Material(fillShader) { color = Color.white } : null;

        outlineRenderers = new SpriteRenderer[todos.Length];
        for (int i = 0; i < todos.Length; i++)
        {
            SpriteRenderer sr = todos[i];

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
        if (!_jugadorCerca && !_puzzleAbierto) return;

        if (Input.GetKeyDown(teclaInteraccion))
        {
            if (!_puzzleAbierto)
                AbrirPuzzle();
            else if (!_puzzleResuelto)
                CerrarPuzzle();
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"[PuzzleAnimales] TriggerEnter: '{col.gameObject.name}' tag='{col.tag}'");

        if (!col.CompareTag("PJ")) return;

        _jugadorCerca = true;

        if (movimientoJugador == null)
            movimientoJugador = col.GetComponent<MovimientoPersonaje>();

        if (!_puzzleResuelto && textoHint != null)
        {
            textoHint.text = mensajeHint;
            textoHint.gameObject.SetActive(true);
        }

        if (!_puzzleResuelto) SetOutline(true);
        Debug.Log("[PuzzleAnimales] Jugador dentro del trigger. Presioná F para abrir.");
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("PJ")) return;

        _jugadorCerca = false;

        if (textoHint != null) textoHint.gameObject.SetActive(false);
        SetOutline(false);

        if (_puzzleAbierto && !_puzzleResuelto)
            CerrarPuzzle();
    }


    private void AbrirPuzzle()
    {
        _puzzleAbierto = true;

        if (panelPuzzle != null)
        {
            _instancia = Instantiate(panelPuzzle);
            _instancia.SetActive(true);
            var ctrl = _instancia.GetComponentInChildren<ControladorPuzzleAnimales>();
            if (ctrl != null)
                ctrl.OnResueltoEvento += OnPuzzleResuelto;
            Debug.Log($"[PuzzleAnimales] Panel instanciado: '{_instancia.name}'");
        }
        else
        {
            Debug.LogWarning("[PuzzleAnimales] ¡panelPuzzle es null! Asigná el prefab en el Inspector.");
        }

        if (textoHint         != null) textoHint.gameObject.SetActive(false);
        if (movimientoJugador != null) movimientoJugador.sePuedeMover = false;
        SetOutline(false);
        Time.timeScale = 0f;
    }

    private void CerrarPuzzle()
    {
        _puzzleAbierto = false;

        if (_instancia != null) { Destroy(_instancia); _instancia = null; }
        if (movimientoJugador != null) movimientoJugador.sePuedeMover = true;
        Time.timeScale = 1f;

        if (_jugadorCerca && !_puzzleResuelto && textoHint != null)
            textoHint.gameObject.SetActive(true);
    }

    public void OnPuzzleResuelto()
    {
        _puzzleResuelto = true;
        _puzzleAbierto  = false;

        if (_instancia        != null) { Destroy(_instancia); _instancia = null; }
        if (movimientoJugador != null) movimientoJugador.sePuedeMover = true;
        if (textoHint         != null) textoHint.gameObject.SetActive(false);
        SetOutline(false);
        Time.timeScale = 1f;

        Debug.Log("[PuzzleAnimales] ? ¡Puzzle resuelto correctamente!");
    }
}
