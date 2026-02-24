using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteReader : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("UI (Canvas)")]
    [Tooltip("Panel que contiene la nota.")]
    [SerializeField] private GameObject panel;

    [Tooltip("Imagen de la nota (ya configurada).")]
    [SerializeField] private Image noteImage;

    [Tooltip("Texto de la nota (ya configurado).")]
    [SerializeField] private Text noteText;

    [Tooltip("Texto ayuda para recordarle al jugador presionar la F para interactuar")]
    [SerializeField] private TextMeshProUGUI helpText;

    private bool _playerInside;
    private bool _isOpen;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (helpText != null) helpText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInside && !_isOpen) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (_isOpen) Close();
            else Open();
        }
    }

    private void Open()
    {
        _isOpen = true;

        if (panel != null) panel.SetActive(true);
        if (helpText != null) helpText.gameObject.SetActive(false);

        Time.timeScale = 0f;
    }

    private void Close()
    {
        _isOpen = false;

        Time.timeScale = 1f;

        if (panel != null) panel.SetActive(false);
        if (helpText != null && _playerInside) helpText.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PJ")) return;
        _playerInside = true;
        if (helpText != null) helpText.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("PJ")) return;

        _playerInside = false;
        if (helpText != null) helpText.gameObject.SetActive(false);
        if (_isOpen) Close();
    }

    private void OnDisable()
    {
        if (_isOpen) Time.timeScale = 1f;
    }
}