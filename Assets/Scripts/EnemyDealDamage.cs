using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyDealDamage : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform player;

    public float killDistance = 1.5f; // Distancia mínima para "matar" al jugador

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Encontrar al Player
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Verificar la distancia real entre el enemigo y el jugador
        if (Vector3.Distance(transform.position, player.position) <= killDistance)
        {
            Debug.Log("El enemigo ha alcanzado al jugador. Reiniciando nivel...");
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
