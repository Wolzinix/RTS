using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNextUI : MonoBehaviour
{
    [SerializeField] int SceneToload;

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
        SceneManager.LoadScene(SceneIndex.GetIndexOfScene(SceneToload));
    }
    public void MainMenuScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneIndex.GetIndexOfScene(0));
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
        else if (SceneToload == 0)
        {
            MainMenuUI.SetActive(true);
        }
        else
        {
            NextMenuUI.SetActive(true);
        }
        
    }

}
