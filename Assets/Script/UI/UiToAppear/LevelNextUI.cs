using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNextUI : UiAppeirBase
{
    [SerializeField] int SceneToload;

    [SerializeField] GameObject MainMenuUI;
    [SerializeField] GameObject RetryMenuUI;
    [SerializeField] GameObject NextMenuUI;

    SaveForNextlevel save;

    private void Start()
    {
        save = FindObjectOfType<SaveForNextlevel>();
    }
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

    public override void AppearUI(bool IsFailed)
    {
        Time.timeScale = 0;
        if (IsFailed) { RetryMenuUI.SetActive(true);  }
        else if (SceneToload == 0) { MainMenuUI.SetActive(true);  }
        else { NextMenuUI.SetActive(true); }
    }

    public override void AppearUI()
    {
        Time.timeScale = 0;
        if (SceneToload == 0) { MainMenuUI.SetActive(true); }
        else { NextMenuUI.SetActive(true); }
    }

    public override void DisappearUI()
    {
        Time.timeScale = 1;
        RetryMenuUI.SetActive(false);
        MainMenuUI.SetActive(false);  
        NextMenuUI.SetActive(false);
    }
}
