using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] private string nombreEscena;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PJ"))
            SceneManager.LoadScene(nombreEscena);
    }
}
