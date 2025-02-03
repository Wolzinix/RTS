using UnityEngine;

public class MainObjectiveToDestroy : MonoBehaviour
{
    [SerializeField] GameObject _UI;

    [SerializeField] LevelNextUI _FinalScreen;
    public int number = 0;

    private void OnDestroy()
    {
        if (_UI) { _UI.SetActive(false); }

        if (_FinalScreen) { _FinalScreen.AppearUI(number); }

    }
}
