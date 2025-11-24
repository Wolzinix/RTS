using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonRebind : Button
{
    private InputAction action;
    private InputBinding inputBinding;
    private int bindingIndex;
    [SerializeField] private TMP_Text TextInfo;
    [SerializeField] private TMP_Text TextTouche;
    private string text;

    void RemapButtonClicked()
    {
        action.Disable();
        var rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.2f)
            .OnComplete(operation => 
            {
                text = action.GetBindingDisplayString().Replace("/", "|").Split("|")[bindingIndex];
                ActualiseText(); 
            })
            .Start();
    }

    public void SetAction(InputAction input, int bindingIndex,string text)
    {
        onClick.AddListener(delegate { RemapButtonClicked(); });
        action = input;
        this.bindingIndex = bindingIndex;
        this.text = text;
        ActualiseText();
    }
    
    private void ActualiseText()
    {
        action.Enable();
        inputBinding = action.bindings[bindingIndex];
        TextTouche.text = inputBinding.effectivePath.Split("/")[0];
        TextTouche.text = text;
        //string name = inputBinding.name == "" ? name = action.name : name = inputBinding.name;
        //TextInfo.text = name;
    }
}
