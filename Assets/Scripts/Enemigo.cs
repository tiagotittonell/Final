using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemigo : EnemigoBase
{
    [SerializeField] private float vida;
    [SerializeField] private BarraDeVidaBoss barraVida;
    [SerializeField] private float vidaMaxima;
    [SerializeField] private float distanciaVisibilidad = 5f;

    public delegate void MuerteDelegate();
    public event MuerteDelegate OnMuerte;

    private Animator animator;

    public Rigidbody2D rb;

    public Transform jugador;

    private bool mirandoDerecha = true;

    //ATAQUE ENEMIGO

    [SerializeField] private Transform controladorAtaque;

    [SerializeField] private float radioAtaque;

    [SerializeField] private float dañoAtaque;

    [SerializeField] private float distanciaAtaque = 2f;

    private float tiempoUltimoVolteo;
    [SerializeField] private float cooldownVolteo = 0.5f;
    private float posicionXAnterior;
    private float tiempoAtascado = 0f;
    [SerializeField] private float tiempoMaximoAtascado = 0.3f;
    [SerializeField] private float umbralMovimiento = 0.005f;

    void Start()
    {

        vida = vidaMaxima;
        barraVida.InicializarBarraDeVida(vida);
        barraVida.gameObject.SetActive(false);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        jugador = GameObject.FindGameObjectWithTag("PJ").GetComponent<Transform>();
        posicionXAnterior = transform.position.x;
    }
    private void Update()
    {
        
        float distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        float distanciaParaAnimador = distanciaJugador <= distanciaAtaque ? 0f : distanciaJugador;
        animator.SetFloat("DistanciaJugador", distanciaParaAnimador);
        barraVida.gameObject.SetActive(distanciaJugador <= distanciaVisibilidad);

        if (Mathf.Abs(rb.velocity.x) > 0.01f)
        {
            if (Mathf.Abs(transform.position.x - posicionXAnterior) < umbralMovimiento)
            {
                tiempoAtascado += Time.deltaTime;
                if (tiempoAtascado >= tiempoMaximoAtascado && Time.time >= tiempoUltimoVolteo + cooldownVolteo)
                {
                    Voltear();
                    tiempoUltimoVolteo = Time.time;
                    tiempoAtascado = 0f;
                }
            }
            else
            {
                tiempoAtascado = 0f;
            }
        }

        posicionXAnterior = transform.position.x;

        MirarJugador();
    }
        
    public override void TomarDaño(float daño)
    {
        base.TomarDaño(daño);

        vida -= daño;

        barraVida.CambiarVidaActual(vida);
        if (vida <= 0)
        {
           
            animator.SetTrigger("Muerte");
            //Muerte();
        }

    }
    private void Muerte()
    {
        //if (vida <= 0)
        //{
        //    //animator.SetTrigger("Muerte");

        Destroy(gameObject);
        OnMuerte?.Invoke();
        //}
    }

    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }

    public void MirarJugador()
    {
        if (Time.time < tiempoUltimoVolteo + cooldownVolteo)
            return;

        if ((jugador.position.x > transform.position.x && !mirandoDerecha) || (jugador.position.x < transform.position.x && mirandoDerecha))
        {
            Voltear();
            tiempoUltimoVolteo = Time.time;
        }
    }

    public void Ataque()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);

        foreach (Collider2D colision in objetos)
        {
            if (colision.CompareTag("PJ"))
            {
                colision.GetComponent<VidaPersonaje>().TomarDañoBarra(dañoAtaque);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
    }
}
