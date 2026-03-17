using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextTutorial : MonoBehaviour
{
    void Start()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PJ"))
        {

            SceneManager.LoadScene("Tutorial 2");

        }
    }
}
