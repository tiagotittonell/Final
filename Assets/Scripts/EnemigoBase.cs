using UnityEngine;

public class EnemigoBase : MonoBehaviour, IDamageable
{
    public virtual void TomarDaño(float daño)
    {
        AudioManager.Instance?.ReproducirDañoEnemigo();

        Vector2 dir = Vector2.right;
        GameObject pj = GameObject.FindGameObjectWithTag("PJ");
        if (pj != null)
            dir = ((Vector2)(transform.position - pj.transform.position)).normalized;

        ControladorCamara.Instance?.ShakeGolpe(dir);
    }
}
