using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int keysRequired = 3; // Número de llaves necesarias para abrir la puerta
    private bool isOpen = false; // Estado de la puerta
    public Animator doorAnimator; // Referencia al Animator de la puerta

    private void OnTriggerEnter(Collider other)
    {
        PlayerKeyManager keyManager = other.GetComponent<PlayerKeyManager>();

        if (keyManager != null && !isOpen)
        {
            if (keyManager.keysCollected >= keysRequired)
            {
                OpenDoor();
            }
            else
            {
                Debug.Log("Faltan llaves. Necesitas: " + (keysRequired - keyManager.keysCollected) + " más.");
            }
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log("¡Puerta abierta!");
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open"); // Activa la animación de apertura
        }
    }
}
