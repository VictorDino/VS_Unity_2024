using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public LayerMask transformableLayer; // Capa de objetos transformables
    public float transformationRadius = 5f; // Radio para buscar objetos transformables
    public KeyCode transformKey = KeyCode.T; // Tecla para transformarse
    public EnemyController enemyController; // Referencia al EnemyController
    public KeyCode interactionKey = KeyCode.F; // Tecla para interactuar con el objeto transformado

    private GameObject transformedObject; // Objeto en el que se transforma el jugador
    private bool isTransformed = false; // Estado de transformación

    private Renderer playerRenderer; // Renderer del jugador para ocultarlo
    private CharacterController characterController; // CharacterController del jugador

    private Vector3 originalPosition; // Posición original del jugador

    void Start()
    {
        // Obtener el Renderer del jugador
        playerRenderer = GetComponentInChildren<Renderer>();

        // Obtener el CharacterController
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(transformKey))
        {
            if (isTransformed)
            {
                RevertTransformation();
            }
            else
            {
                TransformToObject();
            }
        }

        if (isTransformed && Input.GetKeyDown(interactionKey))
        {
            InteractWithTransformedObject();
        }
    }

    void TransformToObject()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, transformationRadius, transformableLayer);

        if (nearbyObjects.Length > 0)
        {
            transformedObject = nearbyObjects[0].gameObject;

            // Guardar la posición original
            originalPosition = transform.position;

            // Ocultar el modelo del jugador
            playerRenderer.enabled = false;

            // Desactivar el CharacterController para que no pueda moverse
            characterController.enabled = false;

            isTransformed = true;
            Debug.Log("Jugador transformado en: " + transformedObject.name);

            // Notificar al enemigo
            enemyController?.OnPlayerTransformed();
        }
        else
        {
            Debug.Log("No se detectaron objetos transformables cerca.");
        }
    }

    void RevertTransformation()
    {
        if (isTransformed)
        {
            // Mostrar el modelo del jugador
            playerRenderer.enabled = true;

            // Reactivar el CharacterController
            characterController.enabled = true;

            // Volver a la posición original
            transform.position = originalPosition;

            isTransformed = false;
            Debug.Log("Jugador ha vuelto a su forma original.");

            // Notificar al enemigo
            enemyController?.OnPlayerReverted();
        }
        else
        {
            Debug.LogWarning("No hay transformación activa para revertir.");
        }
    }

    void InteractWithTransformedObject()
    {
        string interactionType = null;

        if (transformedObject.CompareTag("Light"))
        {
            Light lightComponent = transformedObject.GetComponentInChildren<Light>();
            lightComponent.enabled = !lightComponent.enabled;
            interactionType = "Light";
            Debug.Log("Luz alternada en el objeto transformado.");
        }
        else if (transformedObject.CompareTag("Particles"))
        {
            ParticleSystem particles = transformedObject.GetComponentInChildren<ParticleSystem>();
            if (particles.isPlaying) particles.Stop(); else particles.Play();
            interactionType = "Particles";
            Debug.Log("Partículas alternadas en el objeto transformado.");
        }
        else if (transformedObject.CompareTag("Sound"))
        {
            AudioSource audioSource = transformedObject.GetComponent<AudioSource>();
            if (audioSource.isPlaying) audioSource.Stop(); else audioSource.Play();
            interactionType = "Sound";
            Debug.Log("Sonido alternado en el objeto transformado.");
        }

        if (!string.IsNullOrEmpty(interactionType))
        {
            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            foreach (EnemyController enemy in enemies)
            {
                enemy.ReactToInteraction(interactionType, transformedObject.transform, 5f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, transformationRadius);
    }
}

