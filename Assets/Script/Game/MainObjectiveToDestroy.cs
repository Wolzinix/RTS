using UnityEngine;

public class MainObjectiveToDestroy : MonoBehaviour
{
    [SerializeField] GameObject _UI;

    [SerializeField] GameObject _FinalScreen;

    private void OnDestroy()
    {
        if (_UI) { _UI.SetActive(false); }

        if (_FinalScreen) { _FinalScreen.SetActive(true); }

    }
}
