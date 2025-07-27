using TMPro;
using UnityEngine;

public class RessourceUi : MonoBehaviour
{
    [SerializeField] TMP_Text _gold;
    int goldNB;
    [SerializeField] TMP_Text _wood;
    int woodNB;
    RessourceController controller;

    private void Start()
    {
        controller = FindAnyObjectByType<ControlManager>().GetComponent<RessourceController>();
        controller.ressourcesAddUI.AddListener(AddRessource);
        ActualiseData();
    }

    public void AddGold(int gold)
    {
        goldNB += gold;
        ActualsieText();
    }

    public void AddWood(int wood)
    {
        woodNB += wood;
        ActualsieText();
    }

    public void AddRessource(int gold , int wood)
    {
        AddGold(gold);
        AddWood(wood);
    }

    private void ActualsieText()
    {
        _gold.text = goldNB + "";

        _wood.text = woodNB + "";
    }

    public void ActualiseData()
    {
        goldNB = controller.GetGold();
        woodNB = controller.GetWood();
        ActualsieText();
    }

}
