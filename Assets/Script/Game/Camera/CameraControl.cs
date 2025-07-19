using Assets.Script.Tools;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CameraControl : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveCameraInput;
    [SerializeField] private InputActionReference rotateCameraInput;
    [SerializeField] private InputActionReference activeRotateCameraInput;
    [SerializeField] private InputActionReference zoomCameraInput;
    [SerializeField] private InputActionReference accelerateInput;

    private bool _accelerateIsActive;
    private bool _rotationActivated;
    
    public float xmax, xmin, zmax, zmin;
    

    [SerializeField] private GameObject mainGround;
    Camera _camera;
    CameraBehaviour _cameraBehaviour;
    public void SetCameraBehaviour(CameraBehaviour cameraBehaviour)
    {
        _cameraBehaviour = cameraBehaviour;
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;

        _camera.transform.position = new Vector3(
            _camera.transform.position.x > xmax ? xmax : _camera.transform.position.x < xmin ? xmin : _camera.transform.position.x
            , _camera.transform.position.y
            , _camera.transform.position.z > zmax ? zmax : _camera.transform.position.z < zmin ? zmin : _camera.transform.position.z);;



        SetCameraBehaviour(_camera.GetComponent<CameraBehaviour>());
    }
    void Start()
    {
        activeRotateCameraInput.action.performed += ActiveRotation;
        activeRotateCameraInput.action.canceled += DesactiveRotation;
        zoomCameraInput.action.performed += Zoom;
        accelerateInput.action.performed += AccelerateInputPressed;
        accelerateInput.action.canceled += AccelerateInputCanceled;

        SetLimitation();
    }

    void Update()
    {
        float y = moveCameraInput.action.ReadValue<Vector2>().y;
        float x = moveCameraInput.action.ReadValue<Vector2>().x;
        if (y != 0 || x != 0 )
        {
            MoveCamera(y,x);
        }
        else 
        {
            if(_camera.fieldOfView != _cameraBehaviour.fov) { _camera.fieldOfView = _cameraBehaviour.fov; }
            StopMoving(); 
        }

        if (_rotationActivated) { RotateCameraY(rotateCameraInput.action.ReadValue<Vector2>().x); }
    }
    private void OnDestroy()
    {
        activeRotateCameraInput.action.performed -= ActiveRotation;
        activeRotateCameraInput.action.canceled -= DesactiveRotation;
        zoomCameraInput.action.performed -= Zoom;
        accelerateInput.action.performed -= AccelerateInputPressed;
        accelerateInput.action.canceled -= AccelerateInputCanceled;
    }

    public void DesactiveZoom() { zoomCameraInput.action.performed -= Zoom; }
    public void ActiveZoom() { zoomCameraInput.action.performed += Zoom; }
    private void AccelerateInputPressed(InputAction.CallbackContext obj) { _accelerateIsActive = true; }
    private void AccelerateInputCanceled(InputAction.CallbackContext obj) { _accelerateIsActive = false; }
    private void ActiveRotation(InputAction.CallbackContext obj) { _rotationActivated = true; }
    private void DesactiveRotation(InputAction.CallbackContext obj) { _rotationActivated = false; }
    public void StopMoving() 
    { 
        if(_cameraBehaviour._rb.velocity != Vector3.zero)
        {
            _cameraBehaviour._rb.velocity = Vector3.zero;
        }
    }
    private void SetLimitation()
    {

        Vector3 sizeOfGround = mainGround.GetComponent<NavMeshSurface>().size;
        Vector3 GroundCoord = mainGround.transform.position + mainGround.GetComponent<NavMeshSurface>().center;


        xmax = sizeOfGround.x / 2 + GroundCoord.x;
        xmin = -sizeOfGround.x / 2 + GroundCoord.x;

        zmax = sizeOfGround.z / 2 + GroundCoord.z;
        zmin = -sizeOfGround.z / 2 + GroundCoord.z;
    }
    private void Zoom(InputAction.CallbackContext obj)
    {
        _cameraBehaviour.Zoom(zoomCameraInput.action.ReadValue<Vector2>().y, _accelerateIsActive);
    }
    private void MoveCamera(float y, float x)
    {
        float[] maxmin = {xmax, xmin, zmax, xmin};
        _cameraBehaviour.MoveCamera(y,
            x,
            _accelerateIsActive,
            maxmin);
    }
   
    private void RotateCameraY(float x)
    {
        _cameraBehaviour.RotateCameraY(x);
    }

}
