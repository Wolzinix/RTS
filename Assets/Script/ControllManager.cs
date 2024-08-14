using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllManager : MonoBehaviour
{
    [SerializeField] private InputActionReference selectEntityInput;
    [SerializeField] private InputActionReference mooveEntityInput;
    [SerializeField] private InputActionReference mooveCameraInput;
    [SerializeField] private InputActionReference rotateCameraInput;
    [SerializeField] private InputActionReference activeRotateCameraInput;
    [SerializeField] private InputActionReference zoomCameraInput;
    [SerializeField] private InputActionReference multiSelectionInput;
    [SerializeField] private InputActionReference multiPathInput;
    [SerializeField] private InputActionReference dragSelect;
    
    [SerializeField] private float speed = 1;
    [SerializeField] private float speedOfZoom = 10;


    
    //private bool _rotationActivated = false;
    private Camera _camera;
    private bool _multiSelectionIsActive = false;
    private bool _multiPathIsActive = false;

    private Vector3 _dragCoord;
    [SerializeField] private RectTransform dragBox;

    private bool _dragging;

    private SelectManager _selectManager;
    

    void Start()
    {
        _selectManager = FindObjectOfType<SelectManager>();
        _camera = Camera.main;
        selectEntityInput.action.started += DoASelection;
        mooveEntityInput.action.started += MooveSelected;
        activeRotateCameraInput.action.performed += ChangeRotate;
        zoomCameraInput.action.performed += Zoom;
        activeRotateCameraInput.action.canceled += ChangeRotate;
        multiSelectionInput.action.performed += ActiveMultiSelection;
        multiSelectionInput.action.canceled += ActiveMultiSelection;
        multiPathInput.action.performed += ActiveMultiPath;
        multiPathInput.action.canceled += ActiveMultiPath;
        dragSelect.action.performed += StartDragSelect;
        dragSelect.action.canceled += EndDragSelect;
    }

    private void ActiveMultiSelection(InputAction.CallbackContext obj)
    {
        _multiSelectionIsActive = !_multiSelectionIsActive;
    }
    
    private void ActiveMultiPath(InputAction.CallbackContext obj)
    {
        _multiPathIsActive = !_multiPathIsActive;
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
        //if (obj.performed)_rotationActivated = true;
        
        //if (obj.canceled) _rotationActivated = false;
    }

    private void OnDestroy()
    {
        selectEntityInput.action.started -= DoASelection;
        mooveEntityInput.action.started -= MooveSelected;
        activeRotateCameraInput.action.performed -= ChangeRotate;
        zoomCameraInput.action.performed -= Zoom;
        activeRotateCameraInput.action.canceled -= ChangeRotate;
        multiSelectionInput.action.performed -= ActiveMultiSelection;
        multiSelectionInput.action.canceled -= ActiveMultiSelection; 
        multiPathInput.action.started -= ActiveMultiPath;
        multiPathInput.action.canceled -= ActiveMultiPath;
    }


    private void Update()
    {
        _camera.transform.position += new Vector3(mooveCameraInput.action.ReadValue<Vector2>().x
                                          ,0
                                          ,mooveCameraInput.action.ReadValue<Vector2>().y) 
                                      * (Time.deltaTime * speed);

        if (_dragging)
        {
            float longueur =  Input.mousePosition.x - _dragCoord.x;
            float largeur =  Input.mousePosition.y - _dragCoord.y;

            dragBox.anchoredPosition = new Vector2(_dragCoord.x,_dragCoord.y) + new Vector2(longueur / 2, largeur / 2);
            dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        }
        //if(_rotationActivated) RotateCamera();
    }

    private void DoASelection(InputAction.CallbackContext context )
    {
        RaycastHit hit = DoARayCast();
        if (hit.transform)
        {
            if (!_multiSelectionIsActive) _selectManager.ClearList();
            
            if (hit.transform.GetComponent<EntityController>()) _selectManager.AddSelect(hit.transform.gameObject.GetComponent<EntityController>());
               
            else _selectManager.ClearList();
        }
        else if (!_multiSelectionIsActive) _selectManager.ClearList();
    }

    private void MooveSelected(InputAction.CallbackContext context)
    {
        RaycastHit hit = DoARayCast();
        if (!_multiPathIsActive)
        {
            _selectManager.ResetPath();
        }
        _selectManager.MooveSelected(hit);
            
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
