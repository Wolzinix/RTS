using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private GameObject _selectedObject;
    
    [SerializeField] private InputActionReference selectEntityInput;
    
    [SerializeField] private InputActionReference mooveEntityInput;
    // Start is called before the first frame update
    void Start()
    {
        selectEntityInput.action.started += DoASelection;
    }

    private void OnDisable()
    {
        selectEntityInput.action.started -= DoASelection;
    }
    
    private void DoASelection(InputAction.CallbackContext context )
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, 100)) {
            if (hit.transform.GetComponent(typeof(NavMeshAgent)))
            {
                _selectedObject = hit.transform.gameObject;
            }
        }
    }
    
    
}
