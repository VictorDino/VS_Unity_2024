using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerKeyManager keyManager = other.GetComponent<PlayerKeyManager>();
        if (keyManager != null)
        {
            keyManager.AddKey();
            Destroy(gameObject); // Destruye la llave tras recogerla
        }
    }
}
