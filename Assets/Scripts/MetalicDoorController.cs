using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalicDoorController : MonoBehaviour
{
    public GameObject[] doors; // Puertas a destruir
    public float countdownTime = 5f; // Tiempo de cuenta atr�s en segundos
    public string playerTag = "Player"; // Tag del jugador

    private bool countdownStarted = false; // Control para evitar m�ltiples activaciones

    void OnTriggerEnter(Collider other)
    {
        if (!countdownStarted && other.CompareTag(playerTag))
        {
            countdownStarted = true;
            Debug.Log("Cuenta atr�s iniciada...");
            StartCoroutine(DestroyDoorsAfterCountdown());
        }
    }

    private IEnumerator DestroyDoorsAfterCountdown()
    {
        yield return new WaitForSeconds(countdownTime);

        foreach (GameObject door in doors)
        {
            if (door != null) // Asegurar que la puerta no est� ya destruida
            {
                UnityEngine.AI.NavMeshObstacle obstacle = door.GetComponent<UnityEngine.AI.NavMeshObstacle>();
                if (obstacle != null) Destroy(obstacle); // Eliminar el obst�culo
                Destroy(door); // Destruir la puerta
                Debug.Log($"Puerta {door.name} destruida.");
            }
        }
    }
}
