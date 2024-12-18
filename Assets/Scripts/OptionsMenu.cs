using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    public Toggle fullscreenToggle; // Referencia al Toggle de Pantalla Completa
    public Dropdown resolutionDropdown; // Referencia al Dropdown de Resolución
    public Slider volumeSlider;     // Referencia al Slider de Volumen (opcional)

    private Resolution[] resolutions; // Array de resoluciones disponibles

    void Start()
    {
        // Configurar el Toggle de Pantalla Completa
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        // Configurar el Dropdown de Resolución
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // Crear una lista de opciones de resoluciones
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Verificar la resolución actual
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Configurar el Slider de Volumen (opcional)
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume", 1f); // Cargar volumen
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume); // Aplicar volumen
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen; // Cambiar a Pantalla Completa o Ventana
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex); // Guardar la resolución seleccionada
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // Cambiar el volumen global
        PlayerPrefs.SetFloat("Volume", volume); // Guardar volumen
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
