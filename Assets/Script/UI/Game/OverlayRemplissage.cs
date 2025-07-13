using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayRemplissage : MonoBehaviour
{
    [SerializeField] Image AbilityOverlay;

    [Header("Cost")]
    [SerializeField] Image EntityOverlay;

    [SerializeField] Image GoldImage;
    [SerializeField] Image WoodImage;
    TMP_Text _text;
    TMP_Text _Description;
    RessourceController _RessourceController;

    private void OnEnable()
    {
        if(!_RessourceController) {
            _RessourceController = FindAnyObjectByType<ControlManager>().GetComponent<RessourceController>();
        }
    }
    public void SetUpOverlay(Vector3 coord)
    {
        AbilityOverlay.transform.parent.position = coord;
        EntityOverlay.transform.parent.position = coord;

        if (!_text) { GetText(); }

    }

    public void ActualiseOverlay(CapacityController capacity)
    {
        AbilityIsOn();

        AbilityOverlay.sprite = capacity.sprite;
        _text.text = capacity.Name;
        _Description.text = capacity.Description;
    }

    
    public void ActualiseOverlay(EntityManager entity)
    {
        EntityIsOn();

        EntityOverlay.sprite = entity.GetSprit();
        _text = EntityOverlay.transform.parent.GetComponentInChildren<TMP_Text>();
        _text.text = entity.entityType.ToString();
        if (entity.GoldCost > 0)
        {
            TMP_Text goldText = GoldImage.GetComponentInChildren<TMP_Text>();
            goldText.text = "";
            GoldImage.gameObject.SetActive(true);
            goldText.text += "\n" + entity.GoldCost + " / " + _RessourceController.GetGold();
        }
        else { GoldImage.gameObject.SetActive(false); }
        if(entity.WoodCost > 0)
        {
            TMP_Text WoodText = WoodImage.GetComponentInChildren<TMP_Text>();
            WoodImage.gameObject.SetActive(true);
            WoodText.text = "";
            WoodText.text += "\n" + entity.WoodCost + " / " + _RessourceController.GetWood();
        }
        else { WoodImage.gameObject.SetActive(false); }
    }

    private void EntityIsOn()
    {
        EntityOverlay.transform.parent.gameObject.SetActive(true);
        AbilityOverlay.transform.parent.gameObject.SetActive(false);
    }
    private void AbilityIsOn()
    {
        EntityOverlay.transform.parent.gameObject.SetActive(false);
        AbilityOverlay.transform.parent.gameObject.SetActive(true);
    }

    private void GetText()
    {
        List<TMP_Text> listofComponent = AbilityOverlay.transform.parent.GetComponentsInChildren<TMP_Text>().ToList();
        _text = listofComponent[0];

        _Description = listofComponent[1];
    }
}
