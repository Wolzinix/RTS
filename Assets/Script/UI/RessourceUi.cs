using TMPro;
using UnityEngine;

public class RessourceUi : MonoBehaviour
{
    [SerializeField] TMP_Text _gold;
    int gold;
    [SerializeField] TMP_Text _wood;
    int wood;


    private void Start()
    {
        ActualsieText();
    }

    public void AddGold(int gold)
    {
        gold += gold;
        ActualsieText();
    }

    public void AddWood(int wood)
    {
        wood += wood;
        ActualsieText();
    }

    private void ActualsieText()
    {
        _gold.text = gold + " PO";

        _wood.text = wood + " W";
    }

}
