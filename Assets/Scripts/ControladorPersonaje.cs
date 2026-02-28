//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ControladorPersonaje : MonoBehaviour
//{
//    public Animator anim;
//    public Animation aanim;

//    [SerializeField] private Transform controladorGolpe;
//    [SerializeField] private float radioGolpe;
//    [SerializeField] private float dañoGolpe;
//    [SerializeField] private float tiempoEntreAtaques;
//    [SerializeField] private float tiempoSiguienteAtaque;

//    void Start()
//    {
//        anim = GetComponent<Animator>();
//    }

//    private void Update()
//    {
//        if (tiempoEntreAtaques > 0)
//        {
//            tiempoSiguienteAtaque -= Time.deltaTime;
//        }

//        if (Input.GetMouseButtonDown(0) && tiempoSiguienteAtaque <= 0)
//        {
//            Golpe();
//            tiempoSiguienteAtaque = tiempoEntreAtaques;
//        }
//    }

//    private void Golpe()
//    {
//        anim.SetTrigger("Golpe");

//        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);
//        foreach (Collider2D colisionador in objetos)
//        {
//            if (colisionador.CompareTag("Enemigo"))
//            {
//                IDamageable damageable = colisionador.GetComponent<IDamageable>();
//                if (damageable != null)
//                {
//                    damageable.TomarDaño(dañoGolpe);
//                }
//                else
//                {
//                    COPIAENEMIGOSCRIPT enemigoViejo = colisionador.GetComponent<COPIAENEMIGOSCRIPT>();
//                    if (enemigoViejo != null)
//                    {
//                        enemigoViejo.TomarDaño(dañoGolpe);
//                    }
//                }
//            }
//            else if (colisionador.CompareTag("Boss"))
//            {
//                Enemigo boss = colisionador.GetComponent<Enemigo>();
//                if (boss != null)
//                {
//                    boss.TomarDañoEnemigo(dañoGolpe);
//                }
//            }
//            else if (colisionador.CompareTag("Bell"))
//            {
//                BellInteractable bell = colisionador.GetComponent<BellInteractable>();

//                if (bell != null)
//                {
//                    Debug.Log($"🔔 Campana golpeada: {bell.name}");

//                    bell.Hit();
//                }
//                else
//                {
//                    Debug.LogWarning("Campana sin BellInteractable en: " + colisionador.name);
//                }
//            }
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
//    }
//}

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

    [Header("Combo")]
    [SerializeField] private float ventanaCombo = 0.3f;

    private bool atacando = false;
    private bool comboEnCola = false;
    private int pasoCombo = 0;
    private Coroutine coroutineAtaque;

    private static readonly string[] triggersPorPaso = { "", "Golpe", "Combo2", "Combo3" };

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!atacando)
            {
                IniciarAtaque(1);
            }
            else
            {
                comboEnCola = true;
            }
        }
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
                }
                else
                {
                    COPIAENEMIGOSCRIPT enemigoViejo = colisionador.GetComponent<COPIAENEMIGOSCRIPT>();
                    if (enemigoViejo != null)
                    {
                        enemigoViejo.TomarDaño(dañoGolpe);
                    }
                }
            }
            else if (colisionador.CompareTag("Boss"))
            {
                Enemigo boss = colisionador.GetComponent<Enemigo>();
                if (boss != null)
                {
                    boss.TomarDañoEnemigo(dañoGolpe);
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
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorGolpe == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }
}