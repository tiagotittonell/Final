using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderController : MonoBehaviour, IDamageable
{
    private enum Estado { EnTelarana, Bajando, EnSuelo, Subiendo }

    [Header("Stats")]
    [SerializeField] private float vida = 50f;
    [SerializeField] private float vidaMaxima = 50f;
    private bool estaMuerto = false;

    [Header("Referencia Jugador")]
    [SerializeField] private string tagJugador = "PJ";
    private Transform jugador;

    [Header("Componentes")]
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Deteccion Suelo")]
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionCajaSuelo = new Vector2(0.6f, 0.1f);
    private bool enSuelo;

    // ??? TELARAÑAS ???
    [Header("Telarañas")]
    [SerializeField] private string tagTelarana = "Telarana";
    [SerializeField] private float velocidadEnTelarana = 4f;
    [SerializeField] private GameObject prefabTelarana;
    [SerializeField] private float intervaloCrearTelarana = 12f;
    [SerializeField] private float rangoSpawnTelarana = 8f;
    [SerializeField] private float alturaTelaranas = 6f;
    private List<Transform> telaranas = new List<Transform>();
    private Transform telaranaActual;
    private Transform telaranaDestino;
    private float timerCrearTelarana;
    private Vector2 puntoPatrulla;

    // ??? ATAQUES DESDE TELARAÑA ???
    [Header("Ataque Proyectil (Escupir Telaraña)")]
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreAtaques = 2.5f;
    private float timerAtaque;

    [Header("Ataque Embestida (Dash al Jugador)")]
    [SerializeField] private float velocidadEmbestida = 15f;
    [SerializeField] private float danoEmbestida = 15f;
    private bool embistiendo = false;

    [Header("Ataque Salto (Caer sobre Jugador)")]
    [SerializeField] private float fuerzaSaltoArriba = 12f;
    [SerializeField] private float velocidadCaida = 18f;
    [SerializeField] private float danoCaida = 20f;
    [SerializeField] private float radioImpacto = 1.5f;
    private bool saltando = false;
    private bool cayendo = false;
    private Vector2 posicionObjetivoSalto;
    private float timerBajando;

    // ??? FASE SUELO ???
    [Header("Movimiento en Suelo")]
    [SerializeField] private float velocidadCaminar = 2.5f;
    [SerializeField] private float tiempoEnSuelo = 6f;
    private float timerSuelo;

    [Header("Ataque Cuerpo a Cuerpo")]
    [SerializeField] private Transform controladorAtaqueMelee;
    [SerializeField] private float radioAtaqueMelee = 0.8f;
    [SerializeField] private float danoMelee = 10f;
    [SerializeField] private float distanciaMelee = 2.5f;
    [SerializeField] private float cooldownMelee = 1.2f;
    private float timerMelee;
    private float pausaDespuesMelee = 0f;
    [SerializeField] private float duracionPausaMelee = 1f;

    [Header("Subir a Telaraña")]
    [SerializeField] private float velocidadSubir = 5f;

    // ??? ESTADO ???
    private Estado estadoActual = Estado.EnTelarana;
    private bool mirandoDerecha = true;
    private int ataquesPendientesEnTelarana;
    private Vector2 direccionEmbestida;

    // ??? ANIMATOR HASHES ???
    private static readonly int AnimWalk = Animator.StringToHash("Walk");
    private static readonly int AnimRangeAttack = Animator.StringToHash("RangeAttack");
    private static readonly int AnimAttackJumpDash = Animator.StringToHash("Dash");
    private static readonly int AnimMelee = Animator.StringToHash("Melee");
    private static readonly int AnimDeath = Animator.StringToHash("Death");

    // Pausa por ataque a distancia
    private bool pausadoPorAtaque = false;
    [SerializeField] private float duracionPausaDisparo = 1.5f;

    // Barra de vida UI
    private Slider sliderVida;
    private GameObject canvasBarraVida;
    [Header("Barra de Vida")]
    [SerializeField] private string nombreEnemigo = "Araña Reina";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag(tagJugador);
        if (go != null) jugador = go.transform;

        RecolectarTelaranas();
        timerCrearTelarana = intervaloCrearTelarana;
        timerAtaque = tiempoEntreAtaques;

        if (telaranas.Count > 0)
        {
            telaranaActual = TelaranaMasCercana();
            transform.position = telaranaActual.position;
            ElegirPuntoPatrulla();
        }

        rb.gravityScale = 0f;
        PrepararAtaquesTelarana();
        CrearBarraDeVida();
    }

    private void Update()
    {
        if (estaMuerto || jugador == null) return;

        MirarHaciaJugador();
        if (estadoActual == Estado.EnTelarana)
            ActualizarTimerTelaranas();

        switch (estadoActual)
        {
            case Estado.EnTelarana:
                LogicaTelarana();
                break;
            case Estado.Bajando:
                LogicaBajando();
                break;
            case Estado.EnSuelo:
                LogicaSuelo();
                break;
            case Estado.Subiendo:
                LogicaSubiendo();
                break;
        }
    }

    private void FixedUpdate()
    {
        enSuelo = controladorSuelo != null &&
                  Physics2D.OverlapBox(controladorSuelo.position, dimensionCajaSuelo, 0f, capaSuelo);
    }

    // ??????????????????????????????????????????
    //  TELARAÑAS
    // ??????????????????????????????????????????
    private void RecolectarTelaranas()
    {
        telaranas.Clear();
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagTelarana);
        foreach (var o in objs)
            telaranas.Add(o.transform);
    }

    private Transform TelaranaMasCercana()
    {
        Transform mejor = null;
        float dist = float.MaxValue;
        foreach (var t in telaranas)
        {
            if (t == null) continue;
            float d = Vector2.Distance(transform.position, t.position);
            if (d < dist) { dist = d; mejor = t; }
        }
        return mejor;
    }

    private void ElegirTelaranaDestino()
    {
        if (telaranas.Count <= 1) { telaranaDestino = telaranaActual; return; }

        Transform nueva;
        do
        {
            nueva = telaranas[Random.Range(0, telaranas.Count)];
        } while (nueva == telaranaActual && telaranas.Count > 1);

        telaranaDestino = nueva;
    }

    private void ElegirPuntoPatrulla()
    {
        if (telaranaActual == null) return;

        Collider2D col = telaranaActual.GetComponent<Collider2D>();
        if (col != null)
        {
            Bounds b = col.bounds;
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            puntoPatrulla = new Vector2(x, y);
        }
        else
        {
            puntoPatrulla = telaranaActual.position;
        }
    }

    private void ActualizarTimerTelaranas()
    {
        timerCrearTelarana -= Time.deltaTime;
        if (timerCrearTelarana <= 0f)
        {
            CrearTelarana();
            timerCrearTelarana = Random.Range(10f, 15f);
        }
    }

    private void CrearTelarana()
    {
        if (prefabTelarana == null)
        {
            Debug.LogWarning("[Spider] prefabTelarana NO está asignado en el Inspector!");
            return;
        }

        // Limpiar telarañas destruidas
        telaranas.RemoveAll(t => t == null);

        if (telaranas.Count == 0)
        {
            Vector3 posFallback = new Vector3(transform.position.x, alturaTelaranas, 0f);
            GameObject nuevaFb = Instantiate(prefabTelarana, posFallback, Quaternion.identity);
            telaranas.Add(nuevaFb.transform);
            return;
        }

        // Elegir una telaraña existente al azar como referencia
        Transform referencia = telaranas[Random.Range(0, telaranas.Count)];
        Collider2D colRef = referencia.GetComponent<Collider2D>();

        float anchoNueva = 0f;
        // Calcular el ancho del prefab para no superponer
        Collider2D colPrefab = prefabTelarana.GetComponent<Collider2D>();
        if (colPrefab != null)
            anchoNueva = colPrefab.bounds.size.x;

        float posX;
        if (colRef != null)
        {
            // Posicionar a la izquierda o derecha del borde de la referencia
            float dir = Random.value < 0.5f ? -1f : 1f;
            float bordeRef = dir > 0 ? colRef.bounds.max.x : colRef.bounds.min.x;
            posX = bordeRef + dir * (anchoNueva * 0.5f);
        }
        else
        {
            float dir = Random.value < 0.5f ? -1f : 1f;
            posX = referencia.position.x + dir * (anchoNueva > 0 ? anchoNueva : 3f);
        }

        Vector3 pos = new Vector3(posX, referencia.position.y, 0f);

        // Verificar que no se superponga con ninguna existente
        bool superpone = false;
        foreach (var t in telaranas)
        {
            if (t == null) continue;
            Collider2D c = t.GetComponent<Collider2D>();
            if (c != null)
            {
                float dist = Mathf.Abs(pos.x - t.position.x);
                float anchoExistente = c.bounds.size.x;
                if (dist < (anchoExistente * 0.5f + anchoNueva * 0.5f) * 0.8f)
                {
                    superpone = true;
                    break;
                }
            }
        }

        if (superpone) return;

        GameObject nueva = Instantiate(prefabTelarana, pos, Quaternion.identity);
        telaranas.Add(nueva.transform);
    }

    // ??????????????????????????????????????????
    //  ESTADO: EN TELARAÑA
    // ??????????????????????????????????????????
    private void LogicaTelarana()
    {
        rb.gravityScale = 0f;

        if (pausadoPorAtaque) return;

        if (telaranaActual != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                puntoPatrulla,
                velocidadEnTelarana * Time.deltaTime);

            if (Vector2.Distance(transform.position, puntoPatrulla) < 0.2f)
            {
                if (telaranas.Count > 1 && Random.value < 0.3f)
                {
                    ElegirTelaranaDestino();
                    telaranaActual = telaranaDestino;
                }
                ElegirPuntoPatrulla();
            }
        }

        timerAtaque -= Time.deltaTime;
        if (timerAtaque <= 0f)
        {
            EjecutarAtaqueDesdeTelarana();
            timerAtaque = tiempoEntreAtaques;
        }
    }

    private void PrepararAtaquesTelarana()
    {
        ataquesPendientesEnTelarana = Random.Range(2, 5);
    }

    private void EjecutarAtaqueDesdeTelarana()
    {
        if (ataquesPendientesEnTelarana <= 0)
        {
            int ataqueEspecial = Random.Range(0, 2);
            if (ataqueEspecial == 0)
                IniciarEmbestida();
            else
                IniciarSalto();
            return;
        }

        ataquesPendientesEnTelarana--;
        DispararProyectil();
    }

    // ??? Ataque 1: Proyectil ???
    private void DispararProyectil()
    {
        StartCoroutine(DispararProyectilConPausa());
    }

    private IEnumerator DispararProyectilConPausa()
    {
        pausadoPorAtaque = true;

        if (animator != null) animator.SetTrigger(AnimRangeAttack);

        if (prefabProyectil != null && puntoDisparo != null)
        {
            Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(duracionPausaDisparo);

        pausadoPorAtaque = false;
    }

    // ??? Ataque 2: Embestida ???
    private void IniciarEmbestida()
    {
        if (animator != null) animator.SetTrigger(AnimAttackJumpDash);

        estadoActual = Estado.Bajando;
        embistiendo = true;
        saltando = false;
        cayendo = false;
        rb.gravityScale = 0f;
        timerBajando = 0f;

        direccionEmbestida = ((Vector2)(jugador.position - transform.position)).normalized;
    }

    // ??? Ataque 3: Salto arriba + caída ???
    private void IniciarSalto()
    {
        estadoActual = Estado.Bajando;
        saltando = true;
        cayendo = false;
        embistiendo = false;
        rb.gravityScale = 0f;
        timerBajando = 0f;
        rb.velocity = Vector2.up * fuerzaSaltoArriba;

        StartCoroutine(EsperarYCaer());
    }

    private IEnumerator EsperarYCaer()
    {
        yield return new WaitForSeconds(0.8f);
        rb.velocity = Vector2.zero;
        posicionObjetivoSalto = jugador.position;
        // Posicionar la araña justo encima de la posición capturada del jugador
        transform.position = new Vector2(posicionObjetivoSalto.x, transform.position.y);
        yield return new WaitForSeconds(0.3f);
        cayendo = true;
        if (animator != null) animator.SetTrigger(AnimAttackJumpDash);
    }

    // ??????????????????????????????????????????
    //  ESTADO: BAJANDO (embestida o salto)
    // ??????????????????????????????????????????
    private void LogicaBajando()
    {
        timerBajando += Time.deltaTime;

        // Seguridad: si lleva demasiado tiempo bajando, forzar transición
        if (timerBajando > 4f)
        {
            embistiendo = false;
            saltando = false;
            cayendo = false;
            EntrarEstadoSuelo();
            return;
        }

        if (embistiendo)
        {
            rb.velocity = direccionEmbestida * velocidadEmbestida;

            if (enSuelo)
            {
                embistiendo = false;
                EntrarEstadoSuelo();
            }
        }
        else if (saltando)
        {
            if (cayendo)
            {
                // Caer recto sin moverse horizontalmente
                rb.velocity = new Vector2(0f, -velocidadCaida);

                if (enSuelo)
                {
                    ImpactoCaida();
                    saltando = false;
                    cayendo = false;
                    EntrarEstadoSuelo();
                }
            }
        }
    }

    private void ImpactoCaida()
    {
        Vector2 puntoImpacto = controladorSuelo != null
            ? (Vector2)controladorSuelo.position
            : (Vector2)transform.position;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(puntoImpacto, radioImpacto);
        foreach (var col in objetos)
        {
            if (col.CompareTag(tagJugador))
            {
                var hp = col.GetComponent<VidaPersonaje>();
                if (hp != null) hp.TomarDañoBarra(danoCaida);
            }
        }
    }

    // ??????????????????????????????????????????
    //  ESTADO: EN SUELO
    // ??????????????????????????????????????????
    private void EntrarEstadoSuelo()
    {
        estadoActual = Estado.EnSuelo;
        rb.gravityScale = 3f;
        rb.velocity = Vector2.zero;
        timerSuelo = tiempoEnSuelo;
        timerMelee = 0f;
    }

    private void LogicaSuelo()
    {
        timerSuelo -= Time.deltaTime;

        if (timerSuelo <= 0f)
        {
            IniciarSubida();
            return;
        }

        if (pausaDespuesMelee > 0f)
        {
            pausaDespuesMelee -= Time.deltaTime;
            rb.velocity = new Vector2(0f, rb.velocity.y);
            if (animator != null) animator.SetBool(AnimWalk, false);
            return;
        }

        float distanciaX = Mathf.Abs(transform.position.x - jugador.position.x);

        if (distanciaX <= distanciaMelee && timerMelee <= 0f)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            AtaqueMelee();
            timerMelee = cooldownMelee;
            pausaDespuesMelee = duracionPausaMelee;
        }
        else if (timerMelee > 0f)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            if (animator != null) animator.SetBool(AnimWalk, false);
        }
        else
        {
            CaminarHaciaJugador();
        }

        timerMelee -= Time.deltaTime;

        if (animator != null)
            animator.SetBool(AnimWalk, Mathf.Abs(rb.velocity.x) > 0.1f);
    }

    private void CaminarHaciaJugador()
    {
        float dir = Mathf.Sign(jugador.position.x - transform.position.x);
        rb.velocity = new Vector2(dir * velocidadCaminar, rb.velocity.y);
    }

    private void AtaqueMelee()
    {
        if (animator != null) animator.SetTrigger(AnimMelee);
    }

    // Llamar desde Animation Event en el frame de impacto de MeleeAttack
    public void EjecutarDanoMelee()
    {
        Vector2 puntoAtaque = controladorAtaqueMelee != null
            ? (Vector2)controladorAtaqueMelee.position
            : (Vector2)transform.position;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(puntoAtaque, radioAtaqueMelee);
        foreach (var col in objetos)
        {
            if (col.CompareTag(tagJugador))
            {
                var hp = col.GetComponent<VidaPersonaje>();
                if (hp != null) hp.TomarDañoBarra(danoMelee);
            }
        }
    }

    // ??????????????????????????????????????????
    //  ESTADO: SUBIENDO
    // ??????????????????????????????????????????
    private void IniciarSubida()
    {
        estadoActual = Estado.Subiendo;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        if (animator != null) animator.SetBool(AnimWalk, false);

        RecolectarTelaranas();
        telaranaActual = TelaranaMasCercana();
    }

    private void LogicaSubiendo()
    {
        if (telaranaActual == null)
        {
            RecolectarTelaranas();
            telaranaActual = TelaranaMasCercana();
            if (telaranaActual == null) return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            telaranaActual.position,
            velocidadSubir * Time.deltaTime);

        if (Vector2.Distance(transform.position, telaranaActual.position) < 0.2f)
        {
            estadoActual = Estado.EnTelarana;
            ElegirPuntoPatrulla();
            PrepararAtaquesTelarana();
            timerAtaque = tiempoEntreAtaques;
        }
    }

    // ??????????????????????????????????????????
    //  BARRA DE VIDA (creada por código)
    // ??????????????????????????????????????????
    private void CrearBarraDeVida()
    {
        // Canvas
        canvasBarraVida = new GameObject("CanvasBarraVidaSpider");
        Canvas canvas = canvasBarraVida.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = canvasBarraVida.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasBarraVida.AddComponent<GraphicRaycaster>();

        // Slider — ocupa ~7/9 del ancho de pantalla, centrado abajo
        // 7/9 = 0.777... ? anchors de 0.111 a 0.888
        GameObject sliderObj = new GameObject("SliderVidaSpider");
        sliderObj.transform.SetParent(canvasBarraVida.transform, false);

        sliderVida = sliderObj.AddComponent<Slider>();
        sliderVida.minValue = 0f;
        sliderVida.maxValue = vidaMaxima;
        sliderVida.value = vida;
        sliderVida.interactable = false;

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(1f / 9f, 0f);
        sliderRect.anchorMax = new Vector2(8f / 9f, 0f);
        sliderRect.pivot = new Vector2(0.5f, 0f);
        sliderRect.anchoredPosition = new Vector2(0f, 30f);
        sliderRect.sizeDelta = new Vector2(0f, 60f);

        // Borde negro
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(sliderObj.transform, false);
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = Color.black;
        RectTransform borderRect = borderObj.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(6f, 6f);

        // Background (dentro del borde)
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;

        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillArea.transform, false);
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = new Color(0.8f, 0f, 0f, 1f);
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        sliderVida.fillRect = fillRect;

        // Nombre del enemigo — centrado dentro de la barra
        GameObject textObj = new GameObject("TextoNombreBoss");
        textObj.transform.SetParent(sliderObj.transform, false);
        Text texto = textObj.AddComponent<Text>();
        texto.text = nombreEnemigo;
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize = 28;
        texto.fontStyle = FontStyle.Bold;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = Color.white;
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2f, -2f);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = Vector2.zero;
    }

    // ??????????????????????????????????????????
    //  DAÑO Y MUERTE
    // ??????????????????????????????????????????
    public void TomarDaño(float daño)
    {
        if (estaMuerto) return;

        vida -= daño;
        if (sliderVida != null) sliderVida.value = vida;

        if (vida <= 0f)
        {
            vida = 0f;
            if (sliderVida != null) sliderVida.value = 0f;
            estaMuerto = true;
            StopAllCoroutines();
            embistiendo = false;
            saltando = false;
            cayendo = false;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 3f;
            if (animator != null) animator.SetTrigger(AnimDeath);
        }
    }

    // Llamar desde Animation Event en el último frame de SpiderDeath
    public void EventoMuerte()
    {
        if (canvasBarraVida != null) Destroy(canvasBarraVida);
        Destroy(gameObject);
    }

    // ??????????????????????????????????????????
    //  COLISIÓN (daño de embestida)
    // ??????????????????????????????????????????
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(tagJugador)) return;

        if (embistiendo)
        {
            var hp = collision.collider.GetComponent<VidaPersonaje>();
            if (hp != null) hp.TomarDañoBarra(danoEmbestida);
            embistiendo = false;
            EntrarEstadoSuelo();
        }
        else if (saltando && cayendo)
        {
            var hp = collision.collider.GetComponent<VidaPersonaje>();
            if (hp != null) hp.TomarDañoBarra(danoCaida);
            saltando = false;
            cayendo = false;
            EntrarEstadoSuelo();
        }
    }

    // ??????????????????????????????????????????
    //  UTILIDADES
    // ??????????????????????????????????????????
    private void MirarHaciaJugador()
    {
        float dx = jugador.position.x - transform.position.x;
        if (Mathf.Abs(dx) < 0.05f) return;

        bool deberiaDerechas = dx > 0;
        if (deberiaDerechas != mirandoDerecha)
        {
            mirandoDerecha = deberiaDerechas;
            transform.eulerAngles = new Vector3(0, mirandoDerecha ? 0 : 180, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (controladorSuelo != null)
            Gizmos.DrawWireCube(controladorSuelo.position, dimensionCajaSuelo);

        Gizmos.color = Color.red;
        if (controladorAtaqueMelee != null)
            Gizmos.DrawWireSphere(controladorAtaqueMelee.position, radioAtaqueMelee);

        Gizmos.color = Color.cyan;
        if (controladorSuelo != null)
            Gizmos.DrawWireSphere(controladorSuelo.position, radioImpacto);
        else
            Gizmos.DrawWireSphere(transform.position, radioImpacto);
    }
}
