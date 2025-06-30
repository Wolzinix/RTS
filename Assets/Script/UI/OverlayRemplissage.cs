using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayRemplissage : MonoBehaviour
{
    [SerializeField] Image AbilityOverlay;
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpOverlay(Vector3 coord,string Name, Sprite image)
    {
        AbilityOverlay.transform.parent.position = coord;
        AbilityOverlay.sprite = image;
        AbilityOverlay.GetComponentInChildren<TMP_Text>().text = Name;
    }
    public void ActualiseOverlay(string Name, Sprite image)
    {
        AbilityOverlay.sprite = image;
        AbilityOverlay.GetComponentInChildren<TMP_Text>().text = Name;
    }
}
