using UnityEngine;

public class PalancaLevelDescend : MonoBehaviour
{
    [Header("Interacción")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;
    [SerializeField] private string tagJugador = "PJ";

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerActivacion = "Activar";

    [Header("Puzzle")]
    [SerializeField] private ControladorPalancasLevelDescend controladorPalancas;

    [Header("Resaltado blanco")]
    [SerializeField, Min(1f)] private float escalaOutline = 1.06f;

    private bool jugadorCerca;
    private bool activada;
    private SpriteRenderer[] outlineRenderers;
    private Material materialOutline;

    public bool EstaActivada => activada;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (controladorPalancas == null)
            controladorPalancas = FindObjectOfType<ControladorPalancasLevelDescend>();

        CrearOutlines();
        SetOutline(false);
    }

    private void Update()
    {
        if (!jugadorCerca || activada)
            return;

        if (Input.GetKeyDown(teclaInteraccion))
            ActivarPalanca();
    }

    private void ActivarPalanca()
    {
        if (activada)
            return;

        activada = true;
        SetOutline(false);

        if (animator != null && !string.IsNullOrWhiteSpace(triggerActivacion))
            animator.SetTrigger(triggerActivacion);

        if (controladorPalancas != null)
            controladorPalancas.RegistrarPalanca(this);
        else
            Debug.LogWarning($"[{nameof(PalancaLevelDescend)}] No hay un controlador de palancas asignado.", this);
    }

    private void CrearOutlines()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);

        Shader fillShader = Resources.Load<Shader>("SpriteFill");
        if (fillShader == null)
            fillShader = Shader.Find("Custom/SpriteFill");

        if (fillShader != null)
            materialOutline = new Material(fillShader) { color = Color.white };

        outlineRenderers = new SpriteRenderer[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            SpriteRenderer spriteOriginal = sprites[i];
            GameObject outlineGO = new GameObject($"Outline_{spriteOriginal.gameObject.name}");
            outlineGO.transform.SetParent(spriteOriginal.transform, false);
            outlineGO.transform.localScale = Vector3.one * escalaOutline;

            SpriteRenderer outline = outlineGO.AddComponent<SpriteRenderer>();
            outline.sprite = spriteOriginal.sprite;
            outline.color = Color.white;
            outline.flipX = spriteOriginal.flipX;
            outline.flipY = spriteOriginal.flipY;
            outline.sortingLayerID = spriteOriginal.sortingLayerID;
            outline.sortingOrder = spriteOriginal.sortingOrder - 1;

            if (materialOutline != null)
                outline.material = materialOutline;

            outline.enabled = false;
            outlineRenderers[i] = outline;
        }
    }

    private void SetOutline(bool activo)
    {
        if (outlineRenderers == null)
            return;

        foreach (SpriteRenderer outline in outlineRenderers)
        {
            if (outline != null)
                outline.enabled = activo;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagJugador))
            return;

        jugadorCerca = true;

        if (!activada)
            SetOutline(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(tagJugador))
            return;

        jugadorCerca = false;
        SetOutline(false);
    }

    private void OnDestroy()
    {
        if (materialOutline != null)
            Destroy(materialOutline);
    }
}
