using UnityEngine;
using UnityEngine.Events;


public class EstatuePuzzleController : MonoBehaviour
{
    [Header("Estatuas del puzzle (arrastrar las 4)")]
    [SerializeField] private EstatuaPuzzle[] estatuas;

    [Header("Al resolver el puzzle")]
    [SerializeField] private UnityEvent onPuzzleResuelto;

    private bool puzzleResuelto = false;

    private void Start()
    {
        foreach (var e in estatuas)
            if (e != null) e.OnMedallonCambiado += VerificarSolucion;
    }

    private void OnDestroy()
    {
        foreach (var e in estatuas)
            if (e != null) e.OnMedallonCambiado -= VerificarSolucion;
    }


    private void VerificarSolucion()
    {
        if (puzzleResuelto) return;

        foreach (var e in estatuas)
        {
            if (e == null || !e.EsCorrecto) return;
        }

        puzzleResuelto = true;
        Debug.Log("[EstatuePuzzle] Puzzle resuelto — invocando onPuzzleResuelto");

        foreach (var e in estatuas)
            e?.Bloquear();

        onPuzzleResuelto.Invoke();
    }
}
