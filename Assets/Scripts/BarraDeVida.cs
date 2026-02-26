using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
            Debug.LogError("[BarraDeVida] No hay Slider (uGUI) en este GameObject.");
    }

    public void CambiarDeVidaMaxima(float vidaMaxima)
    {
        if (slider == null) return;
        slider.maxValue = vidaMaxima;
    }

    public void CambiarVidaActual(float cantidadVida)
    {
        if (slider == null) return;
        slider.value = cantidadVida;
    }

    public void InicializarBarraDeVida(float cantidadVida)
    {
        CambiarDeVidaMaxima(cantidadVida);
        CambiarVidaActual(cantidadVida);
    }
}