using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilidadEnemigo : MonoBehaviour
{
    [SerializeField] private float daño;

    [SerializeField] private Vector2 dimensionCaja;

    [SerializeField] private Transform posicionCaja;

    [SerializeField] private float tiempoVida;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, tiempoVida);

    }


    public void Golpear2()
    {
        Collider2D[] objetos = Physics2D.OverlapBoxAll(posicionCaja.position, dimensionCaja, 0f);
        foreach (Collider2D colisiones in objetos)
        {
            if (colisiones.CompareTag("PJ"))
            {
                colisiones.GetComponent<VidaPersonaje>().TomarDañoBarra(daño);
            }
        }
    }
    // Update is called once per frame
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(posicionCaja.position, dimensionCaja);
    }
}
