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
        vida = GestorJugador.tieneVidaGuardada ? GestorJugador.vidaGuardada : vidaMaxima;

        if (barraVida != null)
        {
            barraVida.InicializarBarraDeVida(vidaMaxima);
            barraVida.CambiarVidaActual(vida);
        }
        else
            Debug.LogError("[VidaPersonaje] barraVida no está asignada en el Inspector");

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

    private IEnumerator ProcesarMuerte()
    {
        yield return new WaitForEndOfFrame();
        animator.SetFloat("VelocidadY", 0f);
        animator.SetFloat("VelocidadX", 0f);
        animator.SetBool("enSuelo", true);
        animator.SetBool("Escalar", false);
        animator.SetBool("EsDobleSalto", false);
        animator.SetTrigger("Muerte");
        yield return new WaitForSeconds(2f);
        GestorEscenas.CargarGameOver();
    }

    public void TomarDañoBarra(float daño)
    {
        if (estaMuerto) return;

        float vidaAntes = vida;

        vida -= daño;

        Debug.Log("DAMAGE RECIBIDO: " + daño + " vidaAntes=" + vidaAntes + " vidaAhora=" + vida);

        barraVida.CambiarVidaActual(vida);

        Vector2 dirShake = Vector2.down;
        Collider2D[] cercanos = Physics2D.OverlapCircleAll(transform.position, 6f);
        foreach (Collider2D c in cercanos)
        {
            if (c.CompareTag("Enemigo") || c.CompareTag("Boss"))
            {
                dirShake = ((Vector2)(c.transform.position - transform.position)).normalized;
                break;
            }
        }
        ControladorCamara.Instance?.ShakeDaño(dirShake);

        if (vida <= 0)
        {
            estaMuerto = true;
            movimientoJugador.sePuedeMover = false;
            movimientoJugador.enabled = false;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.isKinematic = true;
            animator.SetFloat("VelocidadY", 0f);
            animator.SetFloat("VelocidadX", 0f);
            animator.SetBool("enSuelo", true);
            animator.SetBool("Escalar", false);
            animator.SetBool("EsDobleSalto", false);
            StartCoroutine(ProcesarMuerte());
            return;
        }

        animator.ResetTrigger("Hurt");
        animator.SetTrigger("Hurt");

        StartCoroutine(PerderControl());
    }

    private void OnDestroy()
    {
        if (!estaMuerto && !GestorJugador.bloquearGuardado)
            GestorJugador.GuardarVida(vida);
    }

    public void Curar(float cantidad)
    {
        if (estaMuerto) return;
        vida = Mathf.Min(vida + cantidad, vidaMaxima);
        barraVida.CambiarVidaActual(vida);
    }

}
