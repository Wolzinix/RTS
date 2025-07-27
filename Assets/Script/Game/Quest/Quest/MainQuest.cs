using UnityEngine;

public class MainQuest : Quest
{
    [SerializeField] GameObject _UI;

    [SerializeField] UiAppeirBase _ScreenToAppeir;
    public bool IsSuccesLevel;
    public bool EntityNextLevel;
    protected override void AllObjectifSucces()
    {
        if (_UI) { _UI.SetActive(false); }

        if (_ScreenToAppeir && !EntityNextLevel) { _ScreenToAppeir.AppearUI(IsSuccesLevel); }

        if (_ScreenToAppeir && EntityNextLevel) { _ScreenToAppeir.AppearUI(); }
    }
}
