using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartGameScene()
    {
        // Iniciar con la escena del carro (trayecto hacia la mansión),
        // que luego transiciona sola a PrincipalScene.
        SceneManager.LoadScene("Carro y salida");
    }
}