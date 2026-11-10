using UnityEngine;

public class ObjectDamage : MonoBehaviour
{
    [Header("Daño que aplica el objeto")]
    [SerializeField] private int damage = 25;

    [Header("Tipo de detección")]
    [SerializeField] private bool useTrigger = true;
    [SerializeField] private bool useCollision = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTrigger) return;

        if (other.CompareTag("PJ"))
        {
            VidaPersonaje vida = other.GetComponent<VidaPersonaje>();

            if (vida != null)
            {
                Vector2 direccionGolpe = (other.transform.position - transform.position).normalized;

                //vida.TomarDaños(damage, direccionGolpe);
                vida.TomarDañoBarra(damage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!useCollision) return;

        if (collision.transform.CompareTag("PJ"))
        {
            VidaPersonaje vida = collision.transform.GetComponent<VidaPersonaje>();

            if (vida != null)
            {
                Vector2 direccionGolpe = (collision.transform.position - transform.position).normalized;

                //vida.TomarDaños(damage, direccionGolpe);
                vida.TomarDañoBarra(damage);
            }
        }
    }
}