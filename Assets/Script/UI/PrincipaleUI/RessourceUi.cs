using TMPro;
using UnityEngine;

public class RessourceUi : MonoBehaviour
{
    [SerializeField] TMP_Text _gold;
    [SerializeField] private TMP_Text _wood;

    private int goldNB;
    private int woodNB;
    private RessourceController controller;

    private void SetText()
    {
        _gold.SetText(goldNB.ToString());

        _wood.SetText(woodNB.ToString());
    }

    public void UpdateData()
    {
        goldNB = controller.GetGold();
        woodNB = controller.GetWood();
        SetText();
    }
    private void Start()
    {
        controller = FindAnyObjectByType<ControlManager>().GetComponent<RessourceController>();
        controller.ressourcesAddUI.AddListener(AddRessource);
        UpdateData();
    }
    public void AddGold(int gold)
    {
        goldNB += gold;
        SetText();
    }

    public void AddWood(int wood)
    {
        woodNB += wood;
        SetText();
    }

    public void AddRessource(int gold , int wood)
    {
        AddGold(gold);
        AddWood(wood);
    }
}
