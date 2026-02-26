using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemigo : MonoBehaviour
{
    [SerializeField] private float vida;
    [SerializeField] private BarraDeVidaBoss barraVida;
    [SerializeField] private float vidaMaxima;

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

    // Start is called before the first frame update
    void Start()
    {

        vida = vidaMaxima;
        barraVida.InicializarBarraDeVida(vida);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        jugador = GameObject.FindGameObjectWithTag("PJ").GetComponent<Transform>();
    }
    private void Update()
    {
        
            float distanciaJugador = Vector2.Distance(transform.position, jugador.position);
            animator.SetFloat("DistanciaJugador", distanciaJugador);
        
    }
        // Update is called once per frame
        
    public void TomarDañoEnemigo(float daño)
    {
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

    public void MirarJugador()
    {
        if ((jugador.position.x > transform.position.x && !mirandoDerecha) || (jugador.position.x < transform.position.x && mirandoDerecha))
        {
            mirandoDerecha = !mirandoDerecha;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
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
