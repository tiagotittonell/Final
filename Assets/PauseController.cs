using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private string nombreEscenaMenu = "Menu";

    private void Start()
    {
        panelPausa.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePausa();
    }

    private void TogglePausa()
    {
        bool pausado = !panelPausa.activeSelf;
        panelPausa.SetActive(pausado);
        Time.timeScale = pausado ? 0f : 1f;
    }

    public void BotonMenu()
    {
        Time.timeScale = 1f;
        GestorJugador.bloquearGuardado = true;
        GestorJugador.Resetear();
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void BotonReiniciar()
    {
        Time.timeScale = 1f;
        GestorJugador.bloquearGuardado = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BotonSalir()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}



