using System.Collections;
using UnityEngine;

public class LancerEnemy : EnemigoBase
{
    private enum Estado { Idle, Caminar, AtaqueMelee, AtaqueBarrido, AtaqueSalto }

    [Header("Vida")]
    [SerializeField] private float vida = 15f;
    private bool estaMuerto = false;

    [Header("Detección")]
    [SerializeField] private string tagJugador = "PJ";
    private Transform jugador;
    [SerializeField] private float distanciaDeteccion = 9f;

    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 2.5f;

    [Header("Suelo")]
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionCajaSuelo = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask capaSuelo;
    private bool enSuelo;
    private float gravityInicial;

    [Header("Melee — LancerAttack2")]
    [SerializeField] private BoxCollider2D hitboxMelee;
    [SerializeField] private float distanciaMelee = 1.5f;
    [SerializeField] private float dañoMelee = 10f;
    [SerializeField] private float cooldownMelee = 2.5f;
    private float timerMelee;

    [Header("Barrido — attackRect")]
    [SerializeField] private BoxCollider2D hitboxBarrido;
    [SerializeField] private float distanciaMinimaBarrido = 3.5f;
    [SerializeField] private float distanciaMaximaBarrido = 7f;
    [SerializeField] private float velocidadBarrido = 14f;
    [SerializeField] private float distanciaBarrido = 3f;
    [SerializeField] private float pausaEntreBarridos = 0.12f;
    [SerializeField] private float recoveryBarrido = 0.4f;
    [SerializeField] private float dañoBarrido = 8f;
    [SerializeField] private float cooldownBarrido = 4f;
    private float timerBarrido;

    [Header("Ataque Aéreo — LancerJump + AttackJump")]
    [SerializeField] private BoxCollider2D hitboxAereo;
    [SerializeField] private float distanciaMinimaAereo = 4.5f;
    [SerializeField] private float distanciaMaximaAereo = 9f;
    [SerializeField] private float alturaVuelo = 4f;
    [SerializeField] private float velocidadSubida = 6f;
    [SerializeField] private float tiempoEsperaEnAire = 0.4f;
    [SerializeField] private float velocidadDive = 18f;
    [SerializeField] private float dañoAereo = 15f;
    [SerializeField] private float cooldownAereo = 6f;
    private float timerAereo;

    [Header("Melee Rush")]
    [SerializeField][Range(0f, 1f)] private float chanceMeleeRush = 0.35f;

    [Header("Componentes")]
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    private BarraVidaEnemigo barraVida;

    [Header("Hit Flash")]
    [SerializeField] private Color colorFlash = Color.red;
    [SerializeField] private float duracionFlash = 0.15f;
    private Coroutine coroutinaFlash;

    private Estado estado = Estado.Idle;
    private bool mirandoDerecha = true;

    private static readonly int HashVelocidadX  = Animator.StringToHash("velocidadX");
    private static readonly int TrgMelee        = Animator.StringToHash("Melee");
    private static readonly int TrgBarrido      = Animator.StringToHash("Barrido");
    private static readonly int TrgSalto        = Animator.StringToHash("salto");
    private static readonly int TrgAtaqueSalto  = Animator.StringToHash("AtaqueSalto");
    private static readonly int TrgMuerte       = Animator.StringToHash("Muerte");

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (rb   == null) rb   = GetComponent<Rigidbody2D>();
        if (sr   == null) sr   = GetComponent<SpriteRenderer>();
        barraVida    = GetComponent<BarraVidaEnemigo>();
        gravityInicial = rb.gravityScale;
    }

    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag(tagJugador);
        if (go != null) jugador = go.transform;
        barraVida?.Inicializar(vida);
    }

    private void Update()
    {
        if (estaMuerto || jugador == null) return;

        enSuelo = controladorSuelo != null &&
                  Physics2D.OverlapBox(controladorSuelo.position, dimensionCajaSuelo, 0f, capaSuelo);

        if (timerMelee   > 0) timerMelee   -= Time.deltaTime;
        if (timerBarrido > 0) timerBarrido -= Time.deltaTime;
        if (timerAereo   > 0) timerAereo   -= Time.deltaTime;

        if (estado == Estado.Idle || estado == Estado.Caminar)
            TomarDecision();
    }


    private void TomarDecision()
    {
        float dist = Vector2.Distance(transform.position, jugador.position);

        if (dist > distanciaDeteccion) { SetIdle(); return; }

        if (dist <= distanciaMelee && timerMelee <= 0 && enSuelo)
        {
            StartCoroutine(CorrutinaMelee());
            return;
        }

        bool puedeBarrido = timerBarrido <= 0 && dist >= distanciaMinimaBarrido && dist <= distanciaMaximaBarrido && enSuelo;
        bool puedeAereo   = timerAereo   <= 0 && dist >= distanciaMinimaAereo   && dist <= distanciaMaximaAereo   && enSuelo;

        if ((puedeBarrido || puedeAereo) && Random.value < chanceMeleeRush)
        {
            CaminarHaciaJugador();
            return;
        }

        if (puedeBarrido && puedeAereo)
        {
            if (Random.value < 0.5f) StartCoroutine(CorrutinaBarrido());
            else                     StartCoroutine(CorrutinaAtaqueSalto());
            return;
        }
        if (puedeBarrido) { StartCoroutine(CorrutinaBarrido());     return; }
        if (puedeAereo)   { StartCoroutine(CorrutinaAtaqueSalto()); return; }

        CaminarHaciaJugador();
    }

    private void CaminarHaciaJugador()
    {
        estado = Estado.Caminar;
        float dx = jugador.position.x - transform.position.x;
        Girar(dx > 0);
        rb.velocity = new Vector2(Mathf.Sign(dx) * velocidadCaminar, rb.velocity.y);
        anim.SetFloat(HashVelocidadX, velocidadCaminar);
    }

    private void SetIdle()
    {
        estado = Estado.Idle;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        anim.SetFloat(HashVelocidadX, 0f);
    }

    private void Girar(bool haciaLaDerecha)
    {
        if (mirandoDerecha == haciaLaDerecha) return;
        mirandoDerecha = haciaLaDerecha;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    private IEnumerator CorrutinaMelee()
    {
        estado = Estado.AtaqueMelee;
        timerMelee = cooldownMelee;
        rb.velocity = Vector2.zero;
        anim.SetFloat(HashVelocidadX, 0f);
        Girar(jugador.position.x > transform.position.x);
        anim.SetTrigger(TrgMelee);

        yield return new WaitForSeconds(1f); 

        if (!estaMuerto) estado = Estado.Idle;
    }

    public void AplicarDañoMelee()
    {
        if (hitboxMelee == null) return;
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxMelee.bounds.center, hitboxMelee.bounds.size, 0f);
        foreach (Collider2D c in hits)
        {
            if (c.CompareTag(tagJugador))
                c.GetComponent<VidaPersonaje>()?.TomarDañoBarra(dañoMelee);
        }
    }

    private IEnumerator CorrutinaBarrido()
    {
        estado = Estado.AtaqueBarrido;
        timerBarrido = cooldownBarrido;
        rb.velocity = Vector2.zero;
        anim.SetFloat(HashVelocidadX, 0f);

        float direccionInicial = jugador.position.x < transform.position.x ? -1f : 1f;

        for (int i = 0; i < 3; i++)
        {
            if (estaMuerto) yield break;

            float dirX = (i % 2 == 0) ? direccionInicial : -direccionInicial;

            Girar(dirX > 0);
            anim.SetTrigger(TrgBarrido);

            yield return new WaitForSeconds(0.15f);

            float posicionInicialX = transform.position.x;
            float distanciaRecorrida = 0f;
            bool dañoAplicadoEnEstaBarrida = false;
            float posXAnterior = transform.position.x;
            float tiempoAtascado = 0f;

            while (distanciaRecorrida < distanciaBarrido)
            {
                if (estaMuerto) yield break;

                rb.velocity = new Vector2(dirX * velocidadBarrido, rb.velocity.y);

                float posXActual = transform.position.x;
                distanciaRecorrida = Mathf.Abs(posXActual - posicionInicialX);

                // Acumula tiempo sin avanzar: tolera frames sin FixedUpdate (fps > 50hz)
                if (distanciaRecorrida > 0.05f)
                {
                    if (Mathf.Abs(posXActual - posXAnterior) > 0.001f)
                        tiempoAtascado = 0f;
                    else
                        tiempoAtascado += Time.deltaTime;

                    if (tiempoAtascado >= 0.1f)
                    {
                        rb.velocity = new Vector2(0f, rb.velocity.y);
                        break;
                    }
                }
                posXAnterior = posXActual;

                if (!dañoAplicadoEnEstaBarrida && hitboxBarrido != null)
                {
                    Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxBarrido.bounds.center, hitboxBarrido.bounds.size, 0f);
                    foreach (Collider2D c in hits)
                    {
                        if (c.CompareTag(tagJugador))
                        {
                            c.GetComponent<VidaPersonaje>()?.TomarDañoBarra(dañoBarrido);
                            dañoAplicadoEnEstaBarrida = true;
                            break;
                        }
                    }
                }

                yield return null;
            }

            rb.velocity = new Vector2(0f, rb.velocity.y);

            if (i < 2)
                yield return new WaitForSeconds(pausaEntreBarridos);
        }

        yield return new WaitForSeconds(recoveryBarrido);

        if (!estaMuerto)
            estado = Estado.Idle;
    }


    private IEnumerator CorrutinaAtaqueSalto()
    {
        estado = Estado.AtaqueSalto;
        timerAereo = cooldownAereo;
        rb.velocity = Vector2.zero;
        anim.SetFloat(HashVelocidadX, 0f);
        anim.SetTrigger(TrgSalto);

        yield return new WaitForSeconds(0.2f); 

        // Subida
        rb.gravityScale = 0f;
        float targetY = transform.position.y + alturaVuelo;
        while (transform.position.y < targetY)
        {
            if (estaMuerto) { RestaurarGravedad(); yield break; }
            rb.velocity = new Vector2(0f, velocidadSubida);
            yield return null;
        }
        rb.velocity = Vector2.zero;

        Vector2 objetivoJugador = jugador.position;
        Girar(objetivoJugador.x > transform.position.x);
        anim.SetTrigger(TrgAtaqueSalto);
        yield return new WaitForSeconds(tiempoEsperaEnAire);

        Vector2 diff    = objetivoJugador - (Vector2)transform.position;
        Vector2 dirDive = diff.magnitude < 0.01f ? Vector2.down : diff.normalized;

        bool  dañoAplicado = false;
        float tiempoDive   = 0f;

        while (!enSuelo && tiempoDive < 4f)
        {
            if (estaMuerto) { RestaurarGravedad(); yield break; }

            rb.velocity = dirDive * velocidadDive;
            tiempoDive += Time.deltaTime;

            if (!dañoAplicado && hitboxAereo != null)
            {
                Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxAereo.bounds.center, hitboxAereo.bounds.size, 0f);
                foreach (Collider2D c in hits)
                {
                    if (c.CompareTag(tagJugador))
                    {
                        c.GetComponent<VidaPersonaje>()?.TomarDañoBarra(dañoAereo);
                        dañoAplicado = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        RestaurarGravedad();
        yield return new WaitForSeconds(0.5f); 

        if (!estaMuerto) estado = Estado.Idle;
    }

    private void RestaurarGravedad()
    {
        rb.gravityScale = gravityInicial;
        rb.velocity = Vector2.zero;
    }

    public override void TomarDaño(float daño)
    {
        if (estaMuerto) return;
        base.TomarDaño(daño);
        vida -= daño;
        barraVida?.ActualizarVida(vida);

        if (coroutinaFlash != null) StopCoroutine(coroutinaFlash);
        coroutinaFlash = StartCoroutine(FlashHit());

        if (vida <= 0) Morir();
    }

    private IEnumerator FlashHit()
    {
        if (sr == null) yield break;
        sr.color = colorFlash;
        yield return new WaitForSeconds(duracionFlash);
        sr.color = Color.white;
    }

    private void Morir()
    {
        estaMuerto = true;
        StopAllCoroutines();
        if (sr != null) sr.color = Color.white;
        rb.velocity = Vector2.zero;
        rb.gravityScale = gravityInicial;
        anim.SetTrigger(TrgMuerte);
        StartCoroutine(DestruirTrasAnimacion());
    }

    private IEnumerator DestruirTrasAnimacion()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        if (controladorSuelo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(controladorSuelo.position, dimensionCajaSuelo);
        }
    }
}

