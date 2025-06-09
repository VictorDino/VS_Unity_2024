using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationPlatform : MonoBehaviour
{
    public GameObject objectToDestroy; // El objeto que será destruido, por ejemplo, una pared
    public AudioClip activationSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Verifica si el objeto que entra es un enemigo
        {
            Transform childToDestroy = transform.Find("ChildPlatform");
            if (objectToDestroy != null)
            {
                AudioSource audioSource = other.GetComponent<AudioSource>();
                audioSource.PlayOneShot(activationSound);
                Destroy(objectToDestroy); // Destruye el objeto
                Destroy(childToDestroy.gameObject);
                Debug.Log("¡El objeto ha sido destruido por el enemigo!");
            }
        }
    }
}
