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


    private void Awake()
    {
        if (textoHint != null) textoHint.gameObject.SetActive(false);
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

        Debug.Log("[PuzzleAnimales] Jugador dentro del trigger. Presioná F para abrir.");
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("PJ")) return;

        _jugadorCerca = false;

        if (textoHint != null) textoHint.gameObject.SetActive(false);

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
        Time.timeScale = 1f;

        Debug.Log("[PuzzleAnimales] ? ¡Puzzle resuelto correctamente!");
    }
}
