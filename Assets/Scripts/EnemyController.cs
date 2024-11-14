using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float detectionRadius = 10f; // Radio de detección para el jugador
    public float stopChaseDistance = 15f; // Distancia a la que el enemigo deja de perseguir
    public LayerMask playerLayer; // Para detectar al jugador
    public Transform[] patrolPoints; // Puntos de patrullaje
    public float patrolWaitTime = 2f; // Tiempo de espera en cada punto de patrullaje
    public float patrolPointReachDistance = 1f; // Distancia para considerar que llegó al punto de patrullaje

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private bool isPlayerTransformed = false; // Saber si el jugador está transformado
    private float patrolWaitTimer;
    private bool waitingAtPoint = false; // Saber si está esperando en un punto de patrullaje

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolWaitTimer = patrolWaitTime;

        // Verificar si hay puntos de patrullaje asignados
        if (patrolPoints.Length > 0)
        {
            Debug.Log("Iniciando patrullaje. Moviéndose al primer punto.");
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else
        {
            Debug.LogWarning("No se asignaron puntos de patrullaje.");
        }
    }

    void Update()
    {
        if (!isChasing)
        {
            Patrol();
        }
        else
        {
            if (isPlayerTransformed)
            {
                StopChasingPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }

        if (!isPlayerTransformed && !isChasing && PlayerInSight())
        {
            StartChasingPlayer();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return; // Si no hay puntos, no hacer nada

        // Calcular la distancia manualmente entre el enemigo y el punto de patrullaje
        float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position);

        // Verificar si ha llegado al punto de patrullaje
        if (!waitingAtPoint && distanceToPatrolPoint <= patrolPointReachDistance)
        {
            // Iniciar la espera en el punto de patrullaje
            waitingAtPoint = true;
            patrolWaitTimer = patrolWaitTime;

            Debug.Log("En el punto de patrullaje: " + currentPatrolIndex + ". Esperando " + patrolWaitTime + " segundos.");
        }

        // Mientras espera, reducir el temporizador
        if (waitingAtPoint)
        {
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0f)
            {
                waitingAtPoint = false;

                // Mover al siguiente punto de patrullaje
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);

                Debug.Log("Moviéndose al siguiente punto de patrullaje: " + currentPatrolIndex);
            }
        }
    }

    void StartChasingPlayer()
    {
        isChasing = true;
        Debug.Log("Comenzando a perseguir al jugador.");
        agent.SetDestination(player.position);
    }

    void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= stopChaseDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            StopChasingPlayer();
        }
    }

    void StopChasingPlayer()
    {
        isChasing = false;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        Debug.Log("Dejando de perseguir al jugador. Volviendo a patrullar.");
    }

    bool PlayerInSight()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.transform == player)
            {
                return true;
            }
        }
        return false;
    }

    public void OnPlayerTransformed()
    {
        isPlayerTransformed = true;
        StopChasingPlayer();
    }

    public void OnPlayerReverted()
    {
        isPlayerTransformed = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
