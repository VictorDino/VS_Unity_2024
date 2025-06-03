using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    public AudioClip keyPickupSound;
    private void OnTriggerEnter(Collider other)
    {
        PlayerKeyManager keyManager = other.GetComponent<PlayerKeyManager>();
        if (keyManager != null)
        {
            keyManager.AddKey();
            AudioSource audioSource = other.GetComponent<AudioSource>();
            audioSource.PlayOneShot(keyPickupSound);
            Destroy(gameObject); // Destruye la llave tras recogerla
        }
    }
}
