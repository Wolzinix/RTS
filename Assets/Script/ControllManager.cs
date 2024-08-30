using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ControllManager : MonoBehaviour
{
    [SerializeField] private InputActionReference moveCameraInput;
    [SerializeField] private InputActionReference rotateCameraInput;
    [SerializeField] private InputActionReference activeRotateCameraInput;
    [SerializeField] private InputActionReference zoomCameraInput;
    [SerializeField] private InputActionReference accelerateInput;
    
    [SerializeField] private InputActionReference multiSelectionInput;
    [SerializeField] private InputActionReference multiPathInput;
    [SerializeField] private InputActionReference dragSelect;
    [SerializeField] private InputActionReference selectEntityInput;
    [SerializeField] private InputActionReference moveEntityInput;

    [SerializeField] private float speedOfDeplacement = 1;
    [SerializeField] private float speedOfZoom = 10;


    private bool _accelerateIsActive;
    private float _incrasementSpeed = 10;


    
    private bool _rotationActivated = false;
    private Camera _camera;
    private bool _multiSelectionIsActive;
    private bool _multiPathIsActive;

    private Vector3 _dragCoord;
    [SerializeField] private RectTransform dragBox;

    private bool _dragging;

    private SelectManager _selectManager;
    

    void Start()
    {
        _selectManager = FindObjectOfType<SelectManager>();
        _camera = Camera.main;
        
        selectEntityInput.action.started += DoASelection;
        moveEntityInput.action.started += MooveSelected;
        multiSelectionInput.action.performed += ActiveMultiSelection;
        multiSelectionInput.action.canceled += ActiveMultiSelection;
        multiPathInput.action.performed += ActiveMultiPath;
        multiPathInput.action.canceled += ActiveMultiPath;
        dragSelect.action.performed += StartDragSelect;
        dragSelect.action.canceled += EndDragSelect;
        
        activeRotateCameraInput.action.performed += ActiveRotation;
        activeRotateCameraInput.action.canceled += DesactiveRotation;
        zoomCameraInput.action.performed += Zoom;
        accelerateInput.action.performed += AccelerateInputPressed;
        accelerateInput.action.canceled += AccelerateInputCanceled;
    }

    private void AccelerateInputPressed(InputAction.CallbackContext obj) { _accelerateIsActive = true; }
    
    private void AccelerateInputCanceled(InputAction.CallbackContext obj) { _accelerateIsActive = false; }
    private void ActiveMultiSelection(InputAction.CallbackContext obj) { _multiSelectionIsActive = !_multiSelectionIsActive; }
    private void ActiveMultiPath(InputAction.CallbackContext obj) { _multiPathIsActive = !_multiPathIsActive; }
    private void Zoom(InputAction.CallbackContext obj)
    {
        if (_camera.transform.position.y >= 3 && zoomCameraInput.action.ReadValue<Vector2>().y >= 0 || 
            _camera.transform.position.y <= 8 && zoomCameraInput.action.ReadValue<Vector2>().y <= 0)
        {
            Vector3 newPosition = new Vector3(0,
                -zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom,
                zoomCameraInput.action.ReadValue<Vector2>().y / speedOfZoom)* (Time.deltaTime * speedOfDeplacement);

            if (_accelerateIsActive) { newPosition *= _incrasementSpeed; }
            
            _camera.transform.position += newPosition;
        }
    }
    private void ActiveRotation(InputAction.CallbackContext obj) {_rotationActivated = true; }
    private void DesactiveRotation(InputAction.CallbackContext obj) {_rotationActivated = false; }

    private void OnDestroy()
    {
        selectEntityInput.action.started -= DoASelection;
        moveEntityInput.action.started -= MooveSelected;
        multiSelectionInput.action.performed -= ActiveMultiSelection;
        multiSelectionInput.action.canceled -= ActiveMultiSelection;
        multiPathInput.action.performed -= ActiveMultiPath;
        multiPathInput.action.canceled -= ActiveMultiPath;
        dragSelect.action.performed -= StartDragSelect;
        dragSelect.action.canceled -= EndDragSelect;
        
        activeRotateCameraInput.action.performed -= ActiveRotation;
        activeRotateCameraInput.action.canceled -= DesactiveRotation;
        zoomCameraInput.action.performed -= Zoom;
        accelerateInput.action.performed -= AccelerateInputPressed;
        accelerateInput.action.canceled -= AccelerateInputCanceled;
    }


    private void Update()
    {
        MoveCamera();

        if (_dragging)
        {
            float longueur =  Input.mousePosition.x - _dragCoord.x;
            float largeur =  Input.mousePosition.y - _dragCoord.y;

            dragBox.anchoredPosition = new Vector2(_dragCoord.x,_dragCoord.y) + new Vector2(longueur / 2, largeur / 2);
            dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        }
        //if(_rotationActivated) RotateCamera();
    }

    private void MoveCamera()
    {
        Vector3 newPosition = new Vector3(moveCameraInput.action.ReadValue<Vector2>().x
                                  ,0
                                  ,moveCameraInput.action.ReadValue<Vector2>().y) 
                              * (Time.deltaTime * speedOfDeplacement);
        
        if (_accelerateIsActive) { newPosition *= _incrasementSpeed;}
        
        _camera.transform.position += newPosition;
    }

    private void DoASelection(InputAction.CallbackContext context )
    {
        RaycastHit hit = DoARayCast();
        if (hit.transform)
        {
            if (!_multiSelectionIsActive) _selectManager.ClearList();
            
            if (hit.transform.GetComponent<EntityController>() ) _selectManager.AddSelect(hit.transform.gameObject.GetComponent<EntityController>());
               
            else _selectManager.ClearList();
        }
        else if (!_multiSelectionIsActive) _selectManager.ClearList();
    }

    private void MooveSelected(InputAction.CallbackContext context)
    {
        RaycastHit hit = DoARayCast();
        if (!_multiPathIsActive)
        {
            _selectManager.ResetOrder();
        }
        _selectManager.ActionGroup(hit);
            
    }
    
    private RaycastHit DoARayCast()
    {
        Ray ray = _camera.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, 100)) return hit;
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

    private void StartDragSelect(InputAction.CallbackContext obj)
    {
        _dragCoord = Input.mousePosition;
        dragBox.GameObject().SetActive(true);
        _dragging = true;
    }


    private void EndDragSelect(InputAction.CallbackContext obj)
    {
        float longueur =  Input.mousePosition.x - _dragCoord.x;
        float largeur =  Input.mousePosition.y - _dragCoord.y;

        dragBox.anchoredPosition = new Vector2(_dragCoord.x,_dragCoord.y) + new Vector2(longueur / 2, largeur / 2);
        dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        
        Bounds bounds = new Bounds(dragBox.anchoredPosition, dragBox.sizeDelta);
        
        foreach (EntityController i in FindObjectsOfType<EntityController>() )
        {
            if (UnitInDragBox(_camera.WorldToScreenPoint(i.transform.position), bounds))
            {
                _selectManager.AddSelect(i);
            }
        }

        dragBox.GameObject().SetActive(false);
    }

    private bool UnitInDragBox(Vector2 coords, Bounds bounds)
    {
        return coords.x >= bounds.min.x && coords.x <= bounds.max.x && coords.y >= bounds.min.y && coords.y <= bounds.max.y;
    }
}
