using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformablesManager : MonoBehaviour
{
    // Referencia al objeto original (el personaje)
    public GameObject originalObject;
    public EnemyController enemyController;

    // LayerMask para los objetos transformables
    public LayerMask transformableMask;

    // Radio dentro del cual se pueden transformar los objetos
    public float transformationRadius = 5.0f;

    // Teclas para interactuar
    public KeyCode transformKey = KeyCode.T;
    public KeyCode interactionKey = KeyCode.F;  // Tecla para interactuar (encender luz, reproducir sonido o activar/desactivar partículas)

    // Variable para almacenar el objeto transformado
    private GameObject transformedObject;

    // Indica si el personaje está transformado
    private bool isTransformed = false;

    // Material de resaltado
    public Material highlightMaterial;
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();

    // Guardar una referencia a los objetos cercanos resaltados
    private List<Collider> highlightedObjects = new List<Collider>();

    // Componentes específicos del objeto transformado
    private Light lampLight;          // Referencia a la luz de la lámpara (si es el objeto transformado)
    private AudioSource soundSource;   // Referencia al sonido (si el objeto transformado es el objeto con sonido)
    private ParticleSystem particles;  // Referencia a las partículas (si el objeto transformado es el objeto con partículas)

    void Start()
    {
        // Apagar la luz de la lámpara en el inicio si está en la escena
        GameObject lamp = GameObject.Find("Lamp");  // Cambia "Lamp" por el nombre de tu lámpara en la escena
        if (lamp != null)
        {
            Light lightComponent = lamp.GetComponentInChildren<Light>();
            if (lightComponent != null)
            {
                lightComponent.enabled = false;  // Asegurar que la luz esté apagada desde el principio
            }
        }
    }

    void Update()
    {
        // Resaltar los objetos dentro del radio
        HighlightNearbyObjects();

        // Detectar si se presiona la tecla para transformar
        if (Input.GetKeyDown(transformKey))
        {
            if (!isTransformed)
            {
                TransformToObject();
            }
            else
            {
                ReturnToOriginalForm();
            }
        }

        // Interacción con el objeto transformado (luz, sonido o partículas)
        if (isTransformed && Input.GetKeyDown(interactionKey))
        {
            if (lampLight != null)
            {
                ToggleLampLight();
            }
            else if (soundSource != null)
            {
                PlaySound();
            }
            else if (particles != null)
            {
                ToggleParticles();
            }
        }
    }

    // Método para resaltar los objetos cercanos
    private void HighlightNearbyObjects()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(originalObject.transform.position, transformationRadius, transformableMask);

        // Restablecer el material de los objetos que ya no están dentro del radio
        ResetObjectMaterials(nearbyObjects);

        // Aplicar resaltado a los nuevos objetos dentro del radio
        foreach (Collider obj in nearbyObjects)
        {
            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer != null && !originalMaterials.ContainsKey(objRenderer))
            {
                originalMaterials[objRenderer] = objRenderer.material;
                objRenderer.material = highlightMaterial;
            }

            if (!highlightedObjects.Contains(obj))
            {
                highlightedObjects.Add(obj);
            }
        }
    }

    // Método para volver al material original después de deseleccionar
    private void ResetObjectMaterials(Collider[] nearbyObjects)
    {
        HashSet<Collider> nearbySet = new HashSet<Collider>(nearbyObjects);

        for (int i = highlightedObjects.Count - 1; i >= 0; i--)
        {
            Collider obj = highlightedObjects[i];
            if (!nearbySet.Contains(obj))
            {
                Renderer objRenderer = obj.GetComponent<Renderer>();
                if (objRenderer != null && originalMaterials.ContainsKey(objRenderer))
                {
                    objRenderer.material = originalMaterials[objRenderer];
                    originalMaterials.Remove(objRenderer);
                }

                highlightedObjects.RemoveAt(i);
            }
        }
    }

    // Método para transformarse en un objeto cercano
    private void TransformToObject()
    {
        if (highlightedObjects.Count > 0)
        {
            transformedObject = highlightedObjects[0].gameObject;
            Vector3 transformedObjectPosition = transformedObject.transform.position;

            originalObject.SetActive(false);
            originalObject.transform.position = transformedObjectPosition;

            transformedObject.SetActive(true);

            // Detectar componentes específicos del objeto transformado
            lampLight = transformedObject.GetComponentInChildren<Light>();
            soundSource = transformedObject.GetComponent<AudioSource>();
            particles = transformedObject.GetComponentInChildren<ParticleSystem>();

            // Apagar la luz inicialmente si es una lámpara
            if (lampLight != null)
            {
                lampLight.enabled = false;
            }

            isTransformed = true;
            enemyController.OnPlayerTransformed();
            ResetObjectMaterials(new Collider[0]);
        }
    }

    // Método para volver al objeto original
    private void ReturnToOriginalForm()
    {
        if (transformedObject != null)
        {
            originalObject.SetActive(true);
            Vector3 transformedPosition = transformedObject.transform.position;
            originalObject.transform.position = transformedPosition;

            isTransformed = false;

            // Limpiar referencias específicas
            transformedObject = null;
            lampLight = null;
            soundSource = null;
            particles = null;

            enemyController.OnPlayerReverted();
        }
    }

    // Método para encender o apagar la luz de la lámpara
    private void ToggleLampLight()
    {
        if (lampLight != null)
        {
            lampLight.enabled = !lampLight.enabled;
        }
    }

    // Método para reproducir el sonido del objeto transformado
    private void PlaySound()
    {
        if (soundSource != null && !soundSource.isPlaying)
        {
            soundSource.Play();
        }
    }

    // Método para activar o desactivar las partículas del objeto transformado
    private void ToggleParticles()
    {
        if (particles != null)
        {
            if (particles.isPlaying)
            {
                particles.Stop();
            }
            else
            {
                particles.Play();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(originalObject.transform.position, transformationRadius);
    }
}
