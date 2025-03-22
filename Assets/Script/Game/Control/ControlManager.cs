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
    [SerializeField] private InputActionReference mapModInput;
    [SerializeField] private InputActionReference PauseInput;

    [SerializeField] private Texture2D buildingCursor;

    [SerializeField] private Texture2D DeplacementCursor;
    [SerializeField] private Texture2D AttackCursor;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _mapCamera;

    private bool _multiSelectionIsActive;
    private bool _multiPathIsActive;

    private bool _patrolOrder;

    CapacityController _capacityController;
    private bool _capactityOrder;

    [SerializeField] GameObject _listOfEntity;

    private Vector3 _dragCoord;
    [SerializeField] private RectTransform dragBox;
    private bool _dragging;
    private float _timeOfDragging;

    private SelectManager _selectManager;



    private bool _order;
    private bool _travelAttack;

    private bool _buildingOrder;
    private int _nbOfBuilding;

    private UiGestioneur _UiGestioneur;

    [SerializeField] string _ennemieTag;

    [SerializeField] Canvas _pauseCanvas;

    [SerializeField] private MapMod _mapMod;

    void Start()
    {
        _selectManager = FindObjectOfType<SelectManager>();

        _mapCamera.GetComponent<CameraControl>().DesactiveZoom();

        ActiveAllInput();
        mapModInput.action.started += MapModActive;
        PauseInput.action.started += SetPause;

        _UiGestioneur = FindObjectOfType<UiGestioneur>();

        _selectManager.SetEnnemieTag(_ennemieTag);
        _selectManager.SetAllieTag(gameObject.tag);
    }

    private void ActiveMultiSelection(InputAction.CallbackContext obj)
    {
        _multiSelectionIsActive = true;
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

    private void MapModActive(InputAction.CallbackContext obj)
    {
        _mapMod.MapModActive();

        if (_mapMod._isMapMod)
        {
            selectEntityInput.action.started += TeleporteOnMap;
            _UiGestioneur.gameObject.SetActive(false);
            DesactiveAllInput();
            _selectManager.ClearList();
            _UiGestioneur.DesactiveUi();
        }
        else
        {
            selectEntityInput.action.started -= TeleporteOnMap;
            _UiGestioneur.gameObject.SetActive(true);
            ActiveAllInput();
        }
    }

    private void TeleporteOnMap(InputAction.CallbackContext obj)
    {
        _mapMod.TeleporteMainCamera(DoARayCast(_mapCamera).point);
        selectEntityInput.action.started -= TeleporteOnMap;
        _UiGestioneur.gameObject.SetActive(true);
        ActiveAllInput();
    }

    public void SetPause(InputAction.CallbackContext obj) { ActivePause(); }

    public void ActivePause()
    {
        if (_pauseCanvas.gameObject.activeSelf)
        {
            Time.timeScale = 1;
            _pauseCanvas.gameObject.SetActive(false);
        }
        else
        {
            _pauseCanvas.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void ActiveAllInput()
    {
        selectEntityInput.action.started += LeftClickGestion;
        moveEntityInput.action.started += RightClickGestion;
        multiSelectionInput.action.performed += ActiveMultiSelection;
        multiSelectionInput.action.canceled += DesactiveMultiSelection;
        multiPathInput.action.performed += ActiveMultiPath;
        multiPathInput.action.canceled += DesactiveMultiPath;
        dragSelect.action.performed += StartDragSelect;
        dragSelect.action.canceled += EndDragSelect;
    }

    private void DesactiveAllInput()
    {
        selectEntityInput.action.started -= LeftClickGestion;
        moveEntityInput.action.started -= RightClickGestion;
        multiSelectionInput.action.performed -= ActiveMultiSelection;
        multiSelectionInput.action.canceled -= ActiveMultiSelection;
        multiPathInput.action.performed -= ActiveMultiPath;
        multiPathInput.action.canceled -= ActiveMultiPath;
        dragSelect.action.performed -= StartDragSelect;
        dragSelect.action.canceled -= EndDragSelect;
    }

    private void ActiveMultiPath(InputAction.CallbackContext obj) { _multiPathIsActive = true; }
    private void OnDestroy()
    {
        DesactiveAllInput();
        PauseInput.action.started -= SetPause;
        mapModInput.action.started -= MapModActive;
    }
    private void Update()
    {
        if (_dragging)
        {
            _timeOfDragging += Time.deltaTime;

            float longueur = Input.mousePosition.x - _dragCoord.x;
            float largeur = Input.mousePosition.y - _dragCoord.y;

            dragBox.anchoredPosition = new Vector2(_dragCoord.x, _dragCoord.y) + new Vector2(longueur / 2, largeur / 2);
            dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        }
    }
    private void LeftClickGestion(InputAction.CallbackContext context)
    {
        Physics.SyncTransforms();
        RaycastHit hit = DoARayCast(_camera);

        if (_buildingOrder)
        {
            IsMultipathActive();
            _selectManager.DoABuild(_nbOfBuilding, hit);
        }
        else if (_order)
        {
            IsMultipathActive();
            _selectManager.ActionGroup(hit);
        }
        else if (_capactityOrder)
        {
            IsMultipathActive();
            if (hit.transform && _capacityController && _capacityController.ready)
            {
                if (!hit.transform.gameObject.GetComponent<RessourceManager>())
                {
                    if (_capacityController.GetType().IsSubclassOf(typeof(ActivableCapacity)))
                    {
                        ActivableCapacity capa = (ActivableCapacity)_capacityController;
                        capa.DoOnce();
                    }
                    _capacityController.AddTarget(hit.transform.GetComponent<SelectableManager>()); 
                }
            }
        }
        else if (_travelAttack)
        {
            IsMultipathActive();
            if (hit.transform && hit.transform.GetComponent<SelectableManager>()) { _selectManager.AddTarget(hit.transform.GetComponent<SelectableManager>()); }
            else { _selectManager.AttackingOnTravel(hit.point); }
        }

        else if (_patrolOrder)
        {
            IsMultipathActive();
            _selectManager.PatrouilleOrder(hit.point);
            if (!_selectManager.getAddingMoreThanOne()) { _selectManager.setAddingMoreThanOne(true); }
        }
        else
        {
            DoASelection(hit);
        }
    }

    private void DoASelection(RaycastHit hit)
    {
        List<RaycastResult> listOfUIRay = DoUiRayCast();
        if (listOfUIRay.Count == 0)
        {
            if (!_multiSelectionIsActive) { _selectManager.ClearList(); }
            if (hit.collider)
            {
                Debug.DrawLine(_camera.transform.position, hit.point, color: Color.blue, 10f);
                if (hit.transform.GetComponent<SelectableManager>() &&
                    (hit.transform.GetComponentInChildren<SkinnedMeshRenderer>() && hit.transform.GetComponentInChildren<SkinnedMeshRenderer>().enabled ||
                    hit.transform.GetComponentInChildren<MeshRenderer>() && hit.transform.GetComponentInChildren<MeshRenderer>().enabled))
                {
                    _UiGestioneur.ActualiseUi(hit.transform.gameObject.GetComponent<SelectableManager>());
                    _selectManager.AddSelect(hit.transform.gameObject.GetComponent<SelectableManager>());
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
                    
                    _selectManager.AddSelect(groupUI.GetEntity().GetComponent<SelectableManager>());
                }
            }
        }
    }

    private void RightClickGestion(InputAction.CallbackContext context)
    {
        List<RaycastResult> listOfUIRay = DoUiRayCast();
        if (listOfUIRay.Count == 0)
        {
            if (!_order && !_patrolOrder && !_travelAttack)
            {
                IsMultipathActive();
                RaycastHit hit = DoARayCast(_camera);

                _selectManager.ActionGroup(hit);
            }
            else { ResetUiOrder(); }
        }
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
        _capactityOrder = false;

        Cursor.SetCursor(null, hotSpot, cursorMode);
    }

    private RaycastHit DoARayCast(Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
        return hit;
    }

    private List<RaycastResult> DoUiRayCast()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();
        eventData.position = Input.mousePosition;
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

        if (_timeOfDragging > 0.1) { StartCoroutine(IsOnDragBox()); }
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

        foreach (EntityController i in _listOfEntity.GetComponentsInChildren<EntityController>())
        {
            Vector3 point = _camera.WorldToScreenPoint(i.transform.position);

            if (UnitInDragBox(point, bounds) && i.CompareTag(gameObject.tag))
            {
                _selectManager.AddSelect(i.gameObject.GetComponent<SelectableManager>());
                _UiGestioneur.AddOnGroupUi(i.gameObject.GetComponent<SelectableManager>());
            }
        }

        if (_selectManager._groupManager.getNumberOnGroup() == 1)
        {
            _UiGestioneur.ActualiseUi(_selectManager._groupManager.getSelectList()[0].gameObject.GetComponent<SelectableManager>());
        }
    }

    private bool UnitInDragBox(Vector2 coords, Bounds bounds)
    {
        return coords.x >= bounds.min.x && coords.x <= bounds.max.x && coords.y >= bounds.min.y && coords.y <= bounds.max.y;
    }

    public void ResetOrder() { _selectManager.ResetOrder(); }

    public void TenirPosition() { _selectManager.TenirPositionOrder(); }

    public void MoveOrder()
    {
        ResetUiOrder();
        _order = true;
        Cursor.SetCursor(DeplacementCursor, hotSpot, cursorMode);
    }
    public void CapacityOrder(TroupeManager troupeManager, CapacityController capacity)
    {
        if(capacity.ready)
        {
            ResetUiOrder();
            
            _capactityOrder = true;
            _capacityController = capacity;
            Cursor.SetCursor(DeplacementCursor, hotSpot, cursorMode);
            
        }
    }

    public void ChangeCapacityActif(ActivableCapacity capacity)
    {
        capacity.ChangeActif();
    }

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

        Cursor.SetCursor(AttackCursor, hotSpot, cursorMode);
    }

    public void DoABuilding(int nb)
    {
        _buildingOrder = true;
        _nbOfBuilding = nb;

        Cursor.SetCursor(buildingCursor, hotSpot, cursorMode);
    }
}
