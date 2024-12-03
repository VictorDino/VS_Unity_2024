using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformablesManager : MonoBehaviour
{
    // Referencia al objeto original (el personaje)
    public GameObject originalObject;
    public Camera mainCamera; // Referencia a la cámara principal
    public EnemyController enemyController;

    // LayerMask para los objetos transformables
    public LayerMask transformableMask;

    // Radio dentro del cual se pueden transformar los objetos
    public float transformationRadius = 5.0f;

    // Teclas para interactuar
    public KeyCode transformKey = KeyCode.T;
    public KeyCode interactionKey = KeyCode.F;

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
    private Light lampLight;
    private AudioSource soundSource;
    private ParticleSystem particles;

    void Start()
    {
        // Apagar la luz de la lámpara en el inicio si está en la escena
        GameObject lamp = GameObject.Find("Lamp");
        if (lamp != null)
        {
            Light lightComponent = lamp.GetComponentInChildren<Light>();
            if (lightComponent != null)
            {
                lightComponent.enabled = false;
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

    private void TransformToObject()
    {
        if (highlightedObjects.Count > 0)
        {
            // Seleccionar el primer objeto resaltado para transformarse
            transformedObject = highlightedObjects[0].gameObject;

            // Guardar la posición del objeto transformado
            Vector3 transformedObjectPosition = transformedObject.transform.position;

            // Ocultar el objeto original
            originalObject.SetActive(false);

            // Mover la cámara al objeto transformado
            mainCamera.transform.SetParent(transformedObject.transform);
            mainCamera.transform.localPosition = new Vector3(0, 2, -3); // Ajusta según sea necesario
            mainCamera.transform.localRotation = Quaternion.identity;

            // Activar el objeto transformado
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

    private void ReturnToOriginalForm()
    {
        if (transformedObject != null)
        {
            // Reactivar el objeto original
            originalObject.SetActive(true);

            // Mover la cámara de vuelta al objeto original
            mainCamera.transform.SetParent(originalObject.transform);
            mainCamera.transform.localPosition = new Vector3(0, 2, -3); // Ajusta según sea necesario
            mainCamera.transform.localRotation = Quaternion.identity;

            // Desactivar el objeto transformado
            transformedObject.SetActive(false);
            transformedObject = null;

            // Limpiar referencias específicas
            lampLight = null;
            soundSource = null;
            particles = null;

            isTransformed = false;
            enemyController.OnPlayerReverted();
        }
    }

    private void HighlightNearbyObjects()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(originalObject.transform.position, transformationRadius, transformableMask);

        ResetObjectMaterials(nearbyObjects);

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

    private void ToggleLampLight()
    {
        if (lampLight != null)
        {
            lampLight.enabled = !lampLight.enabled;
        }
    }

    private void PlaySound()
    {
        if (soundSource != null && !soundSource.isPlaying)
        {
            soundSource.Play();
        }
    }

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
