using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class DoorController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [SerializeField] private Collider2D doorBlockCollider;

    [SerializeField] private Collider2D nextLevelTriggerCollider;

    [Header("Animator Param")]
    [SerializeField] private string openBoolName = "Open"; 

    private int _openHash;
    private bool _opened;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        _openHash = Animator.StringToHash(openBoolName);

        SetClosedState();
    }

    private void SetClosedState()
    {
        _opened = false;

        if (animator != null)
            animator.SetBool(_openHash, false);

        if (doorBlockCollider != null)
            doorBlockCollider.enabled = true;

        if (nextLevelTriggerCollider != null)
            nextLevelTriggerCollider.enabled = false;
    }

    public void OpenDoor()
    {
        if (_opened) return;
        _opened = true;

        if (animator != null)
            animator.SetBool(_openHash, true);

        if (doorBlockCollider != null)
            doorBlockCollider.enabled = false;

        if (nextLevelTriggerCollider != null)
            nextLevelTriggerCollider.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[DoorController] OnTriggerEnter2D detectado. Objeto: {other.name}, Tag: {other.tag}");
        
        if (!other.CompareTag("PJ"))
        {
            Debug.Log("[DoorController] No es el jugador (tag no es 'PJ')");
            return;
        }
        
        if (!_opened)
        {
            Debug.Log("[DoorController] La puerta no está abierta (_opened = false)");
            return;
        }

        Debug.Log("[DoorController] Cargando escena 'Nivel 6'...");
        SceneManager.LoadScene("Nivel 6");
    }
}