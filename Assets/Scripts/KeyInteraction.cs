using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerKeyManager playerKeyManager = other.GetComponent<PlayerKeyManager>();
            if (playerKeyManager != null)
            {
                playerKeyManager.CollectKey(); // Notificar al jugador que ha recogido una llave
                Destroy(gameObject); // Destruir la llave
            }
        }
    }
}
