using System.Collections;
using UnityEngine;

public class EnemyFlySkull : MonoBehaviour, IDamageable
{
    private enum State { IdleFly, Sleeping, Rising }

    [Header("Stats")]
    [SerializeField] private float vida = 3f;
    private bool estaMuerto = false;
    private bool estaDormido = false;

    [Header("Target")]
    [SerializeField] private string targetTag = "PJ";
    private Transform _target;

    [Header("Follow")]
    [SerializeField] private float followSpeedX = 2f;
    [SerializeField] private float followRangeX = 6f;

    [Header("Hover")]
    [SerializeField] private float hoverAmplitude = 0.15f;
    [SerializeField] private float hoverSpeed = 2f;
    private float _baseY;
    private float _flyY;

    [Header("Wake Up Rise")]
    [SerializeField] private float riseSpeedY = 4f;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float timeBetweenShots = 3f;

    private float _shotTimer;
    private int _shotsDone;
    private int _shotsBeforeSleep;

    [Header("Sleep")]
    [SerializeField] private float sleepDuration = 5f;
    private float _sleepTimer;

    [Header("Colliders")]
    [SerializeField] private Collider2D hittableCollider;
    [SerializeField] private Collider2D mainCollider;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float sleepGravity = 3f;
    private RigidbodyConstraints2D _defaultConstraints;

    [Header("Animator")]
    [SerializeField] private Animator ani;

    private static readonly int TrgAttack = Animator.StringToHash("Attack");
    private static readonly int TrgSleep = Animator.StringToHash("Sleep");
    private static readonly int TrgWakeUp = Animator.StringToHash("WakeUp");
    private static readonly int TrgHit = Animator.StringToHash("HitDamage");

    private State _state = State.IdleFly;

    private void Awake()
    {
        if (ani == null) ani = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        _flyY = transform.position.y;
        _baseY = _flyY;

        if (rb != null)
        {
            _defaultConstraints = rb.constraints;
            rb.gravityScale = 0f;
        }

        if (hittableCollider != null)
            hittableCollider.enabled = false;

        ResetAttackCycle();
        _shotTimer = timeBetweenShots;
    }

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag(targetTag);
        if (go != null) _target = go.transform;
    }

    private void Update()
    {
        if (estaMuerto || _target == null) return;

        FaceTarget();

        switch (_state)
        {
            case State.IdleFly:
                Hover();
                Follow();
                AttackLogic();
                break;

            case State.Sleeping:
                SleepLogic();
                break;

            case State.Rising:
                RiseLogic();
                break;
        }
    }

    private void Hover()
    {
        Vector3 pos = transform.position;
        pos.y = _baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = pos;
    }

    private void Follow()
    {
        float dx = _target.position.x - transform.position.x;
        if (Mathf.Abs(dx) <= followRangeX) return;

        float step = followSpeedX * Time.deltaTime;
        float newX = Mathf.MoveTowards(transform.position.x, _target.position.x, step);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void FaceTarget()
    {
        float dx = _target.position.x - transform.position.x;
        if (Mathf.Abs(dx) < 0.01f) return;

        Vector3 s = transform.localScale;
        s.x = dx > 0 ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        transform.localScale = s;
    }

    private void AttackLogic()
    {
        _shotTimer -= Time.deltaTime;

        if (_shotTimer <= 0f)
        {
            if (_shotsDone >= _shotsBeforeSleep)
            {
                EnterSleep();
                return;
            }

            Shoot();
        }
    }

    private void Shoot()
    {
        ani.SetTrigger(TrgAttack);

        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        }

        _shotsDone++;
        _shotTimer = timeBetweenShots;
    }

    private void EnterSleep()
    {
        _state = State.Sleeping;
        estaDormido = true;
        _sleepTimer = sleepDuration;

        if (hittableCollider != null)
            hittableCollider.enabled = true;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = sleepGravity;
        }

        ani.SetTrigger(TrgSleep);
    }

    private void SleepLogic()
    {
        _sleepTimer -= Time.deltaTime;
        if (_sleepTimer <= 0f)
        {
            ExitSleep();
        }
    }

    private void ExitSleep()
    {
        estaDormido = false;

        if (hittableCollider != null)
            hittableCollider.enabled = false;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            rb.constraints = _defaultConstraints;
        }

        ResetAttackCycle();
        ani.SetTrigger(TrgWakeUp);
        _state = State.Rising;
    }

    private void RiseLogic()
    {
        float newY = Mathf.MoveTowards(transform.position.y, _flyY, riseSpeedY * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (Mathf.Abs(newY - _flyY) < 0.01f)
        {
            _baseY = _flyY;
            _state = State.IdleFly;
        }
    }

    private void ResetAttackCycle()
    {
        _shotsDone = 0;
        _shotsBeforeSleep = Random.Range(3, 6);
    }

    public void TomarDaño(float daño)
    {
        if (estaMuerto) return;
        if (!estaDormido) return;

        vida -= daño;

        if (vida > 0)
        {
            ani.SetTrigger("Hit");
        }
        else
        {
            estaMuerto = true;
            StartCoroutine(Morir());
        }
    }

    private IEnumerator Morir()
    {
        ani.SetTrigger("Death");

        if (mainCollider != null)
            mainCollider.enabled = false;

        if (hittableCollider != null)
            hittableCollider.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}