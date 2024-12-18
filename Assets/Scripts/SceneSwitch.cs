using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            LoadScene("Tutorial");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            LoadScene("Level1");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            LoadScene("Level2");
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            LoadScene("Level3");
        }
    }

    void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
