using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rebind : MonoBehaviour
{
    [SerializeField] InputActionAsset input;
    [SerializeField] GameObject buttonToCreate;
    [SerializeField] GameObject TextToCreate;
    [SerializeField] GameObject Destination;

    private void InstantiateEmptyGM(GameObject go, int nbOfRepetition)
    {
        for (int i = 0; i < nbOfRepetition; i++) { Instantiate(go, Destination.transform); }
        
    }
    void Start() 
    {
        GameObject gm = new();
        gm.AddComponent<RectTransform>();

        foreach (InputAction i in input.actionMaps[0].actions)
        {
            int index = 0;
            SetActions(i);
            String[] ListOfBinding= i.GetBindingDisplayString().Replace("/", "|").Split("|");

            if(ListOfBinding.Length>2)
            {
                InstantiateEmptyGM(gm, 2);
            }

            foreach (InputBinding w in  i.bindings) 
            {
                if (ListOfBinding.Length > 0 && index < ListOfBinding.Length)
                {
                    if(index % 2 == 0 && ListOfBinding.Length > 2)
                    {
                        InstantiateEmptyGM(gm, 1);
                    }
                    SetBindings(i, index, ListOfBinding[index]);
                }
                index += 1;
            }
            if (ListOfBinding.Length == 1)
            {
                InstantiateEmptyGM(gm, 1);
            }
        }
    }
    private void SetActions(InputAction input)
    {
        GameObject button = Instantiate(TextToCreate, Destination.transform);
        button.GetComponentInChildren<TMP_Text>().text = input.name;
    }

    private void SetBindings(InputAction action, int bindingIndex,string text)
    {
        GameObject button = Instantiate(buttonToCreate, Destination.transform);
        button.GetComponent<ButtonRebind>().SetAction(action,bindingIndex,text);
    }

}
