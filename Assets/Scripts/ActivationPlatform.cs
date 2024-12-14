using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationPlatform : MonoBehaviour
{
    public GameObject objectToDestroy; // El objeto que será destruido, por ejemplo, una pared

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Verifica si el objeto que entra es un enemigo
        {
            if (objectToDestroy != null)
            {
                Destroy(objectToDestroy); // Destruye el objeto
                Debug.Log("¡El objeto ha sido destruido por el enemigo!");
            }
        }
    }
}
