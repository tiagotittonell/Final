using UnityEngine;

public class StatueController : MonoBehaviour
{
    [Header("Panel del mensaje")]
    [SerializeField] private GameObject panelTutorial;

    private bool jugadorDentro = false;
    private bool panelActivo = false;
    private bool yaInteractuo = false;

    void Update()
    {
        if (jugadorDentro && !panelActivo && !yaInteractuo)
        {
            MostrarPanel();
        }

        if (panelActivo && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CerrarPanel();
        }
    }

    private void MostrarPanel()
    {
        panelTutorial.SetActive(true);
        Time.timeScale = 0f;
        panelActivo = true;
    }

    private void CerrarPanel()
    {
        panelTutorial.SetActive(false);
        Time.timeScale = 1f;
        panelActivo = false;
        yaInteractuo = true; // la estatua queda usada
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PJ") && !yaInteractuo)
        {
            jugadorDentro = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("PJ"))
        {
            jugadorDentro = false;
        }
    }
}