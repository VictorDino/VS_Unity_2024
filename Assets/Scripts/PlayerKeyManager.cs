using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKeyManager : MonoBehaviour
{
    public int totalKeys = 4; // Total de llaves necesarias para abrir la puerta
    private int collectedKeys = 0; // Llaves recolectadas por el jugador
    public Text keysCounterText; // Texto del HUD para mostrar las llaves recolectadas
    public Animator doorAnimator; // Animator de la puerta

    void Start()
    {
        UpdateKeysCounter();
    }

    public void CollectKey()
    {
        collectedKeys++;
        UpdateKeysCounter();

        if (collectedKeys >= totalKeys)
        {
            OpenExitDoor();
        }
    }

    private void UpdateKeysCounter()
    {
        if (keysCounterText != null)
        {
            keysCounterText.text = $"{collectedKeys} / {totalKeys}";
        }
    }

    private void OpenExitDoor()
    {
        if (doorAnimator != null)
        {
            Debug.Log("¡Todas las llaves recolectadas! Activando animación de la puerta...");
            doorAnimator.SetTrigger("Open"); // Activa el trigger para la animación de apertura
        }
        else
        {
            Debug.LogWarning("No se asignó el Animator de la puerta.");
        }
    }

    public void ResetKeys()
    {
        collectedKeys = 0;
        UpdateKeysCounter();
    }
}
