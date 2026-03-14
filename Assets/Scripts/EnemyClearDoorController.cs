using System.Collections.Generic;
using UnityEngine;

public class EnemyClearDoorController : MonoBehaviour
{
    [Header("Enemies (asignar por Inspector)")]
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    [Header("Almas (emparejar por índice con Enemies)")]
    [SerializeField] private List<SoulController> souls = new List<SoulController>();

    [Header("Door")]
    [SerializeField] private DoorControllerLVL4 door;

    [Header("Check")]
    [SerializeField] private float checkInterval = 0.25f;

    private bool _opened;

    private void Awake()
    {
        if (door == null)
            door = FindObjectOfType<DoorControllerLVL4>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckEnemies), 0f, checkInterval);
    }

    private void CheckEnemies()
    {
        if (_opened) return;
        if (door == null) return;
        if (enemies == null || enemies.Count == 0) return;

        bool todosEliminados = true;

        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject e = enemies[i];
            bool muerto = e == null || !e.activeInHierarchy;

            if (muerto)
            {
                if (i < souls.Count && souls[i] != null)
                {
                    souls[i].Desaparecer();
                    souls[i] = null;
                }
            }
            else
            {
                todosEliminados = false;
            }
        }

        if (todosEliminados)
        {
            door.OpenDoor();
            _opened = true;
            CancelInvoke(nameof(CheckEnemies));
            enabled = false;
        }
    }

    // Por si querés setear enemigos por código
    public void SetEnemies(List<GameObject> newEnemies)
    {
        enemies = newEnemies;
        _opened = false;
        CheckEnemies();
    }
}