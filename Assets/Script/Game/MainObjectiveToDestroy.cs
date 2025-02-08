using UnityEngine;

public class MainObjectiveToDestroy : MonoBehaviour
{
    [SerializeField] GameObject _UI;

    [SerializeField] LevelNextUI _FinalScreen;
    public bool IsPlayer;

    private void OnDestroy()
    {
        if (_UI) { _UI.SetActive(false); }

        if (_FinalScreen) { _FinalScreen.AppearUI(IsPlayer); }

    }
}
