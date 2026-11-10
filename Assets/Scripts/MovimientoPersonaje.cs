using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class MovimientoPersonaje : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D boxCollider;

    [SerializeField] private Vector2 velocidadRebote;
    [SerializeField] private bool damage_;
    [SerializeField] private int empuje;

    private float movimientoHorizontal = 0f;
    [SerializeField] private float velocidadDeMovimiento;
    [Range(0, 0.3f)][SerializeField] private float suavizadoMovimiento;
    [SerializeField] private float tiempoDeceleracion = 0.15f;
    private Vector3 velocidad = Vector3.zero;
    private bool mirandoDerecha = true;

    [SerializeField] private float deadzoneVelocidadX = 0.05f;

    public float fuerzaSalto = 10f;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionCaja = new Vector2(0.5f, 0.1f);
    private bool enSuelo;
    private bool enSueloAnterior = false;
    private bool salto = false;

    [SerializeField] private float multiplicadorCaida = 2.5f;

    [SerializeField] private float velocidadDash;
    [SerializeField] private float tiempoDash;
    private bool puedeHacerDash = true;
    private bool estaDashing = false;
    public bool sePuedeMover = true;
    [SerializeField] private TrailRenderer trailRender;

    //[SerializeField] private Transform controladorPared;
    //[SerializeField] private Vector2 dimensionCajaPared = new Vector2(0.5f, 1f);
    //private bool enPared;
    //private bool deslizando;
    //[SerializeField] private float velocidadDeslizar;
    //[SerializeField] private float fuerzaSaltoParedX;
    //[SerializeField] private float fuerzaSaltoParedY;
    //[SerializeField] private float tiempoSaltoPared;
    //private bool saltandoPared;
    //private bool saltoPared = false;

    [SerializeField] private int maxSaltosAire = 1;
    private int saltosAireRestantes;
    private bool esDobleSalto = false;

    [SerializeField] private float velocidadEscalar;
    [SerializeField] private float velocidadAnimacionEscalar = 1f;
    private float tiempoNormalizadoEscalar = 0f;
    private int hashEstadoEscalar = -1;
    private bool escalandoAnterior = false;
    private float gravedadInicial;
    private bool escalando;
    //private float tiempoEnAire = 0f;
    //[SerializeField] private float tiempoGraciaParede = 0.1f;
    private bool enHangTime = false;
    private Coroutine coroutinaHangTime;

    public bool EstaEnSuelo => enSuelo;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        gravedadInicial = rb.gravityScale;
    }

    private void Update()
    {
        DetectarEntrada();
        ActualizarAnimaciones();
        ActualizarPasos();

        if (enSuelo && !enSueloAnterior)
            AudioManager.Instance?.ReproducirAterrizaje();
        enSueloAnterior = enSuelo;
    }

    private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionCaja, 0f, queEsSuelo);
        //enPared = Physics2D.OverlapBox(controladorPared.position, dimensionCajaPared, 0f, queEsSuelo);

        //tiempoEnAire = enSuelo ? 0f : tiempoEnAire + Time.fixedDeltaTime;

        if (sePuedeMover)
        {
            Mover(movimientoHorizontal);
        }

        //if (deslizando)
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, -velocidadDeslizar);
        //}

        salto = false;
        //saltoPared = false;

        if (enSuelo)
            saltosAireRestantes = maxSaltosAire;

        Escalar();

        if (!escalando && /*!deslizando &&*/ !estaDashing && !enHangTime)
        {
            if (rb.velocity.y != 0)
                rb.gravityScale = gravedadInicial * multiplicadorCaida;
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
        if (!sePuedeMover)
        {
            movimientoHorizontal = 0f;
            return;
        }

        movimientoHorizontal = UnityEngine.Input.GetAxisRaw("Horizontal") * velocidadDeMovimiento;

        //deslizando = !enSuelo && enPared && tiempoEnAire > tiempoGraciaParede;

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
            //else if (deslizando)
            //{
            //    saltoPared = true;
            //}
            else if (saltosAireRestantes > 0)
            {
                salto = true;
                saltosAireRestantes--;
                esDobleSalto = true;
                anim.SetTrigger("DobleSalto");
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) && puedeHacerDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void ActualizarAnimaciones()
    {
        float vx = Mathf.Abs(rb.velocity.x);
        if (vx < deadzoneVelocidadX) vx = 0f;

        anim.SetFloat("VelocidadX", vx);
        anim.SetFloat("VelocidadY", escalando ? 0f : rb.velocity.y);
        anim.SetBool("enSuelo", enSuelo);
        //anim.SetBool("Deslizando", deslizando);
        anim.SetBool("Escalar", escalando);
        anim.SetBool("EsDobleSalto", esDobleSalto);

        if (escalando)
        {
            if (escalandoAnterior && hashEstadoEscalar == -1)
            {
                if (anim.IsInTransition(0))
                {
                    AnimatorStateInfo nextInfo = anim.GetNextAnimatorStateInfo(0);
                    hashEstadoEscalar = nextInfo.shortNameHash;
                    tiempoNormalizadoEscalar = nextInfo.normalizedTime % 1f;
                }
                else
                {
                    AnimatorStateInfo currentInfo = anim.GetCurrentAnimatorStateInfo(0);
                    hashEstadoEscalar = currentInfo.shortNameHash;
                    tiempoNormalizadoEscalar = currentInfo.normalizedTime % 1f;
                }
            }

            if (hashEstadoEscalar != -1)
            {
                float inputVertical = UnityEngine.Input.GetAxisRaw("Vertical");
                tiempoNormalizadoEscalar = Mathf.Repeat(tiempoNormalizadoEscalar + inputVertical * velocidadAnimacionEscalar * Time.deltaTime, 1f);
                anim.Play(hashEstadoEscalar, 0, tiempoNormalizadoEscalar);
            }
        }
        else
        {
            anim.speed = 1f;
            tiempoNormalizadoEscalar = 0f;
            hashEstadoEscalar = -1;
        }

        escalandoAnterior = escalando;

        if (enSuelo)
            esDobleSalto = false;
    }

    private void Mover(float mover)
    {
        //if (!saltandoPared)
        //{
        if (!enHangTime)
        {
            Vector3 velocidadObjetivo = new Vector2(mover, rb.velocity.y);
            float tiempoSuave = (mover == 0f && enSuelo && !estaDashing && !escalando) ? tiempoDeceleracion : suavizadoMovimiento;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidad, tiempoSuave);
        }
        //}

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
        //if (saltoPared)
        //{
        //    SaltoPared();
        //}
    }

    private void Salto()
    {
        AudioManager.Instance?.ReproducirSalto();
        rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
        enSuelo = false;
    }

    //private void SaltoPared()
    //{
    //    enPared = false;
    //    deslizando = false;
    //    float dirX = mirandoDerecha ? -1f : 1f;
    //    rb.velocity = new Vector2(fuerzaSaltoParedX * dirX, fuerzaSaltoParedY);
    //    saltosAireRestantes = maxSaltosAire;
    //    StartCoroutine(CambioSaltoPared());
    //}

    //private IEnumerator CambioSaltoPared()
    //{
    //    saltandoPared = true;
    //    yield return new WaitForSeconds(tiempoSaltoPared);
    //    saltandoPared = false;
    //}

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
        AudioManager.Instance?.ReproducirDash();
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
            rb.gravityScale = 0; escalando = true; 
        } 
        else 
        { 
            rb.gravityScale = gravedadInicial; escalando = false; } if (enSuelo) { escalando = false; 
        } 
    }

    public void AplicarImpulsoAereo(float duracion, float escalaGravedad, float impulsoLateral)
    {
        if (coroutinaHangTime != null)
            StopCoroutine(coroutinaHangTime);
        coroutinaHangTime = StartCoroutine(CorrutinaImpulsoAereo(duracion, escalaGravedad, impulsoLateral));
    }

    private IEnumerator CorrutinaImpulsoAereo(float duracion, float escalaGravedad, float impulsoLateral)
    {
        enHangTime = true;
        rb.gravityScale = escalaGravedad;
        float dirX = Mathf.Sign(transform.localScale.x);
        rb.velocity = new Vector2(dirX * impulsoLateral, rb.velocity.y);
        yield return new WaitForSeconds(duracion);
        enHangTime = false;
    }

    public void EstablecerVelocidadCinematica(float velocidadHorizontal)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (velocidadHorizontal > 0f && !mirandoDerecha)
            Girar();
        else if (velocidadHorizontal < 0f && mirandoDerecha)
            Girar();

        rb.velocity = new Vector2(velocidadHorizontal, rb.velocity.y);
    }

    public void DetenerMovimientoCinematico()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    [SerializeField] private float intervaloPaso = 0.35f;
    private float _temporizadorPaso;

    private void ActualizarPasos()
    {
        bool seEstaMoviendo = enSuelo && !estaDashing && UnityEngine.Input.GetAxisRaw("Horizontal") != 0f;

        if (seEstaMoviendo)
        {
            _temporizadorPaso += Time.deltaTime;
            if (_temporizadorPaso >= intervaloPaso)
            {
                _temporizadorPaso = 0f;
                AudioManager.Instance?.ReproducirPaso();
            }
        }
        else
        {
            _temporizadorPaso = intervaloPaso;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionCaja);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(controladorPared.position, dimensionCajaPared);
    }
}
