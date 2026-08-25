using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    
    public void StartGameScene()
    {
        // Iniciar con la escena del carro (trayecto hacia la mansión),
        // que luego transiciona sola a PrincipalScene.
        SceneManager.LoadScene(2);
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}