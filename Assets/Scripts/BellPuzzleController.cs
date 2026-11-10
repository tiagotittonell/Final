using System.Collections;
using UnityEngine;

public class BellPuzzleController : MonoBehaviour
{
    [Header("Order (set from Inspector)")]
    [Tooltip("Arrastrá acá las campanas EN EL ORDEN CORRECTO.")]
    [SerializeField] private BellInteractable[] correctOrder;

    [Header("Door")]
    [SerializeField] private DoorController doorToOpen;

    [Header("Feedback / Tuning")]
    [SerializeField] private float resetDelayOnFail = 0.6f;
    [SerializeField] private bool lockPuzzleAfterSuccess = true;

    private int _index;
    private bool _solved;
    private bool _resetting;

    private void Awake()
    {
        _index = 0;
        _solved = false;
        _resetting = false;

        // Vincula automáticamente cada campana con este controller
        if (correctOrder != null)
        {
            for (int i = 0; i < correctOrder.Length; i++)
            {
                if (correctOrder[i] != null)
                    correctOrder[i].SetController(this);
            }
        }
    }

    /// <summary>
    /// Lo llama una campana cuando el jugador la golpea.
    /// </summary>
    public void NotifyBellHit(BellInteractable bell)
    {
        if (_solved) return;
        if (_resetting) return;
        if (correctOrder == null || correctOrder.Length == 0) return;

        // Si toca la campana esperada en este paso
        if (bell == correctOrder[_index])
        {
            _index++;

            // Completó la secuencia
            if (_index >= correctOrder.Length)
            {
                _solved = true;

                if (doorToOpen != null)
                    doorToOpen.OpenDoor();

                if (!lockPuzzleAfterSuccess)
                    StartCoroutine(ResetPuzzle(0.1f)); // opcional: permitir rejugar
            }
        }
        else
        {
            // Falló: reset
            StartCoroutine(ResetPuzzle(resetDelayOnFail));
        }
    }

    private IEnumerator ResetPuzzle(float delay)
    {
        _resetting = true;
        yield return new WaitForSeconds(delay);

        _index = 0;
        _resetting = false;
    }
}