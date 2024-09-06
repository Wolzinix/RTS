using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    private bool _patrolOrder;


    private Vector3 _dragCoord;
    [SerializeField] private RectTransform dragBox;
    private bool _dragging;

    private SelectManager _selectManager;

    private bool _order;
    private bool _travelAttack;
    
    [SerializeField] private EntityUiManager entityUi;
    
    [SerializeField] private GroupUiManager groupUi;
    void Start()
    {
        _selectManager = FindObjectOfType<SelectManager>();
        _camera = Camera.main;
        
        selectEntityInput.action.started += DoASelection;
        moveEntityInput.action.started += MooveSelected;
        multiSelectionInput.action.performed += ActiveMultiSelection;
        multiSelectionInput.action.canceled += ActiveMultiSelection;
        multiPathInput.action.performed += ActiveMultiPath;
        multiPathInput.action.canceled += DesactiveMultiPath;
        dragSelect.action.performed += StartDragSelect;
        dragSelect.action.canceled += EndDragSelect;
    }

    private void ActiveMultiSelection(InputAction.CallbackContext obj)
    {
        _multiSelectionIsActive = !_multiSelectionIsActive;
    }

    private void DesactiveMultiPath(InputAction.CallbackContext obj)
    {
        _multiPathIsActive = false;
        _order = false;  
        _patrolOrder = false;
        _travelAttack = false;
    }
    private void ActiveMultiPath(InputAction.CallbackContext obj) { _multiPathIsActive = true; }
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
    private void FixedUpdate()
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
        Physics.SyncTransforms();
        if (_order)
        {
            RaycastHit hit = DoARayCast();
            IsMultipathActive();
            _selectManager.ActionGroup(hit);
        }
        else if (_travelAttack)
        {
            RaycastHit hit = DoARayCast();
            IsMultipathActive();
            _selectManager.AttackingOnTravel(hit.point);
        }

        else if (_patrolOrder)
        {
            RaycastHit hit = DoARayCast();
            IsMultipathActive();
            _selectManager.PatrouilleOrder(hit.point);
            if (!_selectManager.getAddingMoreThanOne()) { _selectManager.setAddingMoreThanOne(true); }
        }
        else
        {
            if (DoUiRayCast().Count == 0)
            {
                RaycastHit hit = DoARayCast();
                ResetUiOrder();
                
                if (!_multiSelectionIsActive) { _selectManager.ClearList(); }
                if (hit.transform)
                {
                    if (hit.transform.GetComponent<EntityController>())
                    {
                        UIGestion(hit.transform.gameObject.GetComponent<EntityManager>());
                        _selectManager.AddSelect(hit.transform.gameObject.GetComponent<EntityController>());
                    }

                    else
                    {
                        _selectManager.ClearList();
                        DesactiveUi();
                    }
                }
                else
                {
                    _selectManager.ClearList();
                    DesactiveUi();
                }
            }
        }
    }

    private void UIGestion(EntityManager entity)
    {
        if (groupUi.gameObject.activeSelf && !_multiSelectionIsActive)
        {
            groupUi.ClearListOfEntity();
            groupUi.gameObject.SetActive(false);
        }
        
        if (entityUi.gameObject.activeSelf && _multiSelectionIsActive)
        {
            groupUi.gameObject.SetActive(true);
            groupUi.AddEntity(entityUi.GetEntity());
            groupUi.AddEntity(entity);
            
            entityUi.gameObject.SetActive(false);
        }
        else if (groupUi.gameObject.activeSelf && _multiSelectionIsActive)
        {
            groupUi.AddEntity(entity);
        }

        if (!entityUi.gameObject.activeSelf && _multiSelectionIsActive && !groupUi.gameObject.activeSelf )
        {
            entityUi.gameObject.SetActive(true);
            entityUi.SetEntity(entity); 
        }
        
        if(!entityUi.gameObject.activeSelf && !_multiSelectionIsActive)
        {
            groupUi.ClearListOfEntity();
            groupUi.gameObject.SetActive(false);
            
            entityUi.gameObject.SetActive(true);
            entityUi.SetEntity(entity); 
        }
    }

    private void DesactiveUi()
    {
        entityUi.gameObject.SetActive(false);
        groupUi.gameObject.SetActive(false);
    }

    private void MooveSelected(InputAction.CallbackContext context)
    {
        if (!_order && !_patrolOrder && !_travelAttack)
        {
            IsMultipathActive();
            RaycastHit hit = DoARayCast();
            
            _selectManager.ActionGroup(hit);
        }
        else { ResetUiOrder(); }
    }

    private void IsMultipathActive()
    {
        if (!_multiPathIsActive)
        {
            ResetOrder();
            ResetUiOrder();
        }
    }

    private void ResetUiOrder()
    {
        _order = false;
        _patrolOrder = false;
        _travelAttack = false;
    }
    
    private RaycastHit DoARayCast()
    {
        Ray ray = _camera.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, Mathf.Infinity)){ return hit;}
        return hit;
    }

    private  List<RaycastResult> DoUiRayCast()
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
                groupUi.gameObject.SetActive(true);
                groupUi.AddEntity(i.gameObject.GetComponent<EntityManager>());
            }
        }
        
        dragBox.anchoredPosition = new Vector2(0, 0);
        dragBox.sizeDelta = new Vector2(0, 0);
        
        dragBox.GameObject().SetActive(false);
        _dragging = false;
    }

    private bool UnitInDragBox(Vector2 coords, Bounds bounds)
    {
        return coords.x >= bounds.min.x && coords.x <= bounds.max.x && coords.y >= bounds.min.y && coords.y <= bounds.max.y;
    }

    public void ResetOrder()
    {
        _selectManager.ResetOrder();
    }

    public void TenirPosition() { _selectManager.TenirPositionOrder(); }

    public void MoveOrder() { _order = true; }

    public void DoPatrouille()
    {
        _patrolOrder = true;
        _selectManager.setAddingMoreThanOne(false);
    }

    public void DoTravelAttack() { _travelAttack = true; }
}
