using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyDealDamage : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform player;
    private PlayerInteractions playerInteractions;

    public float killDistance = 1.5f; // Distancia mínima para "matar" al jugador

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Encontrar al Player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerInteractions = playerObject.GetComponent<PlayerInteractions>();
        }
    }

    void Update()
    {
        if (player == null || playerInteractions == null) return;

        // Verificar si el jugador está transformado
        if (playerInteractions.IsTransformed()) return;

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
