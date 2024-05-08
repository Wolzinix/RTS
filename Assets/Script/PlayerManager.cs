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
    
    [SerializeField] private InputActionReference mooveCameraInput;
    [SerializeField] private InputActionReference rotateCameraInput;

    [SerializeField] private float speed = 1; 

    
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


    private void Update()
    {
        Camera.main.transform.position += new Vector3(mooveCameraInput.action.ReadValue<Vector2>().x,0,mooveCameraInput.action.ReadValue<Vector2>().y) 
                                          * (Time.deltaTime * speed);
        
        
        var rotation = Camera.main.transform.rotation;
        rotation = new Quaternion(  rotation.x, rotateCameraInput.action.ReadValue<Vector2>().x/180 +rotation.y , rotation.z, rotation.w) ;
        
        Camera.main.transform.rotation = rotation;
    }

    private void DoASelection(InputAction.CallbackContext context )
    {
        RaycastHit hit = DoARayCast();
        if (hit.transform)
        {
            if (hit.transform.GetComponent<NavMeshAgent>())
            {
                _selectedObject = hit.transform.gameObject;
            }
        }
        Debug.Log(_selectedObject);
    }

    private void MooveSelected(InputAction.CallbackContext context)
    {
        if (_selectedObject)
        {
            if (_selectedObject.GetComponent<NavMeshAgent>())
            {
                RaycastHit hit = DoARayCast();
                _selectedObject.GetComponent<NavMeshAgent>().SetDestination(hit.point);
            }
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

    private void MooveCamera(InputAction.CallbackContext context)
    {
        
    }
}
