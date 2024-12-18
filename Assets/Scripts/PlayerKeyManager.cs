using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con la UI

public class PlayerKeyManager : MonoBehaviour
{
    public int keysCollected = 0; // Número de llaves recogidas por el jugador
    public Text keyCountText; // Referencia al Text UI donde se mostrarán las llaves

    void Start()
    {
        UpdateKeyHUD(); // Actualizar el HUD al inicio
    }

    public void AddKey()
    {
        keysCollected++;
        UpdateKeyHUD(); // Actualizar el HUD al recoger una llave
    }

    private void UpdateKeyHUD()
    {
        if (keyCountText != null)
        {
            keyCountText.text = keysCollected.ToString(); // Actualizar el texto del HUD
        }
    }
}
