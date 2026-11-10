using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class ControladorCamara : MonoBehaviour
{
    public static ControladorCamara Instance { get; private set; }

    public Camera camaraPrincipal;
    public Camera camaraSecundaria;

    [Header("Shake")]
    [SerializeField] private float fuerzaShakeDaño = 1.5f;
    [SerializeField] private float fuerzaShakeGolpe = 0.6f;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        if (camaraPrincipal != null && camaraSecundaria != null)
            camaraSecundaria.depth = camaraPrincipal.depth + 1;
    }

    public void ShakeDaño(Vector2 direccion)
    {
        impulseSource.GenerateImpulseWithVelocity(direccion * fuerzaShakeDaño);
    }

    public void ShakeGolpe(Vector2 direccion)
    {
        impulseSource.GenerateImpulseWithVelocity(direccion * fuerzaShakeGolpe);
    }
}
