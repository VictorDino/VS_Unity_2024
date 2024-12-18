using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Panel del menú de pausa
    public Slider volumeSlider;    // Slider para ajustar el volumen
    private bool isPaused = false; // Estado del juego
    private PlayerMovement playerMovement; // Referencia al script de movimiento del jugador

    void Start()
    {
        // Ocultar el menú de pausa al inicio
        pauseMenuUI.SetActive(false);

        // Encontrar el script de movimiento del jugador
        playerMovement = FindObjectOfType<PlayerMovement>();

        // Configurar el slider de volumen
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume; // Inicializar el slider con el volumen actual
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void Update()
    {
        // Detectar la tecla Escape para pausar/reanudar el juego
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // Ajustar el volumen global
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Salir del juego
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Reanudar el tiempo
        isPaused = false;

        // Reactivar los inputs del jugador
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Detener el tiempo
        isPaused = true;

        // Desactivar los inputs del jugador
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }
}
