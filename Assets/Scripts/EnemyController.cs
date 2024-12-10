using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public float detectionRadius = 10f;
    public float stopChaseDistance = 15f;
    public Animator animator;

    private NavMeshAgent agent;
    private Transform currentPatrolTarget;
    private bool isChasing = false;
    private bool isPlayerTransformed = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null || player == null || patrolPoint1 == null || patrolPoint2 == null)
        {
            Debug.LogError("Faltan referencias en EnemyController. Verifica el Inspector.");
            return;
        }

        currentPatrolTarget = patrolPoint1;
        agent.SetDestination(currentPatrolTarget.position);
        animator.SetFloat("Speed", 0);
    }

    void Update()
    {
        if (isPlayerTransformed)
        {
            Patrol();
            return;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            if (PlayerInRadius())
            {
                StartChasingPlayer();
            }
            else
            {
                Patrol();
            }
        }
    }

    void Patrol()
    {
        if (Vector3.Distance(transform.position, currentPatrolTarget.position) <= agent.stoppingDistance + 0.5f)
        {
            Debug.Log($"Patrullaje: alcanzado {currentPatrolTarget.name}. Cambiando de punto.");
            currentPatrolTarget = currentPatrolTarget == patrolPoint1 ? patrolPoint2 : patrolPoint1;
            agent.SetDestination(currentPatrolTarget.position);
        }

        animator.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? 1 : 0);

        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            animator.SetFloat("Speed", 1); // En movimiento
        }
        else
        {
            animator.SetFloat("Speed", 0); // En Idle
        }
    }

    void StartChasingPlayer()
    {
        isChasing = true;
        Debug.Log("El enemigo comienza a perseguir al jugador.");
        animator.SetFloat("Speed", 1);
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
        Debug.Log("El enemigo deja de perseguir al jugador y vuelve a patrullar.");
        agent.SetDestination(currentPatrolTarget.position);
    }

    bool PlayerInRadius()
    {
        return Vector3.Distance(transform.position, player.position) <= detectionRadius;
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