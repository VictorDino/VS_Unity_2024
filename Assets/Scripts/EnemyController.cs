using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Sound, Particles, Light, Basic }
    public EnemyType enemyType;

    public Transform player;
    public Transform[] patrolPoints; // Puntos de patrullaje
    public float detectionRadius = 10f; // Radio para detectar al jugador
    public float interactionRadius = 15f; // Radio para reaccionar a interacciones
    public float stopChaseDistance = 15f; // Distancia para dejar de perseguir al jugador
    public Animator animator;

    public GameObject exclamationHUD; // HUD de persecución
    public GameObject questionHUD; // HUD de vigilancia

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private bool isPlayerTransformed = false;
    private bool isVigilating = false;

    public AudioSource audioSource;
    public AudioClip vigilateClip;
    public AudioClip chaseClip;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de patrullaje asignados en " + gameObject.name);
            return;
        }

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        animator.SetFloat("Speed", 0);

        HideAllHUD();
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
            if (PlayerInDetectionRadius())
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
        if (patrolPoints.Length == 0) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        animator.SetFloat("Speed", agent.remainingDistance > agent.stoppingDistance ? 1 : 0);
    }

    void StartChasingPlayer()
    {
        isChasing = true;
        ShowExclamationHUD();
        audioSource.clip = chaseClip;
        audioSource.Play();
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
        HideAllHUD();
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    bool PlayerInDetectionRadius()
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
        if (enemyType == EnemyType.Basic) return;

        float distanceToObject = Vector3.Distance(transform.position, objectToVigilate.position);

        if (distanceToObject > interactionRadius) return; // No reaccionar si está fuera del rango de interacción

        if ((enemyType == EnemyType.Sound && interactionType == "Sound") ||
            (enemyType == EnemyType.Particles && interactionType == "Particles") ||
            (enemyType == EnemyType.Light && interactionType == "Light"))
        {
            Debug.Log($"Enemigo {enemyType} reaccionando a {interactionType}");
            VigilateObject(objectToVigilate, duration);
        }
    }

    public void VigilateObject(Transform objectToVigilate, float duration)
    {
        if (enemyType == EnemyType.Basic) return;

        StartCoroutine(VigilateRoutine(objectToVigilate, duration));
    }

    private IEnumerator VigilateRoutine(Transform objectToVigilate, float duration)
    {
        isVigilating = true;
        ShowQuestionHUD();
        audioSource.clip = vigilateClip;
        audioSource.PlayOneShot(vigilateClip);

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

        isVigilating = false;
        agent.isStopped = false;
        HideAllHUD();

        if (!isChasing && patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
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
}
