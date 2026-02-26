using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class Palanca : MonoBehaviour {

    public Animator palancaAnimator;
    private bool palancaActivada = false;

    public GameObject puertaDesactivar;
    public GameObject puertaActivar;

    public Camera camaraPrincipal;
    public Camera camaraSecundaria;

    public float tiempoEsperaCamaraSecundaria = 1.0f;
    public float tiempoEsperaPuertas = 1.5f;
    public float tiempoEsperaRestaurarCamaraPrincipal = 4.0f;

    public TextMeshProUGUI textoInteraccion;
    private bool mostrarTexto = false;

    void Update()
    {
        if (mostrarTexto && Input.GetKeyDown(KeyCode.F))
        {
            palancaActivada = !palancaActivada;

        
            palancaAnimator.SetBool("PalancaActivada", palancaActivada);

            if (palancaActivada)
            {
                ActivarPuerta(puertaActivar);
                DesactivarPuerta(puertaDesactivar);

                Invoke("ActivarCamaraSecundaria", tiempoEsperaCamaraSecundaria);
            }
            else
            {
                ActivarPuerta(puertaDesactivar);
                DesactivarPuerta(puertaActivar);
            }
        }
    }

    private void ActivarCamaraSecundaria()
    {
        camaraPrincipal.enabled = false;
        camaraSecundaria.enabled = true;

        Invoke("ControlarPuertas", tiempoEsperaPuertas);
    }

    private void ControlarPuertas()
    {
        Invoke("DesactivarPuertaAnterior", tiempoEsperaPuertas + 1.5f);

    
        Invoke("ActivarNuevaPuerta", tiempoEsperaPuertas + 2.5f);

   
        Invoke("RestaurarCamaraPrincipal", tiempoEsperaRestaurarCamaraPrincipal);
    }

    private void DesactivarPuertaAnterior()
    {
        DesactivarPuerta(puertaDesactivar);
    }

    private void ActivarNuevaPuerta()
    {
        ActivarPuerta(puertaActivar);
    }

    private void RestaurarCamaraPrincipal()
    {
        camaraSecundaria.enabled = false;
        camaraPrincipal.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PJ"))
        {
            Debug.Log("Player cerca de la palanca");
            mostrarTexto = true;
            textoInteraccion.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PJ"))
        {
            mostrarTexto = false;
            textoInteraccion.gameObject.SetActive(false);
        }
    }

   
    private void ActivarPuerta(GameObject puerta)
    {
        if (puerta != null)
        {
            puerta.SetActive(true);
        }
    }

  
    private void DesactivarPuerta(GameObject puerta)
    {
        if (puerta != null)
        {
            puerta.SetActive(false);
        }
    }


   
}