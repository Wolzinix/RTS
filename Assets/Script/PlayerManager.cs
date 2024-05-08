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
        mooveEntityInput.action.started += MooveSelected;
    }

    private void OnDestroy()
    {
        selectEntityInput.action.started -= DoASelection;
        mooveEntityInput.action.started -= MooveSelected;
    }
    
    private void DoASelection(InputAction.CallbackContext context )
    {
        RaycastHit hit = DoARayCast();
        if (hit.transform.GetComponent<NavMeshAgent>())
        {
            _selectedObject = hit.transform.gameObject;
        }
        Debug.Log(_selectedObject);
    }

    private void MooveSelected(InputAction.CallbackContext context)
    {
        if (_selectedObject.GetComponent<NavMeshAgent>())
        {
            RaycastHit hit = DoARayCast();
            _selectedObject.GetComponent<NavMeshAgent>().SetDestination(hit.point);
        }
    }
    
    private RaycastHit DoARayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, 100))
        {
            return hit;
        }

        return hit;
    }
}
