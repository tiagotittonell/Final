using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotonAnimalPuzzle : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler
{
    [Header("Identificación")]
    public int id; 

    [Header("Colores")]
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorHover = new Color(1f, 0.85f, 0.3f, 1f);
    [SerializeField] private Color colorSeleccionado = new Color(0.5f, 0.8f, 1f, 1f);
    [SerializeField] private Color colorConfirmado = new Color(1f, 0.65f, 0f, 1f);
    [SerializeField] private Color colorError = new Color(1f, 0.2f, 0.2f, 1f);

    [Header("Efecto press")]
    [SerializeField] private float escalaPress = 0.88f;
    [SerializeField] private float velocidadAnim = 18f;

    public event Action<int> OnPresionado;

    private Image _img;
    private bool _confirmado = false;
    private bool _seleccionado = false;
    private bool _bloqueado = false;
    private bool _sobreEl = false;
    private Vector3 _escalaBase;
    private Vector3 _escalaTarget;
    private Coroutine _errorRoutine;

    private void Awake()
    {
        _img = GetComponent<Image>();
        _escalaBase = transform.localScale;
        _escalaTarget = _escalaBase;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale, _escalaTarget, Time.unscaledDeltaTime * velocidadAnim);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (_bloqueado) return;
        _sobreEl = true;
        if (!_confirmado && !_seleccionado) _img.color = colorHover;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (_bloqueado) return;
        _sobreEl = false;
        if (!_confirmado)
            _img.color = _seleccionado ? colorSeleccionado : colorNormal;
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (_bloqueado) return;
        _escalaTarget = _escalaBase * escalaPress;
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (_bloqueado) return;
        _escalaTarget = _escalaBase;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (_bloqueado) return;
        OnPresionado?.Invoke(id);
    }

    public void SetSeleccionado()
    {
        _seleccionado = true;
        _img.color = colorSeleccionado;
    }

    public void SetConfirmado()
    {
        _confirmado = true;
        _img.color  = colorConfirmado;
    }

    public void MostrarError()
    {
        if (_errorRoutine != null) StopCoroutine(_errorRoutine);
        _errorRoutine = StartCoroutine(RoutineError());
    }

    public void Resetear()
    {
        if (_errorRoutine != null) StopCoroutine(_errorRoutine);
        _confirmado   = false;
        _seleccionado = false;
        _bloqueado    = false;
        _sobreEl      = false;
        _escalaTarget = _escalaBase;
        transform.localScale = _escalaBase;
        _img.color    = colorNormal;
    }

    public void SetBloqueado(bool valor) => _bloqueado = valor;

    private System.Collections.IEnumerator RoutineError()
    {
        _img.color = colorError;
        yield return new WaitForSecondsRealtime(0.4f);
        _img.color = colorNormal;
        _errorRoutine = null;
    }
}
