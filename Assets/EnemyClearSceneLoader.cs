using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyClearSceneLoader : MonoBehaviour
{
    [Header("Enemies (asignar por Inspector)")]
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "Nivel 7";

    [Header("Check")]
    [SerializeField] private float checkInterval = 0.25f;

    private bool _loaded;

    private void Start()
    {
        InvokeRepeating(nameof(CheckEnemies), 0f, checkInterval);
    }

    private void CheckEnemies()
    {
        if (_loaded) return;
        if (enemies == null || enemies.Count == 0) return;

        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject e = enemies[i];

            if (e == null) continue;

            if (e.activeInHierarchy)
                return;
        }

        _loaded = true;
        CancelInvoke(nameof(CheckEnemies));

        SceneManager.LoadScene(sceneToLoad);
    }

    public void SetEnemies(List<GameObject> newEnemies)
    {
        enemies = newEnemies;
        _loaded = false;
        CheckEnemies();
    }
}