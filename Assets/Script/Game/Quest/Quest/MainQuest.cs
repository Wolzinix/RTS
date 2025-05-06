using UnityEngine;

public class MainQuest : Quest
{
    [SerializeField] GameObject _UI;

    [SerializeField] LevelNextUI _FinalScreen;
    public bool IsPlayer;
    protected override void AllObjectifSucces()
    {
        if (_UI) { _UI.SetActive(false); }

        if (_FinalScreen) { _FinalScreen.AppearUI(IsPlayer); }
    }
}
