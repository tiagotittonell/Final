using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorNiVELES : MonoBehaviour
{
    public List<GameObject> niveles;
    private int indiceActual = 0;

    void Start()
    {
        // Desactivar todos los niveles excepto el primero
        DesactivarTodosLosNiveles();
        niveles[indiceActual].SetActive(true);
    }

    void Update()
    {
        // Cambiar al siguiente nivel al presionar la tecla B
        if (Input.GetKeyDown(KeyCode.C))
        {
            CambiarNivel();
        }
    }

    void DesactivarTodosLosNiveles()
    {
        foreach (var nivel in niveles)
        {
            nivel.SetActive(false);
        }
    }

    void CambiarNivel()
    {
        // Desactivar el nivel actual
        niveles[indiceActual].SetActive(false);

        // Incrementar el índice o volver al primero si estamos en el último
        indiceActual = (indiceActual + 1) % niveles.Count;

        // Activar el siguiente nivel
        niveles[indiceActual].SetActive(true);
    }
}
