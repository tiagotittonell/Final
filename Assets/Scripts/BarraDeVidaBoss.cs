using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVidaBoss : MonoBehaviour
{

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void CambiarDeVidaMaxima(float vidaMaxima)
    {
        slider.maxValue = vidaMaxima;
    }

    public void CambiarVidaActual(float cantidadVida)
    {
        slider.value = cantidadVida;
        
    }

    public void InicializarBarraDeVida(float cantidadVida)
    {
        CambiarDeVidaMaxima(cantidadVida);
        CambiarVidaActual(cantidadVida);
    }
}
