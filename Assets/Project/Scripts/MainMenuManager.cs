using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Scene lastScene;
    public void StartGame()
    {
        SceneManager.LoadScene(lastScene.buildIndex);
    }
    public void SwitchSettingsMenu(bool state)
    {
        settingsMenu.SetActive(state);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
