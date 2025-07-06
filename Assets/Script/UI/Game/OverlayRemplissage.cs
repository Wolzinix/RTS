using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayRemplissage : MonoBehaviour
{
    [SerializeField] Image AbilityOverlay;
    TMP_Text _text;
    TMP_Text _Description;

    public void SetUpOverlay(Vector3 coord,string Name, Sprite image)
    {
        AbilityOverlay.transform.parent.position = coord;
        AbilityOverlay.sprite = image;

        if (!_text) { GetText(); }
        _text.text = Name;
    }

    public void SetUpOverlay(Vector3 coord, CapacityController capacity)
    {
        AbilityOverlay.transform.parent.position = coord;
        AbilityOverlay.sprite = capacity.sprite;

        if (!_text) { GetText(); }
        _text.text = capacity.Name;
        _Description.text = capacity.Description;
    }
    public void ActualiseOverlay(string Name, Sprite image)
    {
        AbilityOverlay.sprite = image;
        _text.text = Name;
    }
    public void ActualiseOverlay(CapacityController capacity)
    {
        AbilityOverlay.sprite = capacity.sprite;
        _text.text = capacity.Name;
        _Description.text = capacity.Description;
    }

    private void GetText()
    {
        List<TMP_Text> listofComponent = AbilityOverlay.transform.parent.GetComponentsInChildren<TMP_Text>().ToList();
        _text = listofComponent[0];

        _Description = listofComponent[1];
    }
}
