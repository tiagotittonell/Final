using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Enemigo3 : MonoBehaviour
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
    public float tiempo_entre_ataques;
    private bool puedeAtacar = true; // Controla si el enemigo puede atacar nuevamente
    public GameObject rango;
    public GameObject Hit;

    // Variables para el tiempo de reacción del jugador
    public float tiempo_reaccion = 1.5f;
    private bool puedeRecibirDanio = false;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        target = GameObject.Find("PJ");
    }

    private void FixedUpdate()
    {
        Comportamientos();
    }

    public void Comportamientos()
    {
        if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_vision && !atacando)
        {
            ani.SetBool("Correr", false);
            cronometro += Time.deltaTime;
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
                if (!atacando && puedeAtacar)
                {
                    atacando = true;
                    ani.SetBool("Caminar", false);
                    ani.SetBool("Correr", false);
                    ani.SetTrigger("Ataque");
                }
            }
        }
    }

    // Método invocado por el evento de la animación de ataque para realizar el ataque
    public void RealizarAtaque()
    {
        rango.GetComponent<BoxCollider2D>().enabled = true;
        Hit.GetComponent<BoxCollider2D>().enabled = true;
    }

    // Método invocado por el evento de la animación de ataque para finalizar el ataque
    public void FinalizarAtaque()
    {
        atacando = false;
        rango.GetComponent<BoxCollider2D>().enabled = false;
        Hit.GetComponent<BoxCollider2D>().enabled = false;
        Invoke("HabilitarReaccion", tiempo_reaccion); // Habilitar la reacción después de un tiempo de reacción
    }

    private void HabilitarReaccion()
    {
        puedeRecibirDanio = true;
    }

    // Método para que el jugador ataque al enemigo
    public void AtacarEnemigo()
    {
        if (puedeRecibirDanio)
        {
            // Realizar las acciones cuando el enemigo es golpeado por el jugador
            // Por ejemplo, reducir la vida del enemigo
            // También podrías reproducir una animación de golpe al enemigo si es necesario

            // Desactivar la posibilidad de recibir daño nuevamente hasta el próximo ataque del enemigo
            puedeRecibirDanio = false;
        }
    }

    ////CODIGO TERCIARIO
    //public int rutina;
    //public float cronometro;
    //public Animator ani;
    //public int direccion;
    //public float speed_walk;
    //public float speed_run;
    //public GameObject target;
    //public bool atacando;

    //public float rango_vision;
    //public float rango_ataque;
    //public GameObject rango;
    //public GameObject Hit;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    ani = GetComponent<Animator>();
    //    target = GameObject.Find("PJ");
    //}

    //private void FixedUpdate()
    //{
    //    Comportamientos();
    //}

    //public void Comportamientos()
    //{
    //    if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_vision && !atacando)
    //    {
    //        ani.SetBool("Correr", false);
    //        cronometro += Time.deltaTime;
    //        if (cronometro >= 4)
    //        {
    //            rutina = Random.Range(0, 2);
    //            cronometro = 0;
    //        }
    //        switch (rutina)
    //        {
    //            case 0:
    //                ani.SetBool("Caminar", false);
    //                break;

    //            case 1:
    //                direccion = Random.Range(0, 2);
    //                rutina++;
    //                break;

    //            case 2:
    //                switch (direccion)
    //                {
    //                    case 0:
    //                        transform.rotation = Quaternion.Euler(0, 0, 0);
    //                        transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
    //                        break;

    //                    case 1:
    //                        transform.rotation = Quaternion.Euler(0, 180, 0);
    //                        transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
    //                        break;
    //                }
    //                ani.SetBool("Caminar", true);
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_ataque && !atacando)
    //        {
    //            if (transform.position.x < target.transform.position.x)
    //            {
    //                ani.SetBool("Caminar", false);
    //                ani.SetBool("Correr", true);
    //                transform.Translate(Vector3.right * speed_run * Time.deltaTime);
    //                transform.rotation = Quaternion.Euler(0, 0, 0);
    //                ani.SetBool("Ataque", false);
    //            }
    //            else
    //            {
    //                ani.SetBool("Caminar", false);
    //                ani.SetBool("Correr", true);
    //                transform.Translate(Vector3.right * speed_run * Time.deltaTime);
    //                transform.rotation = Quaternion.Euler(0, 180, 0);
    //                ani.SetBool("Ataque", false);
    //            }
    //        }
    //        else
    //        {
    //            if (!atacando)
    //            {
    //                atacando = true;
    //                ani.SetBool("Caminar", false);
    //                ani.SetBool("Correr", false);
    //                ani.SetTrigger("Ataque");
    //            }
    //        }
    //    }
    //}

    //// Método invocado por el evento de la animación de ataque para realizar el ataque
    //public void RealizarAtaque()
    //{
    //    rango.GetComponent<BoxCollider2D>().enabled = true;
    //    Hit.GetComponent<BoxCollider2D>().enabled = true;
    //}

    //// Método invocado por el evento de la animación de ataque para finalizar el ataque
    //public void FinalizarAtaque()
    //{
    //    atacando = false;
    //    rango.GetComponent<BoxCollider2D>().enabled = false;
    //    Hit.GetComponent<BoxCollider2D>().enabled = false;
    //}
    ////{CODIGO PRIMARIO
    //public int rutina;
    //public float cronometro;
    //public Animator ani;
    //public int direccion;
    //public float speed_walk;
    //public float speed_run;
    //public GameObject target;
    //public bool atacando;

    //public float rango_vision;
    //public float rango_ataque;
    //public GameObject rango;
    //public GameObject Hit;


    //// Start is called before the first frame update
    //void Start()
    //{
    //    ani = GetComponent<Animator>();
    //    target = GameObject.Find("PJ");
    //}

    //public void Comportamientos()
    //{
    //    if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_vision && !atacando)
    //    {
    //        ani.SetBool("Correr", false);
    //        cronometro += 1 * Time.deltaTime;
    //        if (cronometro >= 4)
    //        {
    //            rutina = Random.Range(0, 2);
    //            cronometro = 0;
    //        }
    //        switch (rutina)
    //        {
    //            case 0:
    //                ani.SetBool("Caminar", false);
    //                break;

    //            case 1:
    //                direccion = Random.Range(0, 2);
    //                rutina++;
    //                break;

    //            case 2:

    //                switch (direccion)
    //                {
    //                    case 0:
    //                        transform.rotation = Quaternion.Euler(0, 0, 0);
    //                        transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
    //                        break;

    //                    case 1:
    //                        transform.rotation = Quaternion.Euler(0, 180, 0);
    //                        transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
    //                        break;
    //                }
    //                ani.SetBool("Caminar", true);
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_ataque && !atacando)
    //        {
    //            if (transform.position.x < target.transform.position.x)
    //            {
    //                ani.SetBool("Caminar", false);
    //                ani.SetBool("Correr", true);
    //                transform.Translate(Vector3.right * speed_run * Time.deltaTime);
    //                transform.rotation = Quaternion.Euler(0, 0, 0);
    //                ani.SetBool("Ataque", false);
    //            }
    //            else
    //            {
    //                ani.SetBool("Caminar", false);
    //                ani.SetBool("Correr", true);
    //                transform.Translate(Vector3.right * speed_run * Time.deltaTime);
    //                transform.rotation = Quaternion.Euler(0, 180, 0);
    //                ani.SetBool("Ataque", false);
    //            }
    //        }
    //        else
    //        {
    //            if (!atacando)
    //            {
    //                if (transform.position.x < target.transform.position.x)
    //                {
    //                    transform.rotation = Quaternion.Euler(0, 0, 0);
    //                }
    //                else
    //                {
    //                    transform.rotation = Quaternion.Euler(0, 180, 0);
    //                }
    //                ani.SetBool("Caminar", false);
    //                ani.SetBool("Correr", false);
    //            }
    //        }
    //    }
    //}

    //public void Final_Ani()
    //{
    //    ani.SetBool("Ataque", false);
    //    atacando = false;
    //    rango.GetComponent<BoxCollider2D>().enabled = true;
    //}
    //public void ColliderWeaponTrue()
    //{
    //    Hit.GetComponent<BoxCollider2D>().enabled = true;
    //}
    //public void ColliderWeaponFalse()
    //{
    //    Hit.GetComponent<BoxCollider2D>().enabled = false;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    Comportamientos();
    //}
}
//CODIGO SECUNDARIO PRUEBA
//    public int rutina;
//    public float cronometro;
//    public Animator ani;
//    public int direccion;
//    public float speed_walk;
//    public float speed_run;
//    public GameObject target;
//    public bool atacando;

//    public float rango_vision;
//    public float rango_ataque;
//    public float tiempo_entre_ataques;
//    private bool puedeAtacar = true; // Controla si el enemigo puede atacar nuevamente
//    public GameObject rango;
//    public GameObject Hit;

//    // Start is called before the first frame update
//    void Start()
//    {
//        ani = GetComponent<Animator>();
//        target = GameObject.Find("PJ");
//    }
//    private void FixedUpdate()
//    {

//            Comportamientos();

//    }
//    public void Comportamientos()
//    {
//        if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_vision && !atacando)
//        {
//            ani.SetBool("Correr", false);
//            cronometro += Time.deltaTime;
//            if (cronometro >= 4)
//            {
//                rutina = Random.Range(0, 2);
//                cronometro = 0;
//            }
//            switch (rutina)
//            {
//                case 0:
//                    ani.SetBool("Caminar", false);
//                    break;

//                case 1:
//                    direccion = Random.Range(0, 2);
//                    rutina++;
//                    break;

//                case 2:

//                    switch (direccion)
//                    {
//                        case 0:
//                            transform.rotation = Quaternion.Euler(0, 0, 0);
//                            transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
//                            break;

//                        case 1:
//                            transform.rotation = Quaternion.Euler(0, 180, 0);
//                            transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
//                            break;
//                    }
//                    ani.SetBool("Caminar", true);
//                    break;
//            }
//        }
//        else
//        {
//            if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_ataque && !atacando)
//            {
//                if (transform.position.x < target.transform.position.x)
//                {
//                    ani.SetBool("Caminar", false);
//                    ani.SetBool("Correr", true);
//                    transform.Translate(Vector3.right * speed_run * Time.deltaTime);
//                    transform.rotation = Quaternion.Euler(0, 0, 0);
//                    ani.SetBool("Ataque", false);
//                }
//                else
//                {
//                    ani.SetBool("Caminar", false);
//                    ani.SetBool("Correr", true);
//                    transform.Translate(Vector3.right * speed_run * Time.deltaTime);
//                    transform.rotation = Quaternion.Euler(0, 180, 0);
//                    ani.SetBool("Ataque", false);
//                }
//            }
//            else
//            {
//                if (!atacando && puedeAtacar)
//                {
//                    atacando = true;
//                    puedeAtacar = false;
//                    Invoke("RealizarAtaque", 0.5f);
//                    Invoke("ResetearPuedeAtacar", tiempo_entre_ataques); // Restablecer el valor de puedeAtacar después del tiempo de enfriamiento
//                }
//                if (!atacando)
//                {
//                    if (transform.position.x < target.transform.position.x)
//                    {
//                        transform.rotation = Quaternion.Euler(0, 0, 0);
//                    }
//                    else
//                    {
//                        transform.rotation = Quaternion.Euler(0, 180, 0);
//                    }
//                    ani.SetBool("Caminar", false);
//                    ani.SetBool("Correr", false);
//                }
//            }
//        }
//    }

//    public void RealizarAtaque()
//    {
//        // Aquí puedes poner el código para realizar el ataque
//        ani.SetBool("Ataque", true);

//        rango.GetComponent<BoxCollider2D>().enabled = false;
//        Invoke("Final_Ani", 0.5f);
//    }

//    public void Final_Ani()
//    {
//        ani.SetBool("Ataque", false);
//        atacando = false;
//        rango.GetComponent<BoxCollider2D>().enabled = true;
//    }

//    public void ColliderWeaponTrue()
//    {
//        Hit.GetComponent<BoxCollider2D>().enabled = true;
//    }

//    public void ColliderWeaponFalse()
//    {
//        Hit.GetComponent<BoxCollider2D>().enabled = false;
//    }

//    private void ResetearPuedeAtacar()
//    {
//        puedeAtacar = true;
//    }

//    // Método para detener el ataque
//    public void DetenerAtaque()
//    {
//        atacando = false;
//    }

//    // Update is called once per frame



