using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPantalla : MonoBehaviour
{
    public void VolverAJugar()
    {
        if (!string.IsNullOrEmpty(GestorEscenas.ultimaEscena))
        {
            SceneManager.LoadScene(GestorEscenas.ultimaEscena);
        }
        else
        {
            SceneManager.LoadScene("Nivel 1");
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }
}