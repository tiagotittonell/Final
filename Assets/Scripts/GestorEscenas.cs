using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorEscenas : MonoBehaviour
{
    public static string ultimaEscena;

    public static void CargarGameOver()
    {
        // Guarda la escena actual
        ultimaEscena = SceneManager.GetActiveScene().name;

        // Carga la escena de Game Over
        SceneManager.LoadScene("PantallaDerrota");
    }
}