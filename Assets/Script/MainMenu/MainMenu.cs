using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] int SceneToload;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneIndex.GetIndexOfScene(SceneToload));
    }
}
