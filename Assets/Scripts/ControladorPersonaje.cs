//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ControladorPersonaje : MonoBehaviour
//{

//    public Animator anim;
//    public Animation aanim;

//    //private bool idleCombate = false;

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

//        if(tiempoEntreAtaques > 0)
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
//            {//{

//                colisionador.transform.GetComponent<COPIAENEMIGOSCRIPT>().TomarDaño(dañoGolpe);
//            }
//            else if (colisionador.CompareTag("Boss"))
//            {
//                colisionador.transform.GetComponent<Enemigo>().TomarDañoEnemigo(dañoGolpe);
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
    public Animation aanim;

    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe;
    [SerializeField] private float dañoGolpe;
    [SerializeField] private float tiempoEntreAtaques;
    [SerializeField] private float tiempoSiguienteAtaque;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (tiempoEntreAtaques > 0)
        {
            tiempoSiguienteAtaque -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0) && tiempoSiguienteAtaque <= 0)
        {
            Golpe();
            tiempoSiguienteAtaque = tiempoEntreAtaques;
        }
    }

    private void Golpe()
    {
        anim.SetTrigger("Golpe");

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
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }
}
