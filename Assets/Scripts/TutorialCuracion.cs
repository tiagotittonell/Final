using UnityEngine;

public class TutorialCuracion : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private KeyCode teclaCerrar = KeyCode.F;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        if (GestorConsumibles.Instance != null)
            GestorConsumibles.Instance.OnConsumibleAgregado += MostrarTutorial;
    }

    private void Update()
    {
        if (panel != null && panel.activeSelf && Input.GetKeyDown(teclaCerrar))
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void MostrarTutorial()
    {
        if (panel != null)
            panel.SetActive(true);

        Time.timeScale = 0f;

        if (GestorConsumibles.Instance != null)
            GestorConsumibles.Instance.OnConsumibleAgregado -= MostrarTutorial;
    }

    private void OnDestroy()
    {
        if (GestorConsumibles.Instance != null)
            GestorConsumibles.Instance.OnConsumibleAgregado -= MostrarTutorial;

        Time.timeScale = 1f;
    }
}
