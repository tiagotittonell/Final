using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaBoss : MonoBehaviour
{
    [SerializeField] private float vida;
    [SerializeField] private BarraDeVida barraVida;

    [SerializeField] private float vidaMaxima;
    private Animator animator;

    public delegate void MuerteDelegate();
    public event MuerteDelegate OnMuerte;
    private void Start()
    {
        vida = vidaMaxima;
        barraVida.InicializarBarraDeVida(vida);

        animator = GetComponent<Animator>();

    }
 
    
    public void TomarDañoBarra(float Daño)
    {
        vida -= Daño;

        barraVida.CambiarVidaActual(vida);
        if (vida <= 0) 
        {
            Debug.Log("el enemigo murio");
            //animator.SetTrigger("Muerte");
            //Muerte();
        }
    }

    private void Muerte()
    {
        Destroy(gameObject);
        OnMuerte?.Invoke();
    }

}
