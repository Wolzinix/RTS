using UnityEngine;
public class MapMod
{
    public bool _isMapMod;

    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _mapCamera;

    public void MapModActive()
    {
        _isMapMod = !_isMapMod;

        CameraGestion();
        SelectGestionMapMod();
        ConnectToEventNewEtentity();
    }

    public void SetMainCamera(Camera camera) { _camera = camera; }
    public void SetMapCamera(Camera camera) { _mapCamera = camera; }
    private void CameraGestion()
    {
        _camera.enabled = !_camera.enabled;
        CameraControl cameraControl = _camera.GetComponent<CameraControl>();
        cameraControl.StopMoving();
        cameraControl.gameObject.SetActive(_camera.enabled);

        _mapCamera.enabled = !_mapCamera.enabled;
        _mapCamera.GetComponent<CameraControl>().StopMoving();
        _mapCamera.gameObject.SetActive(_mapCamera.enabled);

        if (_isMapMod)
        {
            cameraControl.DesactiveZoom();

        }
        else { cameraControl.ActiveZoom(); }
    }
    
    private void SelectGestionMapMod()
    {
        foreach (SelectableManager i in GameObject.FindObjectsOfType<SelectableManager>())
        {
            if (_isMapMod) { i.OnSelected(); }
            else { i.OnDeselected(); }
        }

    }

    private void ConnectToEventNewEtentity()
    {
        foreach (ProductBuildingController i in GameObject.FindObjectsOfType<ProductBuildingController>())
        {
            if (_isMapMod) { i.entitySpawnNow.AddListener(SelectGestionMapMod); }
            else { i.entitySpawnNow.RemoveListener(SelectGestionMapMod); }
        }
    }

    public void TeleporteMainCamera(Vector3 destination)
    {
        if (_mapCamera && _camera)
        {
            _camera.gameObject.transform.position = new Vector3(destination.x, _camera.gameObject.transform.position.y, destination.z);
            MapModActive();
        }
    }
}
