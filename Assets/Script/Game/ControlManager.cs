using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ControlManager : MonoBehaviour
{
    [SerializeField] private InputActionReference multiSelectionInput;
    [SerializeField] private InputActionReference multiPathInput;
    [SerializeField] private InputActionReference dragSelect;
    [SerializeField] private InputActionReference selectEntityInput;
    [SerializeField] private InputActionReference moveEntityInput;
   
    private Camera _camera;
    private bool _multiSelectionIsActive;
    private bool _multiPathIsActive;

    private Vector3 _dragCoord;
    [SerializeField] private RectTransform dragBox;
    private bool _dragging;

    private SelectManager _selectManager;
    
    [SerializeField] private EntityUiManager ui;
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
    }
   
    private void ActiveMultiSelection(InputAction.CallbackContext obj) { _multiSelectionIsActive = !_multiSelectionIsActive; }
    private void ActiveMultiPath(InputAction.CallbackContext obj) { _multiPathIsActive = !_multiPathIsActive; }
    
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
    }
    
    private void Update()
    {
        if (_dragging)
        {
            float longueur =  Input.mousePosition.x - _dragCoord.x;
            float largeur =  Input.mousePosition.y - _dragCoord.y;

            dragBox.anchoredPosition = new Vector2(_dragCoord.x,_dragCoord.y) + new Vector2(longueur / 2, largeur / 2);
            dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        }
    }
    private void DoASelection(InputAction.CallbackContext context )
    {
        if (DoUiCast().Count == 0)
        {
            RaycastHit hit = DoARayCast();
            if (hit.transform)
            {
                if (!_multiSelectionIsActive) { _selectManager.ClearList(); }

                if (hit.transform.GetComponent<EntityController>())
                {
                    ui.SetEntity(hit.transform.gameObject.GetComponent<EntityManager>());
                    ui.gameObject.SetActive(true);
                    _selectManager.AddSelect(hit.transform.gameObject.GetComponent<EntityController>());
                }

                else
                {
                    _selectManager.ClearList();
                    ui.gameObject.SetActive(false);
                }
            }
            else if (!_multiSelectionIsActive) { _selectManager.ClearList(); }
        }
    }

    private void MooveSelected(InputAction.CallbackContext context)
    {
        RaycastHit hit = DoARayCast();
        if (!_multiPathIsActive)
        {
            ResetOrder();
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

    private  List<RaycastResult> DoUiCast()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();
        eventData.position =  Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, results);
        
        return results; 
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

    public void ResetOrder()
    {
        _selectManager.ResetOrder();
    }
}
