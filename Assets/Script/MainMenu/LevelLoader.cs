using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public int SceneToload;

    public Button button;
    public Image LoadingBar;

    public GameObject uiGestionneur;

    private void Start()
    {
        LoadNextScene();
    }
    public void LoadNextScene()
    {
        StartCoroutine( LoadSceneAsync(  SceneIndex.GetIndexOfScene(SceneToload) ));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName ,LoadSceneMode.Additive);

        while (!asyncOperation.isDone)
        {
            float progressionValue = asyncOperation.progress;
            LoadingBar.fillAmount = progressionValue;
            yield return null;
        }

        Time.timeScale = 0;
        uiGestionneur = FindAnyObjectByType<UiGestioneur>().gameObject;
        uiGestionneur.SetActive(false);
        button.gameObject.SetActive(true);
        yield return null;
    }
}
