using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullProjectile : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string targetTag = "PJ";
    private Transform _target;

    [Header("Movement")]
    [SerializeField] private float speed = 14f;          
    [SerializeField] private bool homing = false;        
    [SerializeField] private float turnSpeed = 12f;      

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 5f;

    [Header("Damage")]
    [SerializeField] private int damage = 50;

    [Header("Collision")]
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private string wallTag = "Wall";

    private Vector2 _dir;

    private void Awake()
    {
    }

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag(targetTag);
        if (go != null) _target = go.transform;

        if (_target != null)
            _dir = (_target.position - transform.position).normalized;
        else
            _dir = Vector2.right;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (homing && _target != null)
        {
            Vector2 desired = (_target.position - transform.position).normalized;
            _dir = Vector2.Lerp(_dir, desired, turnSpeed * Time.deltaTime).normalized;
        }

        transform.position += (Vector3)(_dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            var hp = other.GetComponent<VidaPersonaje>();
            if (hp != null)
                hp.TomarDañoBarra(damage);

            Destroy(gameObject);
            return;
        }
        if (other.CompareTag(groundTag) || other.CompareTag(wallTag))
        {
            Destroy(gameObject);
        }
    }
}
