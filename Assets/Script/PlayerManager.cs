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

    [SerializeField] private InputActionReference activeRotateCameraInput;
    
    
    [SerializeField] private InputActionReference zoomCameraInput;
    
    [SerializeField] private float speed = 1;

    [SerializeField] private float speedOfZoom = 10;

    private bool _rotationActivated = false;
    private Camera _camera;


    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        selectEntityInput.action.started += DoASelection;
        mooveEntityInput.action.started += MooveSelected;
        activeRotateCameraInput.action.performed += ChangeRotate;
        zoomCameraInput.action.performed += Zoom;
        activeRotateCameraInput.action.canceled += ChangeRotate;
    }

    private void Zoom(InputAction.CallbackContext obj)
    {
        if (_camera.transform.position.y >= 3 && zoomCameraInput.action.ReadValue<Vector2>().y >= 0)
        {
            _camera.transform.position += new Vector3(0
                ,-zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom,
                zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom)* (Time.deltaTime * speed);
        }
        
        if (_camera.transform.position.y <= 8 && zoomCameraInput.action.ReadValue<Vector2>().y <= 0)
        {
            _camera.transform.position += new Vector3(0
                ,-zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom,
                zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom)* (Time.deltaTime * speed);
        }
    }

    private void ChangeRotate(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            _rotationActivated = true;
        }
        if (obj.canceled)
        {
            _rotationActivated = false;
        }
    }

    private void OnDestroy()
    {
        selectEntityInput.action.started -= DoASelection;
        mooveEntityInput.action.started -= MooveSelected;
        activeRotateCameraInput.action.started -= ChangeRotate;
    }


    private void Update()
    {
        _camera.transform.position += new Vector3(mooveCameraInput.action.ReadValue<Vector2>().x,0,mooveCameraInput.action.ReadValue<Vector2>().y) 
                                      * (Time.deltaTime * speed);
        
        

        //if(_rotationActivated) RotateCamera();
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
        Ray ray = _camera.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, 100))
        {
            return hit;
        }

        return hit;
    }

    private void RotateCamera()
    {
        // to do
        var rotation = _camera.transform.rotation;

        _camera.transform.rotation = new Quaternion(  rotation.x,
                                                    rotateCameraInput.action.ReadValue<Vector2>().x/360 + rotation.y ,
                                                    rotation.z,
                                                    rotation.w);
    }
}
