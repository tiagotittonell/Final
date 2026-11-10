using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDebugNavigator : MonoBehaviour
{
    private static SceneDebugNavigator instancia;

    [Header("Navegación de depuración")]
    [SerializeField] private KeyCode teclaSiguienteEscena = KeyCode.P;
    [SerializeField] private bool volverAlInicioAlFinal = true;

    private bool cargandoEscena;

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!cargandoEscena && Input.GetKeyDown(teclaSiguienteEscena))
            CargarSiguienteEscena();
#endif
    }

    public void CargarSiguienteEscena()
    {
        if (cargandoEscena)
            return;

        int cantidadEscenas = SceneManager.sceneCountInBuildSettings;
        if (cantidadEscenas <= 0)
        {
            Debug.LogWarning("[SceneDebugNavigator] No hay escenas habilitadas en Build Settings.", this);
            return;
        }

        int indiceActual = SceneManager.GetActiveScene().buildIndex;
        int siguienteIndice = indiceActual + 1;

        if (siguienteIndice >= cantidadEscenas)
        {
            if (!volverAlInicioAlFinal)
            {
                Debug.Log("[SceneDebugNavigator] Ya estás en la última escena del Build Settings.", this);
                return;
            }

            siguienteIndice = 0;
        }

        cargandoEscena = true;
        Debug.Log(
            $"[SceneDebugNavigator] Cambiando de escena {indiceActual} a {siguienteIndice}.",
            this);
        SceneManager.LoadScene(siguienteIndice);
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        cargandoEscena = false;
    }

    private void OnDestroy()
    {
        if (instancia != this)
            return;

        SceneManager.sceneLoaded -= AlCargarEscena;
        instancia = null;
    }
}
