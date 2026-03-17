using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControladorPuzzleAnimales : MonoBehaviour
{
    [Header("Botones (arrastrar los 6 Image GameObjects en orden 0-5)")]
    [SerializeField] private BotonAnimalPuzzle[] botones;

    [Header("Secuencia correcta — IDs del 0 al 5, exactamente 4 valores")]
    [SerializeField] private int[] solucion = { 0, 2, 4, 1 };

    [Header("Delay antes de resetear tras error (segundos)")]
    [SerializeField] private float delayReset = 0.6f;

    [Header("Eventos")]
    [SerializeField] private UnityEvent onPuzzleResuelto;
    [SerializeField] private UnityEvent onSecuenciaIncorrecta;

    private int _pasoActual = 0;
    private bool _resuelto = false;
    private int[] _secuenciaIngresada;

    public event System.Action OnResueltoEvento;


    private void Start()
    {
        _secuenciaIngresada = new int[solucion.Length];

        foreach (var img in GetComponentsInChildren<Image>(true))
            if (img.GetComponent<BotonAnimalPuzzle>() == null)
                img.raycastTarget = false;

        foreach (var b in botones)
            if (b != null) b.OnPresionado += OnBotonPresionado;
    }

    private void OnDestroy()
    {
        foreach (var b in botones)
            if (b != null) b.OnPresionado -= OnBotonPresionado;
    }


    private void OnBotonPresionado(int id)
    {
        if (_resuelto) return;
        if (_pasoActual >= solucion.Length) return;

        _secuenciaIngresada[_pasoActual] = id;
        GetBoton(id)?.SetSeleccionado();
        _pasoActual++;

        if (_pasoActual >= solucion.Length)
            VerificarSolucion();
    }

    private void VerificarSolucion()
    {
        bool correcta = true;
        for (int i = 0; i < solucion.Length; i++)
        {
            if (_secuenciaIngresada[i] != solucion[i])
            {
                correcta = false;
                break;
            }
        }

        if (correcta)
            Resolver();
        else
        {
            onSecuenciaIncorrecta?.Invoke();
            StartCoroutine(RoutineError());
        }
    }

    private void Resolver()
    {
        _resuelto = true;
        foreach (var b in botones)
            b?.SetBloqueado(true);

        Debug.Log("[PuzzleAnimales] ¡Puzzle resuelto!");
        OnResueltoEvento?.Invoke();
        onPuzzleResuelto?.Invoke();
    }

    private System.Collections.IEnumerator RoutineError()
    {
        foreach (var b in botones) b?.SetBloqueado(true);

        foreach (var b in botones) b?.MostrarError();

        yield return new WaitForSecondsRealtime(delayReset);

        _pasoActual = 0;
        _secuenciaIngresada = new int[solucion.Length];
        foreach (var b in botones) b?.Resetear();
    }


    private BotonAnimalPuzzle GetBoton(int id)
    {
        foreach (var b in botones)
            if (b != null && b.id == id) return b;
        return null;
    }
}
