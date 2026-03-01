using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class MovimientoPersonaje : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;

    [SerializeField] private Vector2 velocidadRebote;
    [SerializeField] private bool damage_;
    [SerializeField] private int empuje;

    private float movimientoHorizontal = 0f;
    [SerializeField] private float velocidadDeMovimiento;
    [Range(0, 0.3f)][SerializeField] private float suavizadoMovimiento;
    private Vector3 velocidad = Vector3.zero;
    private bool mirandoDerecha = true;

    [SerializeField] private float deadzoneVelocidadX = 0.05f;

    public float fuerzaSalto = 10f;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionCaja = new Vector2(0.5f, 0.1f);
    private bool enSuelo;
    private bool salto = false;

    [SerializeField] private float multiplicadorCaida = 2.5f;
    [SerializeField] private float multiplicadorSaltoCorto = 2f;

    [SerializeField] private float velocidadDash;
    [SerializeField] private float tiempoDash;
    private bool puedeHacerDash = true;
    private bool estaDashing = false;
    public bool sePuedeMover = true;
    [SerializeField] private TrailRenderer trailRender;

    [SerializeField] private Transform controladorPared;
    [SerializeField] private Vector2 dimensionCajaPared = new Vector2(0.5f, 1f);
    private bool enPared;
    private bool deslizando;
    [SerializeField] private float velocidadDeslizar;
    [SerializeField] private float fuerzaSaltoParedX;
    [SerializeField] private float fuerzaSaltoParedY;
    [SerializeField] private float tiempoSaltoPared;
    private bool saltandoPared;
    private bool saltoPared = false;

    [SerializeField] private int maxSaltosAire = 1;
    private int saltosAireRestantes;
    private bool esDobleSalto = false;

    [SerializeField] private float velocidadEscalar;
    private float gravedadInicial;
    private bool escalando;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        gravedadInicial = rb.gravityScale;
    }

    private void Update()
    {
        DetectarEntrada();
        ActualizarAnimaciones();
    }

    private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionCaja, 0f, queEsSuelo);
        enPared = Physics2D.OverlapBox(controladorPared.position, dimensionCajaPared, 0f, queEsSuelo);

        if (sePuedeMover)
        {
            Mover(movimientoHorizontal);
        }

        if (deslizando)
        {
            rb.velocity = new Vector2(rb.velocity.x, -velocidadDeslizar);
        }

        salto = false;
        saltoPared = false;

        if (enSuelo)
            saltosAireRestantes = maxSaltosAire;

        Escalar();

        if (!escalando && !deslizando && !estaDashing)
        {
            if (rb.velocity.y < 0)
                rb.gravityScale = gravedadInicial * multiplicadorCaida;
            else if (rb.velocity.y > 0 && !UnityEngine.Input.GetButton("Jump"))
                rb.gravityScale = gravedadInicial * multiplicadorSaltoCorto;
            else
                rb.gravityScale = gravedadInicial;
        }

        if (Mathf.Abs(rb.velocity.x) < deadzoneVelocidadX)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void DetectarEntrada()
    {
        movimientoHorizontal = UnityEngine.Input.GetAxisRaw("Horizontal") * velocidadDeMovimiento;

        deslizando = !enSuelo && enPared;

        if (UnityEngine.Input.GetButtonDown("Jump"))
        {
            if (enSuelo)
            {
                salto = true;
            }
            else if (escalando)
            {
                escalando = false;
                salto = true;
            }
            else if (deslizando)
            {
                saltoPared = true;
            }
            else if (saltosAireRestantes > 0)
            {
                salto = true;
                saltosAireRestantes--;
                esDobleSalto = true;
                anim.SetTrigger("DobleSalto");
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.C) && puedeHacerDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void ActualizarAnimaciones()
    {
        float vx = Mathf.Abs(rb.velocity.x);
        if (vx < deadzoneVelocidadX) vx = 0f;

        anim.SetFloat("VelocidadX", vx);
        anim.SetFloat("VelocidadY", rb.velocity.y);
        anim.SetBool("enSuelo", enSuelo);
        anim.SetBool("Deslizando", deslizando);
        anim.SetBool("Escalar", escalando);
        anim.SetBool("EsDobleSalto", esDobleSalto);

        if (enSuelo)
            esDobleSalto = false;
    }

    private void Mover(float mover)
    {
        if (!saltandoPared)
        {
            Vector3 velocidadObjetivo = new Vector2(mover, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidad, suavizadoMovimiento);
        }

        if (mover > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (mover < 0 && mirandoDerecha)
        {
            Girar();
        }

        if (salto)
        {
            Salto();
        }
        if (saltoPared)
        {
            SaltoPared();
        }
    }

    private void Salto()
    {
        rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
        enSuelo = false;
    }

    private void SaltoPared()
    {
        enPared = false;
        deslizando = false;
        float dirX = mirandoDerecha ? -1f : 1f;
        rb.velocity = new Vector2(fuerzaSaltoParedX * dirX, fuerzaSaltoParedY);
        saltosAireRestantes = maxSaltosAire;
        StartCoroutine(CambioSaltoPared());
    }

    private IEnumerator CambioSaltoPared()
    {
        saltandoPared = true;
        yield return new WaitForSeconds(tiempoSaltoPared);
        saltandoPared = false;
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private IEnumerator Dash()
    {
        estaDashing = true;
        sePuedeMover = false;
        puedeHacerDash = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(velocidadDash * transform.localScale.x, 0);
        anim.SetTrigger("Dash");
        trailRender.emitting = true;

        yield return new WaitForSeconds(tiempoDash);

        estaDashing = false;
        sePuedeMover = true;
        puedeHacerDash = true;
        rb.gravityScale = gravedadInicial;
        trailRender.emitting = false;
    }

    private void Escalar()
    {
        if ((UnityEngine.Input.GetAxisRaw("Vertical") != 0 || escalando) && boxCollider.IsTouchingLayers(LayerMask.GetMask("ESCALERAS")))
        {
            Vector2 velocidadSubida = new Vector2(rb.velocity.x, UnityEngine.Input.GetAxisRaw("Vertical") * velocidadEscalar);
            rb.velocity = velocidadSubida;
            rb.gravityScale = 0;
            escalando = true;
        }
        else
        {
            rb.gravityScale = gravedadInicial;
            escalando = false;
        }

        if (enSuelo)
        {
            escalando = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionCaja);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorPared.position, dimensionCajaPared);
    }
}