using UnityEngine;

public class ConsumibleMundo : MonoBehaviour
{
    [SerializeField] private float amplitud = 0.2f;
    [SerializeField] private float velocidadFlotacion = 2f;
    [SerializeField] private float velocidadRotacion = 60f;

    private Vector3 posicionInicial;

    private void Start()
    {
        posicionInicial = transform.position;
    }

    private void Update()
    {
        transform.position = posicionInicial + Vector3.up * Mathf.Sin(Time.time * velocidadFlotacion) * amplitud;
        transform.Rotate(Vector3.forward, velocidadRotacion * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PJ")) return;

        if (GestorConsumibles.Instance != null && GestorConsumibles.Instance.AgregarConsumible())
            Destroy(gameObject);
    }
}

