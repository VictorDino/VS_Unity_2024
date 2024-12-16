using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3; // Número máximo de vidas
    public Image[] lifeIcons; // Iconos de los corazones en el HUD
    public float damageCooldown = 10f; // Tiempo de espera entre daños consecutivos
    public Image bloodEffect; // Imagen del efecto de sangre en el HUD
    public float bloodEffectDuration = 0.5f; // Duración del efecto de sangre

    private int currentLives; // Vidas actuales del jugador
    public bool isInvulnerable = false; // Si el jugador es invulnerable

    void Start()
    {
        currentLives = maxLives;
        UpdateLifeIcons();

        if (bloodEffect != null)
        {
            bloodEffect.gameObject.SetActive(false); // Asegurarse de que el efecto esté desactivado al inicio
        }
    }

    public void TakeDamage()
    {
        if (isInvulnerable) return;

        currentLives--;

        UpdateLifeIcons();

        if (bloodEffect != null)
        {
            StartCoroutine(ShowBloodEffect());
        }

        if (currentLives <= 0)
        {
            Debug.Log("¡El jugador ha muerto!");
            // Aquí puedes añadir lógica para reiniciar el nivel o terminar el juego
        }
        else
        {
            StartCoroutine(DamageCooldownRoutine());
        }
    }

    private IEnumerator DamageCooldownRoutine()
    {
        isInvulnerable = true;
        Debug.Log("Jugador invulnerable por " + damageCooldown + " segundos.");
        yield return new WaitForSeconds(damageCooldown);
        isInvulnerable = false;
        Debug.Log("Jugador ya no es invulnerable.");
    }

    private void UpdateLifeIcons()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < currentLives)
            {
                lifeIcons[i].enabled = true;
            }
            else
            {
                lifeIcons[i].enabled = false;
            }
        }
    }

    private IEnumerator ShowBloodEffect()
    {
        bloodEffect.gameObject.SetActive(true); // Mostrar el efecto de sangre
        yield return new WaitForSeconds(bloodEffectDuration);
        bloodEffect.gameObject.SetActive(false); // Ocultar el efecto de sangre
    }
}
