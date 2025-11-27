using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] int SceneToload;

    [SerializeField] InputActionAsset input;

    private void OnEnable()
    {
        LoadInput();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneIndex.GetSceneWithIndex(SceneToload));
    }
    public void LoadInput()
    {

        RebindSaveLoad.Load(input);
        var rebinds = PlayerPrefs.GetString("rebinds");
        print(rebinds);
    }
}
