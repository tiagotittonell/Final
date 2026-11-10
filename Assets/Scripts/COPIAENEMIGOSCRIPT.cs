using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COPIAENEMIGOSCRIPT : EnemigoBase
{


    public int rutina;
    public float cronometro;
    public Animator ani;
    public int direccion;
    public float speed_walk;
    public float speed_run;
    public GameObject target;
    public bool atacando;

    public float rango_vision;
    public float rango_ataque;
    public GameObject rango;
    public GameObject Hit;

    public Rigidbody2D rb;

    [SerializeField] private float vida;
    private bool estaMuerto = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        target = GameObject.Find("PJ");
    }

    public void Comportamientos()
    {
        if (estaMuerto) return; 

        if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_vision && !atacando)
        {
            ani.SetBool("Correr", false);
            cronometro += 1 * Time.deltaTime;
            if (cronometro >= 4)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }
            switch (rutina)
            {
                case 0:
                    ani.SetBool("Caminar", false);
                    break;

                case 1:
                    direccion = Random.Range(0, 2);
                    rutina++;
                    break;

                case 2:

                    switch (direccion)
                    {
                        case 0:
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                            transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
                            break;

                        case 1:
                            transform.rotation = Quaternion.Euler(0, 180, 0);
                            transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
                            break;
                    }
                    ani.SetBool("Caminar", true);
                    break;
            }
        }
        else
        {
            if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_ataque && !atacando)
            {
                if (transform.position.x < target.transform.position.x)
                {
                    ani.SetBool("Caminar", false);
                    ani.SetBool("Correr", true);
                    transform.Translate(Vector3.right * speed_run * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    ani.SetBool("Ataque", false);
                }
                else
                {
                    ani.SetBool("Caminar", false);
                    ani.SetBool("Correr", true);
                    transform.Translate(Vector3.right * speed_run * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    ani.SetBool("Ataque", false);
                }
            }
            else
            {
                if (!atacando)
                {
                    if (transform.position.x < target.transform.position.x)
                    {
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    ani.SetBool("Caminar", false);
                    ani.SetBool("Correr", false);
                }
            }
        }
    }

    public void Final_Ani()
    {
        if (estaMuerto) return; 
        ani.SetBool("Ataque", false);
        atacando = false;
        rango.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ColliderWeaponTrue()
    {
        if (estaMuerto) return; 
        Hit.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ColliderWeaponFalse()
    {
        if (estaMuerto) return; 
        Hit.GetComponent<BoxCollider2D>().enabled = false;
    }

    void Update()
    {
        Comportamientos();
    }

    public override void TomarDaño(float daño)
    {
        if (estaMuerto) return;

        base.TomarDaño(daño);

        vida -= daño;

        if (vida > 0)
        {
            ani.SetTrigger("Daño");
        }
        else
        {
            estaMuerto = true; 
            ani.SetTrigger("Muerte");

            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true; 
            GetComponent<Collider2D>().enabled = false;
            rango.GetComponent<Collider2D>().enabled = false;
            Hit.GetComponent<Collider2D>().enabled = false;

            StartCoroutine(DestruirEnemigo());
        }
    }

    private IEnumerator DestruirEnemigo()
    {
        yield return new WaitForSeconds(2);
        yield return new WaitForSeconds(ani.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    public void Muerte()
    {
        Destroy(gameObject);
    }

}
