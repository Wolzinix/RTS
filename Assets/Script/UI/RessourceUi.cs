using TMPro;
using UnityEngine;

public class RessourceUi : MonoBehaviour
{
    [SerializeField] TMP_Text _gold;
    int goldNB;
    [SerializeField] TMP_Text _wood;
    int woodNB;


    private void Start()
    {
        ActualsieText();
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

    private void ActualsieText()
    {
        _gold.text = goldNB + " PO";

        _wood.text = woodNB + " W";
    }

}
