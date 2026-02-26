using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvaPersonajeMuerto : MonoBehaviour
{
    public TextMeshProUGUI[] interactTexts; // Asigna tus objetos TextMeshPro al inspector
    public KeyCode interactKey = KeyCode.F;

    private bool isInRange = false;
    private int currentTextIndex = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PJ"))
        {
            isInRange = true;
            UpdateTextVisibility();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PJ"))
        {
            isInRange = false;
            interactTexts[currentTextIndex].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(interactKey))
        {
            ToggleTextMeshVisibility();
        }
    }

    void ToggleTextMeshVisibility()
    {
        interactTexts[currentTextIndex].gameObject.SetActive(false);

        // Cambia al siguiente TextMeshPro
        currentTextIndex = (currentTextIndex + 1) % interactTexts.Length;

        UpdateTextVisibility();
    }

    void UpdateTextVisibility()
    {
        for (int i = 0; i < interactTexts.Length; i++)
        {
            interactTexts[i].gameObject.SetActive(i == currentTextIndex);
        }
       
    }
}
