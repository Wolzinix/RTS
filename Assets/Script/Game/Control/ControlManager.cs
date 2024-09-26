using System.Collections;
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
    private float _timeOfDragging;

    public SelectManager _selectManager;

    private bool _order;
    private bool _travelAttack;

    private bool _buildingOrder;
    private int _nbOfBuilding;
    
    private UiGestioneur _UiGestioneur;


    [SerializeField] string _ennemieTag;

    void Start()
    {
        _selectManager = FindObjectOfType<SelectManager>();
        _camera = Camera.main;
        
        selectEntityInput.action.started += DoASelection;
        moveEntityInput.action.started += MooveSelected;
        multiSelectionInput.action.performed += ActiveMultiSelection;
        multiSelectionInput.action.canceled += DesactiveMultiSelection;
        multiPathInput.action.performed += ActiveMultiPath;
        multiPathInput.action.canceled += DesactiveMultiPath;
        dragSelect.action.performed += StartDragSelect;
        dragSelect.action.canceled += EndDragSelect;

        _UiGestioneur = FindObjectOfType<UiGestioneur>();

        _selectManager.SetEnnemieTag(_ennemieTag);
        _selectManager.SetAllieTag(gameObject.tag);

    }

    private void ActiveMultiSelection(InputAction.CallbackContext obj)
    {
        _multiSelectionIsActive =  true;
        _UiGestioneur.SetMulitSelection(_multiSelectionIsActive);
    }

    private void DesactiveMultiSelection(InputAction.CallbackContext obj)
    {
        _multiSelectionIsActive = false;
        _UiGestioneur.SetMulitSelection(_multiSelectionIsActive);
    }

    private void DesactiveMultiPath(InputAction.CallbackContext obj)
    {
        _multiPathIsActive = false;
        ResetUiOrder();
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
    private void Update()
    {
        if (_dragging)
        {
            _timeOfDragging += Time.deltaTime;
            
            float longueur =  Input.mousePosition.x - _dragCoord.x;
            float largeur =  Input.mousePosition.y - _dragCoord.y;

            dragBox.anchoredPosition = new Vector2(_dragCoord.x,_dragCoord.y) + new Vector2(longueur / 2, largeur / 2);
            dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        }
    }
    private void DoASelection(InputAction.CallbackContext context )
    {
        Physics.SyncTransforms();
        if(_buildingOrder)
        {
            RaycastHit hit = DoARayCast();
            IsMultipathActive();
            _selectManager.DoABuild(_nbOfBuilding, hit);
        }
        else if (_order)
        {
            RaycastHit hit = DoARayCast();
            IsMultipathActive();
            _selectManager.ActionGroup(hit);
        }
        else if (_travelAttack)
        {
            RaycastHit hit = DoARayCast();
            IsMultipathActive();
            if(hit.transform && hit.transform.GetComponent<EntityManager>()) { _selectManager.AddTarget(hit.transform.GetComponent<EntityManager>()); }
            else {_selectManager.AttackingOnTravel(hit.point);}
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
            List<RaycastResult> listOfUIRay = DoUiRayCast();
            if (listOfUIRay.Count == 0)
            {
                RaycastHit hit = DoARayCast();
                
                if (!_multiSelectionIsActive) {_selectManager.ClearList();  }
                if (hit.collider)
                {
                    Debug.DrawLine(_camera.transform.position, hit.point, color:Color.blue, 10f);
                    if (hit.transform.GetComponent<EntityManager>())
                    {
                        _UiGestioneur.ActualiseUi(hit.transform.gameObject.GetComponent<EntityManager>());
                        _selectManager.AddSelect(hit.transform.gameObject.GetComponent<EntityManager>());
                    }
                    else
                    {
                        if (!_multiSelectionIsActive)
                        {
                            _selectManager.ClearList();
                            _UiGestioneur.DesactiveUi();
                        }
                    }
                }
                else
                {
                    if (!_multiSelectionIsActive)
                    {
                        _selectManager.ClearList();
                        _UiGestioneur.DesactiveUi();
                    }
                }
            }
            else
            {
                foreach (RaycastResult raycastResult in listOfUIRay)
                {
                    if (raycastResult.gameObject.GetComponent<CadreController>())
                    {
                        ResetUiOrder();
                        _selectManager.ClearList();
                        CadreController groupUI = raycastResult.gameObject.GetComponent<CadreController>();
                        _UiGestioneur.ActualiseUi(groupUI.GetEntity());
                        _selectManager.AddSelect(groupUI.GetEntity().GetComponent<EntityManager>());
                    }
                }
            }
        }
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
        _buildingOrder = false;
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
        _timeOfDragging = 0;
    }

    private void EndDragSelect(InputAction.CallbackContext obj)
    {

        if(_timeOfDragging > 0.1) { StartCoroutine(IsOnDragBox()); }
        dragBox.anchoredPosition = new Vector2(0, 0);
        dragBox.sizeDelta = new Vector2(0, 0);
       
        dragBox.GameObject().SetActive(false);
        _dragging = false;

    }

    IEnumerator IsOnDragBox()
    {
        yield return new WaitForEndOfFrame();

        float longueur = Input.mousePosition.x - _dragCoord.x;
        float largeur = Input.mousePosition.y - _dragCoord.y;

        dragBox.anchoredPosition = new Vector2(_dragCoord.x, _dragCoord.y) + new Vector2(longueur / 2, largeur / 2);
        dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        Bounds bounds = new Bounds(dragBox.anchoredPosition, dragBox.sizeDelta);

        foreach (EntityController i in FindObjectsOfType<EntityController>())
        {
            Vector3 point = _camera.WorldToScreenPoint(i.transform.position);

            if (UnitInDragBox(point, bounds) && i.CompareTag(gameObject.tag))
            {
                _selectManager.AddSelect(i.gameObject.GetComponent<EntityManager>());
                _UiGestioneur.AddOnGroupUi(i.gameObject.GetComponent<EntityManager>());
            }
        }
    }

    private bool UnitInDragBox(Vector2 coords, Bounds bounds)
    {
        return coords.x >= bounds.min.x && coords.x <= bounds.max.x && coords.y >= bounds.min.y && coords.y <= bounds.max.y;
    }

    public void ResetOrder() { _selectManager.ResetOrder();}

    public void TenirPosition() { _selectManager.TenirPositionOrder(); }

    public void MoveOrder() {

        ResetUiOrder(); 
        _order = true; }

    public void DoPatrouille()
    {

        ResetUiOrder();
        _patrolOrder = true;
        _selectManager.setAddingMoreThanOne(false);
    }

    public void DoTravelAttack()
    {

        ResetUiOrder(); 
        _travelAttack = true;
    }

    public void DoABuilding(int nb)
    {
        _buildingOrder = true;
        _nbOfBuilding = nb;
    }
}
