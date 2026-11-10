using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControladorEnemigos : MonoBehaviour
{
    private float minX, maxX, minY, maxY;

    [SerializeField] private Transform[] puntos;
    [SerializeField] private GameObject[] enemigos;
    [SerializeField] private float tiempoEnemigos;
    private float tiempoSiguienteEnemigo;
    //    // Start is called before the first frame update
    //    void Start()
    //    {
    //        maxX = puntos.Max(punto => punto.position.x);
    //        minX = puntos.Min(punto => punto.position.x);
    //        maxY = puntos.Max(punto => punto.position.y);
    //        minY = puntos.Min(punto => punto.position.y);
    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        tiempoSiguienteEnemigo += Time.deltaTime;

    //        if(tiempoSiguienteEnemigo >= tiempoEnemigos)
    //        {
    //            tiempoSiguienteEnemigo = 0;
    //            CrearEnemigo();
    //        }
    //    }
    //    private void CrearEnemigo()
    //    {
    //        int numeroEnemigo= Random.Range(0, enemigos.Length);
    //        Vector2 posicionAleatoria = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

    //        Instantiate(enemigos[numeroEnemigo], posicionAleatoria, Quaternion.identity);
    //    }
    private int enemigosVivos = 0;

    // En el método Start, inicializa enemigosVivos a 0.

    void Start()
    {
        maxX = puntos.Max(punto => punto.position.x);
        minX = puntos.Min(punto => punto.position.x);
        maxY = puntos.Max(punto => punto.position.y);
        minY = puntos.Min(punto => punto.position.y);
        enemigosVivos = 0;
    }

    // En el método Update, controla la creación de enemigos.

    void Update()
    {
        tiempoSiguienteEnemigo += Time.deltaTime;

        if (tiempoSiguienteEnemigo >= tiempoEnemigos && enemigosVivos < 3)
        {
            tiempoSiguienteEnemigo = 0;
            CrearEnemigo();
        }
    }

    private void CrearEnemigo()
    {
        if (enemigosVivos < 3)
        {
            int numeroEnemigo = Random.Range(0, enemigos.Length);
            Vector2 posicionAleatoria = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            Instantiate(enemigos[numeroEnemigo], posicionAleatoria, Quaternion.identity);
            enemigosVivos++;
        }
    }

    // Para llevar un registro de cuándo un enemigo muere, podrías tener una función adicional que disminuye enemigosVivos cuando sea necesario.

    public void EnemigoMuere()
    {
        enemigosVivos--;
    }

}
