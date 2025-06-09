using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    public AudioClip keyPickupSound;
    public float rotationSpeed = 50f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerKeyManager keyManager = other.GetComponent<PlayerKeyManager>();
        keyManager.AddKey();
        AudioSource audioSource = other.GetComponent<AudioSource>();
        audioSource.PlayOneShot(keyPickupSound);
        Destroy(gameObject); // Destruye la llave tras recogerla
        
    }
}
