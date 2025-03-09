using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLoader : MonoBehaviour
{
    public void ChangeScene()
    {
        LevelLoader levelLoader = GetComponent<LevelLoader>();
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.SetActiveScene(SceneManager.GetActiveScene());

        levelLoader.uiGestionneur.SetActive(true);
        SceneIndex.GetIndexOfScene(levelLoader.SceneToload);
        SceneManager.UnloadSceneAsync(scene);
        Time.timeScale = 1;
    }
}
