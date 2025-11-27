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
    public GameObject ScreenOfWaitingForRebind;
    InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void RemapButtonClicked()
    {
        action.Disable();
        ScreenOfWaitingForRebind.SetActive(true);
        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<Keyboard>/escape")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.2f)
            .OnCancel(operation => { CleanOperation(); })
            .OnComplete(operation => 
            {
                text = action.GetBindingDisplayString(bindingIndex);
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

        ScreenOfWaitingForRebind.SetActive(false);
        inputBinding = action.bindings[bindingIndex];
        TextTouche.text = inputBinding.effectivePath.Split("/")[0];
        TextTouche.text = text;
    }
    private void CleanOperation()
    {
        rebindOperation.Dispose();
        action?.Enable();
        ScreenOfWaitingForRebind.SetActive(false);
    }
}
