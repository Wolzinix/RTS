using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rebind : MonoBehaviour
{
    [SerializeField] InputActionAsset input;
    [SerializeField] GameObject buttonToCreate;
    [SerializeField] GameObject TextToCreate;
    [SerializeField] GameObject Destination;
    [SerializeField] GameObject ScreenOfWaitingForRebind;

    private void InstantiateEmptyGM(GameObject go, int nbOfRepetition)
    {
        for (int i = 0; i < nbOfRepetition; i++) { Instantiate(go, Destination.transform); }
        
    }
    void OnEnable() 
    {
        GameObject gm = new();
        gm.AddComponent<RectTransform>();
        gm.isStatic = true;
        int IndexOfButton = 0;
        ButtonRebind[] ListOfButton = GetComponentsInChildren<ButtonRebind>();
        foreach (InputAction i in input.actionMaps[0].actions)
        {
            int index = 0;
            int error = 0;
            if(ListOfButton.Count()==0)
            {

                SetActions(i);
            }

            var displayOptions = InputBinding.DisplayStringOptions.DontUseShortDisplayNames ;
            String cara;
            if (i.bindings.Count>2 && ListOfButton.Count() == 0)
            {
                InstantiateEmptyGM(gm, 2);
            }

            foreach (InputBinding w in  i.bindings)
            {
                cara = i.GetBindingDisplayString(index, displayOptions);
                if(!cara.Contains("|") && i.bindings.Count > 0 && index < i.bindings.Count)
                {
                    if ((index- error) % 2 == 0 && i.bindings.Count > 2 && ListOfButton.Count() == 0) 
                    {
                        InstantiateEmptyGM(gm, 1);
                    }
                    ButtonRebind button = ListOfButton.Count() > IndexOfButton ? ListOfButton[IndexOfButton]  : null ;
                    SetBindings(i, index, cara, button);
                    IndexOfButton += 1;
                }
                else{ error += 1; }

                index += 1;
            }
            if (i.bindings.Count == 1 && ListOfButton.Count() == 0)
            {
                InstantiateEmptyGM(gm, 1);
            }
        }
    }
    private void SetActions(InputAction input)
    {
        GameObject TitleSection = Instantiate(TextToCreate, Destination.transform);
        TitleSection.GetComponentInChildren<TMP_Text>().text = input.name;
    }

    private void SetBindings(InputAction action, int bindingIndex,string text, ButtonRebind button = null)
    {
        if(!button)
        {
            button = Instantiate(buttonToCreate, Destination.transform).GetComponent<ButtonRebind>();
        }
        button.GetComponent<ButtonRebind>().ScreenOfWaitingForRebind = ScreenOfWaitingForRebind;
        button.GetComponent<ButtonRebind>().SetAction(action,bindingIndex,text);
    }

    public void Save()
    {
        RebindSaveLoad.Save(input);
    }

}
