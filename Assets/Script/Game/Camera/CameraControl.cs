using Unity.AI.Navigation;
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

    public bool isMapCamera;

    private float xmax, xmin, zmax, zmin;

    public float ymax, ymin;

    private Rigidbody _rb;

    [SerializeField] private GameObject mainGround;
    private float _lastY;

    void Start()
    {
        activeRotateCameraInput.action.performed += ActiveRotation;
        activeRotateCameraInput.action.canceled += DesactiveRotation;
        zoomCameraInput.action.performed += Zoom;
        accelerateInput.action.performed += AccelerateInputPressed;
        accelerateInput.action.canceled += AccelerateInputCanceled;

        _rb = GetComponent<Rigidbody>();
        SetLimitation();

        //transform.position = mainGround.GetComponent<NavMeshSurface>().transform.position + mainGround.GetComponent<NavMeshSurface>().center ;

        transform.position = new Vector3(
            transform.position.x > xmax ? xmax : transform.position.x < xmin ? xmin : transform.position.x
            , transform.position.y
            , transform.position.z > zmax ? zmax : transform.position.z < zmin ? zmin : transform.position.z);
        _lastY = RaycastForGround(transform.position).y;
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

    public void DesactiveZoom() { zoomCameraInput.action.performed -= Zoom; }

    public void ActiveZoom() { zoomCameraInput.action.performed += Zoom; }
    private void AccelerateInputPressed(InputAction.CallbackContext obj) { _accelerateIsActive = true; }
    private void AccelerateInputCanceled(InputAction.CallbackContext obj) { _accelerateIsActive = false; }
    private void ActiveRotation(InputAction.CallbackContext obj) { _rotationActivated = true; }
    private void DesactiveRotation(InputAction.CallbackContext obj) { _rotationActivated = false; }
    public void StopMoving() { _rb.velocity = Vector3.zero; }
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
        if (transform.position.y >= ymin && zoomCameraInput.action.ReadValue<Vector2>().y >= 0 ||
            transform.position.y <= ymax && zoomCameraInput.action.ReadValue<Vector2>().y <= 0)
        {
            Vector3 newPosition = new Vector3(zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom * transform.forward.x,
                zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom * transform.forward.y,
                zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom * transform.forward.z)
                                  * (Time.deltaTime * speedOfDeplacement);

            if (_accelerateIsActive) { newPosition *= IncrementSpeed; }

            _rb.MovePosition(transform.position + newPosition);
        }
    }
    private void MoveCamera()
    {
        Vector3 newPosition = new Vector3(moveCameraInput.action.ReadValue<Vector2>().y * transform.forward.x + moveCameraInput.action.ReadValue<Vector2>().x * transform.right.x
            , 0
            , moveCameraInput.action.ReadValue<Vector2>().y * transform.forward.z + moveCameraInput.action.ReadValue<Vector2>().x * transform.right.z);


        if (isMapCamera)
        {
            newPosition = new Vector3(moveCameraInput.action.ReadValue<Vector2>().y * transform.up.x + moveCameraInput.action.ReadValue<Vector2>().x * transform.right.x
            , 0
            , moveCameraInput.action.ReadValue<Vector2>().y * transform.up.z + moveCameraInput.action.ReadValue<Vector2>().x * transform.right.z);
        }
        newPosition *= 10;

        

        if (_accelerateIsActive) { newPosition *= IncrementSpeed; }

        _rb.velocity = newPosition;

        Vector3 distanceGround = RaycastForGround(transform.position);
        ymin += distanceGround.y - _lastY;
        ymax += distanceGround.y - _lastY;

        transform.position = new Vector3(
            transform.position.x > xmax ? xmax : transform.position.x < xmin ? xmin : transform.position.x
            , transform.position.y + distanceGround.y - _lastY
            , transform.position.z > zmax ? zmax : transform.position.z < zmin ? zmin : transform.position.z);
        _lastY = distanceGround.y;

    }
    private Vector3 RaycastForGround(Vector3 pos)
    {
        Ray ray = new Ray(pos, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<NavMeshSurface>())
            {
                return hit.point;
            }
        }
        return transform.position;
    }
    private void RotateCameraY()
    {
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles += new Vector3(0, rotateCameraInput.action.ReadValue<Vector2>().x / 5, 0);
        _rb.MoveRotation(rotation);
    }

}
