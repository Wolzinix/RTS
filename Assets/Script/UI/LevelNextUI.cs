using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNextUI : MonoBehaviour
{
    [SerializeField] string MainMenu;
    [SerializeField] string SceneToload;

    [SerializeField] GameObject MainMenuUI;
    [SerializeField] GameObject RetryMenuUI;
    [SerializeField] GameObject NextMenuUI;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneToload);
    }
    public void MainMenuScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(MainMenu);
    }
    public void CurrentScene()
    {
        Time.timeScale = 1;
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
        
    }

    public void AppearUI(bool IsPlayer)
    {
        Time.timeScale = 0;
        if (IsPlayer)
        {
            RetryMenuUI.SetActive(true);
        }
        else if (SceneToload == "")
        {
            MainMenuUI.SetActive(true);
        }
        else
        {
            NextMenuUI.SetActive(true);
        }
        
    }

}
