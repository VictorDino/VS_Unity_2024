using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Sound, Particles, Light } // Tipos de enemigo
    public EnemyType enemyType; // Tipo del enemigo actual

    public Transform player;
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public float detectionRadius = 10f;
    public float stopChaseDistance = 15f;
    public Animator animator;

    public GameObject exclamationHUD; // Imagen de exclamación
    public GameObject questionHUD; // Imagen de interrogación

    private NavMeshAgent agent;
    private Transform currentPatrolTarget;
    private bool isChasing = false;
    private bool isPlayerTransformed = false;
    private bool isVigilating = false;

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

        // Asegurarse de que los HUD estén desactivados al inicio
        if (exclamationHUD) exclamationHUD.SetActive(false);
        if (questionHUD) questionHUD.SetActive(false);
    }

    void Update()
    {
        if (isVigilating)
        {
            ShowQuestionHUD();
            return;
        }

        if (isPlayerTransformed)
        {
            Patrol();
            return;
        }

        if (isChasing)
        {
            ShowExclamationHUD();
            ChasePlayer();
        }
        else
        {
            HideAllHUD();
            if (PlayerInRadius())
            {
                StartChasingPlayer();
            }
            else
            {
                Patrol();
            }
        }

        if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
        {
            DealDamageToPlayer();
        }
    }

    void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Cambiar al siguiente punto de patrullaje
            currentPatrolTarget = currentPatrolTarget == patrolPoint1 ? patrolPoint2 : patrolPoint1;
            agent.SetDestination(currentPatrolTarget.position);
        }

        // Controlar la animación
        animator.SetFloat("Speed", agent.remainingDistance > agent.stoppingDistance ? 1 : 0);
    }

    void StartChasingPlayer()
    {
        isChasing = true;
        ShowExclamationHUD();
        animator.SetFloat("Speed", 1);
        Debug.Log("El enemigo comienza a perseguir al jugador.");
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
        HideAllHUD();
        agent.SetDestination(currentPatrolTarget.position);
        Debug.Log("El enemigo deja de perseguir al jugador y vuelve a patrullar.");
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

    public void ReactToInteraction(string interactionType, Transform objectToVigilate, float duration)
    {
        if ((enemyType == EnemyType.Sound && interactionType == "Sound") ||
            (enemyType == EnemyType.Particles && interactionType == "Particles") ||
            (enemyType == EnemyType.Light && interactionType == "Light"))
        {
            VigilateObject(objectToVigilate, duration);
        }
    }

    public void VigilateObject(Transform objectToVigilate, float duration)
    {
        StartCoroutine(VigilateRoutine(objectToVigilate, duration));
    }

    private IEnumerator VigilateRoutine(Transform objectToVigilate, float duration)
    {
        isVigilating = true;
        ShowQuestionHUD();
        Debug.Log("Enemigo vigilando el objeto: " + objectToVigilate.name);

        agent.SetDestination(objectToVigilate.position);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                animator.SetFloat("Speed", 0); // Animación Idle
            }
            else
            {
                agent.isStopped = false;
                animator.SetFloat("Speed", 1); // Animación Walk
            }

            yield return null;
        }

        Debug.Log("Enemigo deja de vigilar el objeto y vuelve a patrullar.");

        isVigilating = false;
        agent.isStopped = false;
        HideAllHUD();

        if (!isChasing)
        {
            currentPatrolTarget = Vector3.Distance(transform.position, patrolPoint1.position) <
                                  Vector3.Distance(transform.position, patrolPoint2.position)
                ? patrolPoint1
                : patrolPoint2;

            agent.SetDestination(currentPatrolTarget.position);
        }
    }

    void ShowExclamationHUD()
    {
        if (exclamationHUD) exclamationHUD.SetActive(true);
        if (questionHUD) questionHUD.SetActive(false);
    }

    void ShowQuestionHUD()
    {
        if (exclamationHUD) exclamationHUD.SetActive(false);
        if (questionHUD) questionHUD.SetActive(true);
    }

    void HideAllHUD()
    {
        if (exclamationHUD) exclamationHUD.SetActive(false);
        if (questionHUD) questionHUD.SetActive(false);
    }

    void OnEnable()
    {
        PlayerInteractions player = FindObjectOfType<PlayerInteractions>();
        if (player != null)
        {
            player.RegisterEnemy(this);
        }
    }

    void OnDisable()
    {
        PlayerInteractions player = FindObjectOfType<PlayerInteractions>();
        if (player != null)
        {
            player.UnregisterEnemy(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
        }
    }

    private void DealDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null ) // Verifica invulnerabilidad
        {
            playerHealth.TakeDamage();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
