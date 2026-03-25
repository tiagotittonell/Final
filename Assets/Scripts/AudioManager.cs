using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Personaje - Movimiento")]
    [SerializeField] private AudioClip[] pasos;
    [SerializeField] private AudioClip sonidoSalto;
    [SerializeField] private AudioClip sonidoAterrizaje;
    [SerializeField] private AudioClip sonidoDash;

    [Header("Personaje - Combate")]
    [SerializeField] private AudioClip[] sonidosGolpe;
    [SerializeField] private AudioClip sonidoDaño;
    [SerializeField] private AudioClip sonidoMuerte;

    [Header("Enemigos")]
    [SerializeField] private AudioClip sonidoDañoEnemigo;
    [SerializeField] private AudioClip sonidoMuerteEnemigo;

    [Header("Configuracion")]
    [Range(0f, 1f)][SerializeField] private float volumenSFX = 1f;

    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        sfxSource = GetComponent<AudioSource>();
    }

    // --- Personaje ---

    public void ReproducirPaso()
    {
        if (pasos == null || pasos.Length == 0) return;
        AudioClip clip = pasos[Random.Range(0, pasos.Length)];
        sfxSource.PlayOneShot(clip, volumenSFX);
    }

    public void ReproducirSalto()
    {
        if (sonidoSalto == null) return;
        sfxSource.PlayOneShot(sonidoSalto, volumenSFX);
    }

    public void ReproducirAterrizaje()
    {
        if (sonidoAterrizaje == null) return;
        sfxSource.PlayOneShot(sonidoAterrizaje, volumenSFX);
    }

    public void ReproducirDash()
    {
        if (sonidoDash == null) return;
        sfxSource.PlayOneShot(sonidoDash, volumenSFX);
    }

    public void ReproducirGolpe(int paso)
    {
        if (sonidosGolpe == null || sonidosGolpe.Length == 0) return;
        int indice = Mathf.Clamp(paso - 1, 0, sonidosGolpe.Length - 1);
        sfxSource.PlayOneShot(sonidosGolpe[indice], volumenSFX);
    }

    public void ReproducirDañoPersonaje()
    {
        if (sonidoDaño == null) return;
        sfxSource.PlayOneShot(sonidoDaño, volumenSFX);
    }

    public void ReproducirMuertePersonaje()
    {
        if (sonidoMuerte == null) return;
        sfxSource.PlayOneShot(sonidoMuerte, volumenSFX);
    }

    // --- Enemigos ---

    public void ReproducirDañoEnemigo()
    {
        if (sonidoDañoEnemigo == null) return;
        sfxSource.PlayOneShot(sonidoDañoEnemigo, volumenSFX);
    }

    public void ReproducirMuerteEnemigo()
    {
        if (sonidoMuerteEnemigo == null) return;
        sfxSource.PlayOneShot(sonidoMuerteEnemigo, volumenSFX);
    }
}
