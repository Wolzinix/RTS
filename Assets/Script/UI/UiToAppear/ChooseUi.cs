using UnityEngine;

public class ChooseUi : UiAppeirBase
{
    [SerializeField] RessourceUi ressourceUi;
    SaveForNextlevel save;
    RessourceController playerRessource;
    void Start()
    {
        save = FindAnyObjectByType<SaveForNextlevel>();
        playerRessource = FindAnyObjectByType<ControlManager>().GetComponent<RessourceController>();
    }

    public override void AppearUI() 
    {
    }
    
    public override void AppearUI(bool IsPlayer) => throw new System.NotImplementedException();
    public override void DisappearUI()
    {

    }
}
