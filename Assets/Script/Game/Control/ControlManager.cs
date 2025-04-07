using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ControlManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _multiSelectionInput;
    [SerializeField] private InputActionReference _multiPathInput;
    [SerializeField] private InputActionReference _dragSelect;
    [SerializeField] private InputActionReference _selectEntityInput;
    [SerializeField] private InputActionReference _moveEntityInput;
    [SerializeField] private InputActionReference _mapModInput;
    [SerializeField] private InputActionReference _pauseInput;

    [SerializeField] private Texture2D _buildingCursor;
    [SerializeField] private Texture2D _deplacementCursor;
    [SerializeField] private Texture2D _attackCursor;

    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private MapMod _mapMod;

    [SerializeField] string _ennemieTag;
    [SerializeField] Canvas _pauseCanvas;
    [SerializeField] private BuildingPreWatching _buildingPreWatching;
    [SerializeField] private RectTransform _dragBox;
    [SerializeField] GameObject _listOfEntity;

    private bool _multiSelectionIsActive;
    private bool _multiPathIsActive;
    private bool _patrolOrder;
    private CapacityController _capacityController;
    private bool _capactityOrder;
    private Vector3 _dragCoord;
    private bool _dragging;
    private float _timeOfDragging;
    private float _timeToClick;
    private int _nbOfClick;
    private bool _doubleClick;
    private SelectManager _selectManager;
    private List<EntityController> _entitiesBackUp = new List<EntityController>();
    private bool _order;
    private bool _travelAttack;
    private bool _buildingOrder;
    private int _nbOfBuilding;
    private UiGestioneur _uiGestioneur;
    private CursorMode _cursorMode = CursorMode.Auto;
    private Vector2 _hotSpot = Vector2.zero;
    private float _delayToClick = 0.2f;

    void Start()
    {
        _selectManager = GetComponent<SelectManager>();

        _mapCamera.GetComponent<CameraControl>().DesactiveZoom();

        ActiveAllInput();
        _mapModInput.action.started += MapModActive;
        _pauseInput.action.started += SetPause;

        _uiGestioneur = FindObjectOfType<UiGestioneur>();

        _selectManager.SetEnnemieTag(_ennemieTag);
        _selectManager.SetAllieTag(gameObject.tag);
    }

    private void ActiveAllInput()
    {
        _selectEntityInput.action.performed += LeftClickGestion;
        _moveEntityInput.action.started += RightClickGestion;
        _multiSelectionInput.action.performed += ActiveMultiSelection;
        _multiSelectionInput.action.canceled += DesactiveMultiSelection;
        _multiPathInput.action.performed += ActiveMultiPath;
        _multiPathInput.action.canceled += DesactiveMultiPath;
        _dragSelect.action.started += StartDragSelect;
        _dragSelect.action.canceled += EndDragSelect;
    }

    private void DesactiveAllInput()
    {
        _selectEntityInput.action.performed -= LeftClickGestion;
        _moveEntityInput.action.started -= RightClickGestion;
        _multiSelectionInput.action.performed -= ActiveMultiSelection;
        _multiSelectionInput.action.canceled -= ActiveMultiSelection;
        _multiPathInput.action.performed -= ActiveMultiPath;
        _multiPathInput.action.canceled -= ActiveMultiPath;
        _dragSelect.action.started -= StartDragSelect;
        _dragSelect.action.canceled -= EndDragSelect;
    }

    private void ActiveMultiSelection(InputAction.CallbackContext obj)
    {
        _multiSelectionIsActive = true;
        _uiGestioneur.SetMulitSelection(_multiSelectionIsActive);
    }

    private void DesactiveMultiSelection(InputAction.CallbackContext obj)
    {
        _multiSelectionIsActive = false;
        _uiGestioneur.SetMulitSelection(_multiSelectionIsActive);
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
            _selectEntityInput.action.started += TeleporteOnMap;
            _uiGestioneur.gameObject.SetActive(false);
            DesactiveAllInput();
            _selectManager.ClearList();
            _uiGestioneur.DesactiveUi();
        }
        else
        {
            _selectEntityInput.action.started -= TeleporteOnMap;
            _uiGestioneur.gameObject.SetActive(true);
            ActiveAllInput();
        }
    }

    private void TeleporteOnMap(InputAction.CallbackContext obj)
    {
        _mapMod.TeleporteMainCamera(RayCast.DoARayCastFromMouse(_mapCamera).point);
        _selectEntityInput.action.started -= TeleporteOnMap;
        _uiGestioneur.gameObject.SetActive(true);
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
    private void ActiveMultiPath(InputAction.CallbackContext obj) { _multiPathIsActive = true; }
    private void OnDestroy()
    {
        DesactiveAllInput();
        _pauseInput.action.started -= SetPause;
        _mapModInput.action.started -= MapModActive;
    }
    private void Update()
    {
        if(_doubleClick)
        {
            _timeToClick += Time.deltaTime;
            if (_timeToClick >= _delayToClick)
            {
                _timeToClick = 0;
                _doubleClick = false;
                _nbOfClick = 0;
            }
        }
        if (_dragging)
        {
            _timeOfDragging += Time.deltaTime;

            float longueur = Input.mousePosition.x - _dragCoord.x;
            float largeur = Input.mousePosition.y - _dragCoord.y;

            _dragBox.anchoredPosition = new Vector2(_dragCoord.x + longueur / 2, _dragCoord.y + largeur / 2);
            _dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        }
    }
    private void LeftClickGestion(InputAction.CallbackContext context)
    {
        _nbOfClick += 1;
        if(_nbOfClick == 1){ _doubleClick = true; }

        if(_nbOfClick == 1 )
        {
            Physics.SyncTransforms();
            RaycastHit hit = RayCast.DoARayCastFromMouse(_camera);

            if (_buildingOrder)
            {
                IsMultipathActive();
                _selectManager.DoABuild(_nbOfBuilding, hit);
                _buildingPreWatching.BuildingIsCancel();
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
            else { DoASelection(hit); }
        }
        
        if(_nbOfClick == 2)
        {
            Debug.Log(_timeToClick);
            RaycastHit hit = RayCast.DoARayCastFromMouse(_camera);
            SelectableManager selectableTarget = hit.transform.gameObject.GetComponent<SelectableManager>();
            if (selectableTarget)
            {
                DoubleClick(selectableTarget);
            }
        }
    }

    private void DoASelection(RaycastHit hit)
    {
        List<RaycastResult> listOfUIRay = RayCast.DoUiRayCastFromMouse();
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
                    if (_multiSelectionIsActive && _selectManager.getSelectList().Count > 0)
                    {
                        if (_selectManager.getSelectList().Count < 2)
                        {
                            _uiGestioneur.AddOnGroupUi(_selectManager.getSelectList()[0].GetComponent<SelectableManager>());
                        }
                        _uiGestioneur.AddOnGroupUi(hit.transform.GetComponent<SelectableManager>());
                    }
                    else
                    {
                        _uiGestioneur.ActualiseUi(hit.transform.gameObject.GetComponent<SelectableManager>());
                    }
                    _selectManager.AddSelect(hit.transform.gameObject.GetComponent<SelectableManager>());
                }
                else
                {
                    if (!_multiSelectionIsActive)
                    {
                        _selectManager.ClearList();
                        _uiGestioneur.DesactiveUi();
                    }
                }
            }
            else
            {
                if (!_multiSelectionIsActive)
                {
                    _selectManager.ClearList();
                    _uiGestioneur.DesactiveUi();
                }
            }
        }
        else
        {
            foreach (RaycastResult raycastResult in listOfUIRay)
            {
                CadreController cadre = raycastResult.gameObject.GetComponent<CadreController>();
                if (cadre)
                {
                    ResetUiOrder();
                    _selectManager.ClearList();

                    _uiGestioneur.ActualiseUi(cadre.GetEntity());
                    _selectManager.AddSelect(cadre.GetEntity().GetComponent<SelectableManager>());
                }
            }
        }
    }

    private void RightClickGestion(InputAction.CallbackContext context)
    {
        List<RaycastResult> listOfUIRay = RayCast.DoUiRayCastFromMouse();
        if (listOfUIRay.Count == 0)
        {
            if (!_order && !_patrolOrder && !_travelAttack && !_buildingOrder)
            {
                IsMultipathActive();
                RaycastHit hit = RayCast.DoARayCastFromMouse(_camera);

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

        Cursor.SetCursor(null, _hotSpot, _cursorMode);
        _buildingPreWatching.BuildingIsCancel();
    }
    private void StartDragSelect(InputAction.CallbackContext obj)
    {
        _entitiesBackUp = _selectManager.getSelectList();
        _dragCoord = Input.mousePosition;
        _dragBox.GameObject().SetActive(true);
        _dragging = true;
        _timeOfDragging = 0;
    }

    private void DoubleClick(SelectableManager selectableManager)
    {
        StartCoroutine(IsOnScreen(selectableManager));

    }
    private void EndDragSelect(InputAction.CallbackContext obj)
    {
        if (_timeOfDragging > 0.15)
        {
            StartCoroutine(IsOnDragBox());
            if (_entitiesBackUp.Count > 0)
            {
                int w = 0;
                while (w < _entitiesBackUp.Count)
                {
                    EntityController i = _entitiesBackUp[w];
                    SelectableManager selectableI = i.gameObject.GetComponent<SelectableManager>();

                    if (i && !_selectManager.getSelectList().Contains(i)) { _selectManager.AddSelect(selectableI); }
                    if (!_uiGestioneur.groupUi._listOfEntity.Contains(selectableI)) { _uiGestioneur.AddOnGroupUi(selectableI); }

                    w++;
                }
            }
        }
        _dragBox.anchoredPosition = new Vector2(0, 0);
        _dragBox.sizeDelta = new Vector2(0, 0);

        _dragBox.GameObject().SetActive(false);
        _dragging = false;

    }

    IEnumerator IsOnScreen(SelectableManager selectableObject)
    {
        yield return new WaitForEndOfFrame();
        float longueur = Screen.width;
        float largeur = Screen.height;

        Bounds bounds = new Bounds(new Vector2(0, 0), new Vector2(longueur*2, largeur*2));

        foreach (EntityController i in _listOfEntity.GetComponentsInChildren<EntityController>())
        {
            Vector3 point = _camera.WorldToScreenPoint(i.transform.position);

            SelectableManager selectableI = i.gameObject.GetComponent<SelectableManager>();
            if (UnitInDragBox(point, bounds) && i.CompareTag(gameObject.tag) && selectableI.entityType == selectableObject.entityType)
            {
                
                if (!_selectManager.getSelectList().Contains(i)) { _selectManager.AddSelect(selectableI); }
                if (!_uiGestioneur.groupUi._listOfEntity.Contains(i.gameObject.GetComponent<SelectableManager>())) { _uiGestioneur.AddOnGroupUi(selectableI); }
            }
        }

        if (_selectManager._groupManager.getNumberOnGroup() == 1)
        {
            _uiGestioneur.ActualiseUi(_selectManager._groupManager.getSelectList()[0].gameObject.GetComponent<SelectableManager>());
        }
    }
    IEnumerator IsOnDragBox()
    {
        yield return new WaitForEndOfFrame();

        float longueur = Input.mousePosition.x - _dragCoord.x;
        float largeur = Input.mousePosition.y - _dragCoord.y;

        _dragBox.anchoredPosition = new Vector2(_dragCoord.x, _dragCoord.y) + new Vector2(longueur / 2, largeur / 2);
        _dragBox.sizeDelta = new Vector2(Mathf.Abs(longueur), Mathf.Abs(largeur));
        Bounds bounds = new Bounds(_dragBox.anchoredPosition, _dragBox.sizeDelta);

        foreach (EntityController i in _listOfEntity.GetComponentsInChildren<EntityController>())
        {
            Vector3 point = _camera.WorldToScreenPoint(i.transform.position);

            if (UnitInDragBox(point, bounds) && i.CompareTag(gameObject.tag))
            {
                SelectableManager selectableI = i.gameObject.GetComponent<SelectableManager>();
                if (!_selectManager.getSelectList().Contains(i)) { _selectManager.AddSelect(selectableI); }
                if (!_uiGestioneur.groupUi._listOfEntity.Contains(i.gameObject.GetComponent<SelectableManager>())) { _uiGestioneur.AddOnGroupUi(selectableI); }
            }
        }

        if (_selectManager._groupManager.getNumberOnGroup() == 1)
        {
            _uiGestioneur.ActualiseUi(_selectManager._groupManager.getSelectList()[0].gameObject.GetComponent<SelectableManager>());
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
        Cursor.SetCursor(_deplacementCursor, _hotSpot, _cursorMode);
    }
    public void CapacityOrder(CapacityController capacity)
    {
        if (capacity.ready)
        {
            ResetUiOrder();

            _capactityOrder = true;
            _capacityController = capacity;
            Cursor.SetCursor(_deplacementCursor, _hotSpot, _cursorMode);
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
        Cursor.SetCursor(_attackCursor, _hotSpot, _cursorMode);
    }

    public void DoABuilding(int nb, GameObject building)
    {
        _buildingOrder = true;
        _nbOfBuilding = nb;

        Cursor.SetCursor(_buildingCursor, _hotSpot, _cursorMode);
        SelectableManager buildingSelectable = building.GetComponent<SelectableManager>();
        if (buildingSelectable.CurrentShape)
        {
            _buildingPreWatching.SetBuilding(buildingSelectable.CurrentShape, buildingSelectable.CurrentShapeTransform);
        }
    }
}
