using System.Collections;
using UnityEngine;

public class BellInteractable : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [Header("Animator")]
    [SerializeField] private string ringTriggerName = "Ring"; // trigger en el Animator

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ringClip;

    [Header("Anti-spam")]
    [SerializeField] private float hitCooldown = 0.15f;
    private bool _canHit = true;

    private BellPuzzleController _controller;
    private int _ringTriggerHash;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        _ringTriggerHash = Animator.StringToHash(ringTriggerName);
    }

    public void SetController(BellPuzzleController controller)
    {
        _controller = controller;
    }

    /// <summary>
    /// Llamalo cuando el arma del PJ golpee la campana.
    /// </summary>
    public void Hit()
    {
        if (!_canHit) return;

        // feedback visual
        if (animator != null)
            animator.SetTrigger(_ringTriggerHash);

        // feedback audio
        if (audioSource != null && ringClip != null)
            audioSource.PlayOneShot(ringClip);

        // notificar al puzzle
        if (_controller != null)
            _controller.NotifyBellHit(this);

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        _canHit = false;
        yield return new WaitForSeconds(hitCooldown);
        _canHit = true;
    }
}