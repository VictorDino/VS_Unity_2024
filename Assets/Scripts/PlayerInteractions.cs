using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour
{
    public LayerMask transformableLayer; // Capa de objetos transformables
    public float transformationRadius = 5f; // Radio para buscar objetos transformables
    public Material emissionMaterial; // Material con emisión para destacar objetos

    public AudioSource audioSource;           // El AudioSource del jugador
    public AudioClip lightSoundClip;          // Sonido al activar luz
    public AudioClip particleSoundClip;       // Sonido al activar particulas

    private GameObject transformedObject; // Objeto en el que se transforma el jugador
    private bool isTransformed = false; // Estado de transformación

    private Renderer playerRenderer; // Renderer del jugador para ocultarlo
    private CharacterController characterController; // CharacterController del jugador

    private Vector3 originalPosition; // Posición original del jugador

    public GameObject objectMessageText;
    public GameObject indicatorPrefab;
    private GameObject activeIndicator;

    private List<EnemyController> enemies = new List<EnemyController>(); // Lista de enemigos
    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>(); // Guardar materiales originales

    void Start()
    {
        playerRenderer = GetComponentInChildren<Renderer>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HighlightNearbyObjects();

        // Transformar o revertir con el clic izquierdo del ratón
        if (Input.GetMouseButtonDown(0))
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

        // Interactuar con el clic derecho del ratón
        if (isTransformed && Input.GetMouseButtonDown(1))
        {
            InteractWithTransformedObject();
        }
    }

    void HighlightNearbyObjects()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, transformationRadius, transformableLayer);

        // Lista temporal para almacenar los objetos que ya no están en el radio
        List<GameObject> objectsToRemove = new List<GameObject>();

        // Identificar los objetos que deben restaurar su material
        foreach (var obj in originalMaterials.Keys)
        {
            // Excluir el objeto transformado
            if (!System.Array.Exists(nearbyObjects, col => col.gameObject == obj) || obj == transformedObject)
            {
                objectsToRemove.Add(obj);
            }
        }

        // Restaurar materiales fuera del bucle principal
        foreach (var obj in objectsToRemove)
        {
            RestoreOriginalMaterial(obj);
        }

        // Activar material de emisión en los objetos cercanos
        foreach (Collider col in nearbyObjects)
        {
            GameObject obj = col.gameObject;
            if (!originalMaterials.ContainsKey(obj) && obj != transformedObject) // Excluir el objeto transformado
            {
                StoreAndApplyEmissionMaterial(obj);
            }
        }
    }

    void StoreAndApplyEmissionMaterial(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterials[obj] = renderer.material; // Guardar el material original
            renderer.material = emissionMaterial; // Aplicar el material con emisión
        }
    }

    void RestoreOriginalMaterial(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && originalMaterials.ContainsKey(obj))
        {
            renderer.material = originalMaterials[obj]; // Restaurar el material original
        }
        originalMaterials.Remove(obj); // Eliminar de la lista
    }

    void TransformToObject()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, transformationRadius, transformableLayer);

        if (nearbyObjects.Length > 0)
        {
            transformedObject = nearbyObjects[0].gameObject;

            // Restaurar el material original del objeto transformado si estaba resaltado
            if (originalMaterials.ContainsKey(transformedObject))
            {
                RestoreOriginalMaterial(transformedObject);
            }

            originalPosition = transform.position;
            playerRenderer.enabled = false;
            characterController.enabled = false;

            isTransformed = true;
            objectMessageText.SetActive(true);
            
            float heightOffset = 1.3f; // Altura fija
            activeIndicator = Instantiate(indicatorPrefab);
            activeIndicator.transform.SetParent(transformedObject.transform);
            activeIndicator.transform.localPosition = new Vector3(0, heightOffset, 0);
            activeIndicator.transform.localRotation = Quaternion.identity;
            Debug.Log("Jugador transformado en: " + transformedObject.name);

            NotifyEnemies(true);
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
            playerRenderer.enabled = true;
            characterController.enabled = true;
            transform.position = originalPosition;

            isTransformed = false;
            objectMessageText.SetActive(false);
            Destroy(activeIndicator);
            Debug.Log("Jugador ha vuelto a su forma original.");

            NotifyEnemies(false);
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
            audioSource.PlayOneShot(lightSoundClip);
            Debug.Log("Luz alternada en el objeto transformado.");
        }
        else if (transformedObject.CompareTag("Particles"))
        {
            ParticleSystem particles = transformedObject.GetComponentInChildren<ParticleSystem>();
            if (particles.isPlaying) particles.Stop(); else particles.Play();
            interactionType = "Particles";
            audioSource.PlayOneShot(particleSoundClip);
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
            foreach (EnemyController enemy in enemies)
            {
                enemy.ReactToInteraction(interactionType, transformedObject.transform, 5f);
            }
        }
    }

    void NotifyEnemies(bool transformed)
    {
        foreach (EnemyController enemy in enemies)
        {
            if (transformed)
            {
                enemy.OnPlayerTransformed();
            }
            else
            {
                enemy.OnPlayerReverted();
            }
        }
    }

    public bool IsTransformed()
    {
        return isTransformed;
    }

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(EnemyController enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, transformationRadius);
    }
}
