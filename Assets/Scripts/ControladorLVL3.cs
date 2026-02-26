using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ControladorLVL3 : MonoBehaviour
{
    private void Start()
    {
        Enemigo enemigo = FindObjectOfType<Enemigo>(); 
        if (enemigo != null)
        {
            enemigo.OnMuerte += CargarPantallaVictoria;
        }
    }

    private void OnDestroy()
    {
        Enemigo enemigo = FindObjectOfType<Enemigo>();
        if (enemigo != null)
        {
            enemigo.OnMuerte -= CargarPantallaVictoria;
        }
    }

    private void CargarPantallaVictoria()
    {
        Invoke("CargarEscenaVictoria", 2f);
    }

    private void CargarEscenaVictoria()
    {
        SceneManager.LoadScene("Nivel 4");
    }
}
