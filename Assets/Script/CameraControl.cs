using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    
    [SerializeField] private InputActionReference moveCameraInput;
    [SerializeField] private InputActionReference rotateCameraInput;
    [SerializeField] private InputActionReference activeRotateCameraInput;
    [SerializeField] private InputActionReference zoomCameraInput;
    [SerializeField] private InputActionReference accelerateInput;
    
    [SerializeField] private float speedOfDeplacement = 1;
    [SerializeField] private float speedOfZoom = 10;

    private bool _accelerateIsActive;
    private float _incrasementSpeed = 10;
    private bool _rotationActivated = false;
    
    // Start is called before the first frame update
    void Start()
    {
        activeRotateCameraInput.action.performed += ActiveRotation;
        activeRotateCameraInput.action.canceled += DesactiveRotation;
        zoomCameraInput.action.performed += Zoom;
        accelerateInput.action.performed += AccelerateInputPressed;
        accelerateInput.action.canceled += AccelerateInputCanceled;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        
        if(_rotationActivated) RotateCamera();
    }

    private void OnDestroy()
    {
        activeRotateCameraInput.action.performed -= ActiveRotation;
        activeRotateCameraInput.action.canceled -= DesactiveRotation;
        zoomCameraInput.action.performed -= Zoom;
        accelerateInput.action.performed -= AccelerateInputPressed;
        accelerateInput.action.canceled -= AccelerateInputCanceled;
    }

    private void AccelerateInputPressed(InputAction.CallbackContext obj) { _accelerateIsActive = true; }
    
    private void AccelerateInputCanceled(InputAction.CallbackContext obj) { _accelerateIsActive = false; }
    
    private void Zoom(InputAction.CallbackContext obj)
    {
        if (transform.position.y >= 3 && zoomCameraInput.action.ReadValue<Vector2>().y >= 0 || 
            transform.position.y <= 8 && zoomCameraInput.action.ReadValue<Vector2>().y <= 0)
        {
            Vector3 newPosition = new Vector3(0,
                -zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom,
                zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom)* (Time.deltaTime * speedOfDeplacement);

            if (_accelerateIsActive) { newPosition *= _incrasementSpeed; }
            
            transform.position += newPosition;
        }
    }
    private void ActiveRotation(InputAction.CallbackContext obj) {_rotationActivated = true; }
    private void DesactiveRotation(InputAction.CallbackContext obj) {_rotationActivated = false; }
    
    private void MoveCamera()
    {
        if (moveCameraInput.action.ReadValue<Vector2>().x != 0 || moveCameraInput.action.ReadValue<Vector2>().y != 0)
        {
            Vector3 cameraForward = transform.forward;
            
            Vector3 newPosition = new Vector3(moveCameraInput.action.ReadValue<Vector2>().x * cameraForward.x
                                      , 0
                                      , moveCameraInput.action.ReadValue<Vector2>().y * cameraForward.z)
                                  * (Time.deltaTime * speedOfDeplacement);
            
            if (_accelerateIsActive) { newPosition *= _incrasementSpeed;}
        
            transform.position += newPosition;
        }
    }
    
    private void RotateCamera()
    {
        Quaternion rotation = transform.rotation;

        rotation.eulerAngles += new Vector3(0, rotateCameraInput.action.ReadValue<Vector2>().x, 0);

        transform.rotation = rotation;
    }

}
