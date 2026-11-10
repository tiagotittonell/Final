using UnityEngine;

public class SpiderWebProjectile : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string targetTag = "PJ";
    private Transform _target;

    [Header("Movement")]
    [SerializeField] private float speed = 210f;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 3f;

    [Header("Damage")]
    [SerializeField] private float damage = 10f;

    [Header("Ignorar")]
    [SerializeField] private string ignoreTag = "Enemigo";

    private Vector2 _dir;

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag(targetTag);
        if (go != null) _target = go.transform;

        if (_target != null)
            _dir = (_target.position - transform.position).normalized;
        else
            _dir = Vector2.down;

        float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(_dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ignoreTag)) return;

        if (other.CompareTag(targetTag))
        {
            var hp = other.GetComponent<VidaPersonaje>();
            if (hp != null)
                hp.TomarDañoBarra(damage);
        }

        Destroy(gameObject);
    }
}
