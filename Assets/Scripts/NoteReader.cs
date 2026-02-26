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

    [Tooltip("Texto 1 de la nota (TMP).")]
    [SerializeField] private TextMeshProUGUI noteText1;

    [Tooltip("Texto 2 de la nota (TMP).")]
    [SerializeField] private TextMeshProUGUI noteText2;

    [Tooltip("Texto ayuda para recordarle al jugador presionar la F para interactuar")]
    [SerializeField] private TextMeshProUGUI helpText;

    private bool _playerInside;
    private bool _isOpen;

    // 0 = cerrado, 1 = mostrando texto 1, 2 = mostrando texto 2
    private int _step = 0;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (helpText != null) helpText.gameObject.SetActive(false);

        if (noteText1 != null) noteText1.gameObject.SetActive(false);
        if (noteText2 != null) noteText2.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInside && !_isOpen) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (!_isOpen)
            {
                Open();           // abre y muestra texto 1
            }
            else
            {
                AdvanceOrClose(); // pasa a texto 2 o cierra
            }
        }
    }

    private void Open()
    {
        _isOpen = true;
        _step = 1;

        if (panel != null) panel.SetActive(true);
        if (helpText != null) helpText.gameObject.SetActive(false);

        if (noteText1 != null) noteText1.gameObject.SetActive(true);
        if (noteText2 != null) noteText2.gameObject.SetActive(false);

        Time.timeScale = 0f;
    }

    private void AdvanceOrClose()
    {
        if (_step == 1)
        {
            _step = 2;
            if (noteText1 != null) noteText1.gameObject.SetActive(false);
            if (noteText2 != null) noteText2.gameObject.SetActive(true);
            return;
        }

        // _step == 2 -> cerrar
        Close();
    }

    private void Close()
    {
        _isOpen = false;
        _step = 0;

        Time.timeScale = 1f;

        if (noteText1 != null) noteText1.gameObject.SetActive(false);
        if (noteText2 != null) noteText2.gameObject.SetActive(false);

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