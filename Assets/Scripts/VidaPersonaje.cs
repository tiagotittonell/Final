using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaPersonaje : MonoBehaviour
{
    [SerializeField] private float vida;
    [SerializeField] private BarraDeVida barraVida;
    private MovimientoPersonaje movimientoJugador;
    [SerializeField] private float tiempoPerdidaControl = 0.12f; 
    private Animator animator;
    [SerializeField] private float vidaMaxima;
    [SerializeField] private float fuerzaEmpuje = 10f; 
    private Rigidbody2D rb;
    private bool estaMuerto = false;

    [Header("Diagnóstico temporal")]
    [SerializeField] private bool registrarOrigenDaño = true;
    [SerializeField, Min(0.1f)] private float radioDiagnosticoDaño = 3f;

    private void Start()
    {
        vida = GestorJugador.tieneVidaGuardada ? GestorJugador.vidaGuardada : vidaMaxima;

        if (barraVida != null)
        {
            barraVida.InicializarBarraDeVida(vidaMaxima);
            barraVida.CambiarVidaActual(vida);
        }
        else
            Debug.LogError("[VidaPersonaje] barraVida no está asignada en el Inspector");

        movimientoJugador = GetComponent<MovimientoPersonaje>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (registrarOrigenDaño)
            StartCoroutine(RegistrarContactosIniciales());
    }
    private IEnumerator PerderControl(Vector2 dirKnockback)
    {
        movimientoJugador.sePuedeMover = false;
        rb.velocity = new Vector2(dirKnockback.x * fuerzaEmpuje, fuerzaEmpuje * 0.3f);
        yield return new WaitForSeconds(tiempoPerdidaControl);
        movimientoJugador.sePuedeMover = true;
    }

    private IEnumerator ProcesarMuerte()
    {
        yield return new WaitForEndOfFrame();
        animator.SetFloat("VelocidadY", 0f);
        animator.SetFloat("VelocidadX", 0f);
        animator.SetBool("enSuelo", true);
        animator.SetBool("Escalar", false);
        animator.SetBool("EsDobleSalto", false);
        animator.SetTrigger("Muerte");
        yield return new WaitForSeconds(2f);
        GestorEscenas.CargarGameOver();
    }

    public void TomarDañoBarra(float daño)
    {
        if (estaMuerto) return;

        vida -= daño;

        if (registrarOrigenDaño)
            RegistrarDiagnosticoDaño(daño, null);

        barraVida.CambiarVidaActual(vida);

        Vector2 dirKnockback = Vector2.right;
        Vector2 dirShake = Vector2.down;
        Collider2D[] cercanos = Physics2D.OverlapCircleAll(transform.position, 6f);
        foreach (Collider2D c in cercanos)
        {
            if (c.CompareTag("Enemigo") || c.CompareTag("Boss"))
            {
                Vector2 diff = (Vector2)(transform.position - c.transform.position);
                dirKnockback = diff.normalized;
                dirShake     = -dirKnockback;
                break;
            }
        }
        ControladorCamara.Instance?.ShakeDaño(dirShake);

        if (vida <= 0)
        {
            MatarInstantaneamente();
            return;
        }

        animator.ResetTrigger("Hurt");
        animator.SetTrigger("Hurt");

        StartCoroutine(PerderControl(dirKnockback));
    }

    public void MatarInstantaneamente(GameObject fuente = null)
    {
        if (estaMuerto) return;

        if (registrarOrigenDaño)
            RegistrarDiagnosticoDaño(vida, fuente);

        estaMuerto = true;
        vida = 0f;

        if (barraVida != null)
            barraVida.CambiarVidaActual(vida);

        if (movimientoJugador != null)
        {
            movimientoJugador.sePuedeMover = false;
            movimientoJugador.enabled = false;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.isKinematic = true;
        }

        StartCoroutine(ProcesarMuerte());
    }

    private IEnumerator RegistrarContactosIniciales()
    {
        yield return new WaitForFixedUpdate();

        Collider2D[] collidersJugador = GetComponentsInChildren<Collider2D>();
        ContactFilter2D filtro = new ContactFilter2D();
        filtro.NoFilter();
        Collider2D[] resultados = new Collider2D[32];
        StringBuilder mensaje = new StringBuilder();
        mensaje.AppendLine("[DIAGNÓSTICO PJ] Colliders tocando al jugador al comenzar la escena:");
        bool encontroContactos = false;

        foreach (Collider2D colliderJugador in collidersJugador)
        {
            int cantidad = colliderJugador.OverlapCollider(filtro, resultados);

            for (int i = 0; i < cantidad; i++)
            {
                Collider2D contacto = resultados[i];
                if (contacto == null || contacto.transform.IsChildOf(transform))
                    continue;

                encontroContactos = true;
                AgregarInformacionCollider(mensaje, contacto);
            }
        }

        if (!encontroContactos)
            mensaje.AppendLine("- Ningún collider externo está superpuesto con el jugador.");

        Debug.LogWarning(mensaje.ToString(), this);
    }

    private void RegistrarDiagnosticoDaño(float daño, GameObject fuente)
    {
        StringBuilder mensaje = new StringBuilder();
        mensaje.AppendLine("[DIAGNÓSTICO DAÑO PJ]");
        mensaje.AppendLine($"Daño recibido: {daño} | Vida resultante: {vida}");

        if (fuente != null)
        {
            mensaje.AppendLine(
                $"Fuente informada: '{fuente.name}' | Tag: '{fuente.tag}' | Layer: '{LayerMask.LayerToName(fuente.layer)}'");
        }
        else
        {
            mensaje.AppendLine("Fuente directa no informada. Revisar la pila de llamadas debajo del mensaje.");
        }

        Collider2D[] cercanos = Physics2D.OverlapCircleAll(transform.position, radioDiagnosticoDaño);
        mensaje.AppendLine($"Colliders dentro de {radioDiagnosticoDaño:0.##} unidades:");

        foreach (Collider2D cercano in cercanos)
        {
            if (cercano == null || cercano.transform.IsChildOf(transform))
                continue;

            AgregarInformacionCollider(mensaje, cercano);
        }

        Debug.LogWarning(mensaje.ToString(), fuente != null ? fuente : gameObject);
    }

    private static void AgregarInformacionCollider(StringBuilder mensaje, Collider2D colliderEncontrado)
    {
        GameObject objeto = colliderEncontrado.gameObject;
        mensaje.AppendLine(
            $"- Objeto: '{objeto.name}' | Collider: {colliderEncontrado.GetType().Name} | " +
            $"Tag: '{objeto.tag}' | Layer: '{LayerMask.LayerToName(objeto.layer)}' | " +
            $"Trigger: {colliderEncontrado.isTrigger} | Activo: {objeto.activeInHierarchy} | " +
            $"Ruta: {ObtenerRutaJerarquia(objeto.transform)}");
    }

    private static string ObtenerRutaJerarquia(Transform objetivo)
    {
        string ruta = objetivo.name;

        while (objetivo.parent != null)
        {
            objetivo = objetivo.parent;
            ruta = objetivo.name + "/" + ruta;
        }

        return ruta;
    }

    private void OnDestroy()
    {
        if (!estaMuerto && !GestorJugador.bloquearGuardado)
            GestorJugador.GuardarVida(vida);
    }

    public void Curar(float cantidad)
    {
        if (estaMuerto) return;
        vida = Mathf.Min(vida + cantidad, vidaMaxima);
        barraVida.CambiarVidaActual(vida);
    }

}
