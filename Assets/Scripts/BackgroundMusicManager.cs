using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    private static BackgroundMusicManager instance;

    void Awake()
    {
        // Verificar si ya existe una instancia
        if (instance != null)
        {
            Destroy(gameObject); // Evitar duplicados si ya existe
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Mantener este objeto al cambiar de escena
    }
}
