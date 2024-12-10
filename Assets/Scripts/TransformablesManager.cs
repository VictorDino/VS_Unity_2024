using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformablesManager : MonoBehaviour
{
    public GameObject originalObject; 
    public EnemyController enemyController;

    public LayerMask transformableMask;
    public float transformationRadius = 5.0f;
    public KeyCode transformKey = KeyCode.T;
    public KeyCode interactionKey = KeyCode.F;

    private GameObject transformedObject;
    private bool isTransformed = false;

    public Material highlightMaterial;
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private List<Collider> highlightedObjects = new List<Collider>();

    private Light lampLight;
    private AudioSource soundSource;
    private ParticleSystem particles;

    void Update()
    {
        HighlightNearbyObjects();

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

        if (isTransformed && Input.GetKeyDown(interactionKey))
        {
            if (lampLight != null) ToggleLampLight();
            else if (soundSource != null) PlaySound();
            else if (particles != null) ToggleParticles();
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

            if (!highlightedObjects.Contains(obj)) highlightedObjects.Add(obj);
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

    private void TransformToObject()
    {
        if (highlightedObjects.Count > 0)
        {
            transformedObject = highlightedObjects[0].gameObject;

            originalObject.SetActive(false);
            transformedObject.SetActive(true);

            lampLight = transformedObject.GetComponentInChildren<Light>();
            soundSource = transformedObject.GetComponent<AudioSource>();
            particles = transformedObject.GetComponentInChildren<ParticleSystem>();

            if (lampLight != null) lampLight.enabled = false;

            isTransformed = true;
            enemyController.OnPlayerTransformed();
        }
    }

    private void ReturnToOriginalForm()
    {
        if (transformedObject != null)
        {
            originalObject.SetActive(true);

            lampLight = null;
            soundSource = null;
            particles = null;

            transformedObject = null;
            isTransformed = false;

            enemyController.OnPlayerReverted();
        }
    }

    private void ToggleLampLight()
    {
        if (lampLight != null) lampLight.enabled = !lampLight.enabled;
    }

    private void PlaySound()
    {
        if (soundSource != null && !soundSource.isPlaying) soundSource.Play();
    }

    private void ToggleParticles()
    {
        if (particles != null)
        {
            if (particles.isPlaying) particles.Stop();
            else particles.Play();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(originalObject.transform.position, transformationRadius);
    }
}
