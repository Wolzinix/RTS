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
        SceneManager.LoadScene(SceneToload);
        Time.timeScale = 1;
    }
    public void MainMenuScene()
    {
        SceneManager.LoadScene(MainMenu);
        Time.timeScale = 1;
    }
    public void CurrentScene()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1;
    }

    public void AppearUI(int choice)
    {
        Time.timeScale = 0;
        if (choice != 0)
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
