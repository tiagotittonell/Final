using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLancerEnemyController : MonoBehaviour
{
    [SerializeField] private string escenaSiguiente = "Nivel 8";
    [SerializeField] private float delayCargar = 2f;

    private LancerEnemy enemigo;

    private void Start()
    {
        enemigo = FindObjectOfType<LancerEnemy>();
        if (enemigo != null)
            enemigo.OnMuerte += CargarPantallaVictoria;
    }

    private void OnDestroy()
    {
        if (enemigo != null)
            enemigo.OnMuerte -= CargarPantallaVictoria;
    }

    private void CargarPantallaVictoria()
    {
        Invoke(nameof(CargarEscena), delayCargar);
    }

    private void CargarEscena()
    {
        SceneManager.LoadScene(escenaSiguiente);
    }
}
