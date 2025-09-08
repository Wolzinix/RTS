using TMPro;
using UnityEngine;

public class RessourceInfo : MonoBehaviour
{
    [SerializeField] TMP_Text _gold;
    int goldNB;
    [SerializeField] TMP_Text _wood;
    int woodNB;
    public void SetGold(int gold)
    {
        goldNB = gold;
        ActualsieText();
    }

    public void SetWood(int wood)
    {
        woodNB = wood;
        ActualsieText();
    }

    public void SetRessource(int gold, int wood)
    {
        SetGold(gold);
        SetWood(wood);
    }

    private void ActualsieText()
    {
        _gold.text = goldNB + "";

        _wood.text = woodNB + "";
    }
 }
