using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorEscenas : MonoBehaviour
{
    public static string ultimaEscena;

    public static void CargarGameOver()
    {
        ultimaEscena = SceneManager.GetActiveScene().name;
        GestorJugador.Resetear();
        SceneManager.LoadScene("PantallaDerrota");
    }
}