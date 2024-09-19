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
    private const float IncrementSpeed = 2;
    private bool _rotationActivated;

    private Rigidbody _rb;
    
    void Start()
    {
        activeRotateCameraInput.action.performed += ActiveRotation;
        activeRotateCameraInput.action.canceled += DesactiveRotation;
        zoomCameraInput.action.performed += Zoom;
        accelerateInput.action.performed += AccelerateInputPressed;
        accelerateInput.action.canceled += AccelerateInputCanceled;

        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveCamera();

        if (_rotationActivated) { RotateCameraY(); }
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
            Vector3 newPosition = new Vector3(zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom * transform.forward.x,
                zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom * transform.forward.y,
                zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom * transform.forward.z)
                                  * (Time.deltaTime * speedOfDeplacement) ;

            if (_accelerateIsActive) { newPosition *= IncrementSpeed; }
            
            transform.position += newPosition;
        }
    }
    private void ActiveRotation(InputAction.CallbackContext obj) {_rotationActivated = true; }
    private void DesactiveRotation(InputAction.CallbackContext obj) {_rotationActivated = false; }
    
    private void MoveCamera()
    {
        Vector3 newPosition = new Vector3(moveCameraInput.action.ReadValue<Vector2>().y * transform.forward.x + moveCameraInput.action.ReadValue<Vector2>().x * transform.right.x
            , 0
            , moveCameraInput.action.ReadValue<Vector2>().y * transform.forward.z + moveCameraInput.action.ReadValue<Vector2>().x * transform.right.z);

        newPosition *= 10;


        if (_accelerateIsActive) { newPosition *= IncrementSpeed;}
        _rb.velocity = newPosition;
    }
    
    private void RotateCameraY()
    {
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles += new Vector3(0, rotateCameraInput.action.ReadValue<Vector2>().x / 5, 0);
        transform.rotation = rotation;
    }

}
