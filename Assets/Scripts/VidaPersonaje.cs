using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaPersonaje : MonoBehaviour
{
    [SerializeField] private float vida;
    [SerializeField] private BarraDeVida barraVida;
    private MovimientoPersonaje movimientoJugador;
    [SerializeField] private float tiempoPerdidaControl = 2f; 
    private Animator animator;
    [SerializeField] private float vidaMaxima;
    [SerializeField] private float fuerzaEmpuje = 10f; 
    private Rigidbody2D rb;
    private bool estaMuerto = false;

    private void Start()
    {
        vida = vidaMaxima;
        barraVida.InicializarBarraDeVida(vida);
        movimientoJugador = GetComponent<MovimientoPersonaje>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private IEnumerator PerderControl()
    {
        movimientoJugador.sePuedeMover = false;
        yield return new WaitForSeconds(tiempoPerdidaControl);
        movimientoJugador.sePuedeMover = true;
    }

    private IEnumerator Muerte()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("PantallaDerrota");
    }

    public void TomarDañoBarra(float Daño)
    {
        vida -= Daño;
        barraVida.CambiarVidaActual(vida);

        if (vida <= 0)
        {
            movimientoJugador.sePuedeMover = false;
            movimientoJugador.enabled = false;
            GestorEscenas.CargarGameOver();
            animator.SetTrigger("Muerte");
            StartCoroutine(Muerte());
            return;
        }

        animator.ResetTrigger("Hurt"); 
        animator.SetTrigger("Hurt");

        StartCoroutine(PerderControl());
    }

}
