using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteract : MonoBehaviour
{
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectRenderer.material.EnableKeyword("_EMISSION"); // Activar emisión
            objectRenderer.material.SetColor("_EmissionColor", Color.yellow * 2); // Color de resalte
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectRenderer.material.DisableKeyword("_EMISSION"); // Desactivar emisión
        }
    }
}
