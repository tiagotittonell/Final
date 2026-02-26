using System.Collections.Generic;
using UnityEngine;

public class EnemyClearDoorController : MonoBehaviour
{
    [Header("Enemies (asignar por Inspector)")]
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

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

        // Si alguno sigue vivo/activo, no abre
        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject e = enemies[i];

            // Destruido => eliminado
            if (e == null) continue;

            // Si está activo en jerarquía => sigue vivo
            if (e.activeInHierarchy)
                return;
        }

        // Todos eliminados/inactivos => abrir
        door.OpenDoor();
        _opened = true;

        CancelInvoke(nameof(CheckEnemies));
        enabled = false;
    }

    // Por si querés setear enemigos por código
    public void SetEnemies(List<GameObject> newEnemies)
    {
        enemies = newEnemies;
        _opened = false;
        CheckEnemies();
    }
}