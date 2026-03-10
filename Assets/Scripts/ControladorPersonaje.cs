using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorPersonaje : MonoBehaviour
{
    public Animator anim;

    [Header("Daño")]
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe = 1f;
    [SerializeField] private float dañoGolpe = 10f;
    [SerializeField] private GameObject prefabEfectoGolpe;

    [Header("Combo")]
    [SerializeField] private float ventanaCombo = 0.3f;

    [Header("Ataque Aéreo")]
    [SerializeField] private float escalaGravedadHangTime = 0.2f;
    [SerializeField] private float hangTimeDuracion = 0.18f;
    [SerializeField] private float impulsoLateralAereo = 2.5f;

    private bool atacando = false;
    private bool comboEnCola = false;
    private int pasoCombo = 0;
    private Coroutine coroutineAtaque;
    private MovimientoPersonaje movimiento;

    private static readonly string[] triggersPorPaso = { "", "Golpe", "Combo2", "Combo3" };

    void Start()
    {
        anim = GetComponent<Animator>();
        movimiento = GetComponent<MovimientoPersonaje>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!atacando)
                IniciarAtaque(1);
            else
                comboEnCola = true;
        }

        if (movimiento != null)
            anim.SetBool("AtacandoEnAire", atacando && !movimiento.EstaEnSuelo);
    }

    private void IniciarAtaque(int paso)
    {
        if (coroutineAtaque != null)
            StopCoroutine(coroutineAtaque);

        atacando = true;
        pasoCombo = paso;
        comboEnCola = false;

        anim.ResetTrigger("Golpe");
        anim.ResetTrigger("Combo2");
        anim.ResetTrigger("Combo3");
        anim.SetTrigger(triggersPorPaso[paso]);

        if (movimiento != null && !movimiento.EstaEnSuelo)
            movimiento.AplicarImpulsoAereo(hangTimeDuracion, escalaGravedadHangTime, impulsoLateralAereo);

        coroutineAtaque = StartCoroutine(EsperarFinAtaque());
    }

    private IEnumerator EsperarFinAtaque()
    {
        // Espera 2 frames para que el trigger tome efecto
        yield return null;
        yield return null;

        // Espera a que termine la transición de entrada al estado de ataque
        while (anim.IsInTransition(0))
            yield return null;

        // Espera a que el estado de ataque termine (normalizedTime >= 0.95 o empiece Exit Time)
        while (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
            yield return null;

        // Ventana: el jugador tiene ventanaCombo segundos para clickear el siguiente golpe
        float elapsed = 0f;
        while (!comboEnCola && elapsed < ventanaCombo)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (comboEnCola)
        {
            // Espera a que termine la transición a Idle antes de setear el siguiente trigger
            while (anim.IsInTransition(0))
                yield return null;

            int siguiente = (pasoCombo % 3) + 1; // 1→2→3→1
            IniciarAtaque(siguiente);
        }
        else
        {
            atacando = false;
            pasoCombo = 0;
        }
    }

    private void SpawnEfectoGolpe(Collider2D colisionador)
    {
        if (prefabEfectoGolpe == null) return;
        Vector2 direccion = ((Vector2)colisionador.bounds.center - (Vector2)controladorGolpe.position).normalized;
        Vector2 puntoImpacto = (Vector2)controladorGolpe.position + direccion * radioGolpe;

        float yRot = transform.localScale.x < 0f ? 180f : 0f;
        Instantiate(prefabEfectoGolpe, puntoImpacto, Quaternion.Euler(0f, yRot, 0f));
    }

    // ==========================
    // DAÑO — llamado desde Animation Event en el frame exacto del golpe
    // ==========================
    public void AplicarDaño()
    {
        if (controladorGolpe == null) return;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);
        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemigo"))
            {
                IDamageable damageable = colisionador.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TomarDaño(dañoGolpe);
                    SpawnEfectoGolpe(colisionador);
                }
                else
                {
                    COPIAENEMIGOSCRIPT enemigoViejo = colisionador.GetComponent<COPIAENEMIGOSCRIPT>();
                    if (enemigoViejo != null)
                    {
                        enemigoViejo.TomarDaño(dañoGolpe);
                        SpawnEfectoGolpe(colisionador);
                    }
                }
            }
            else if (colisionador.CompareTag("Boss"))
            {
                IDamageable boss = colisionador.GetComponent<IDamageable>();
                if (boss != null)
                {
                    boss.TomarDaño(dañoGolpe);
                    SpawnEfectoGolpe(colisionador);
                }
            }
            else if (colisionador.CompareTag("Bell"))
            {
                BellInteractable bell = colisionador.GetComponent<BellInteractable>();
                if (bell != null)
                {
                    bell.Hit();
                }
            }
            else if (colisionador.CompareTag("Breakable"))
            {
                DummyBreakable dummy = colisionador.GetComponent<DummyBreakable>();
                if (dummy != null)
                {
                    dummy.ReceiveHit();
                    SpawnEfectoGolpe(colisionador);
                }
                else
                {
                    DummyBrekeableTutorial2 dummyAire = colisionador.GetComponent<DummyBrekeableTutorial2>();
                    if (dummyAire != null)
                    {
                        dummyAire.ReceiveHit();
                        SpawnEfectoGolpe(colisionador);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorGolpe == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }
}