
using UnityEngine.InputSystem;
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
        cameraControl.enabled = _camera.enabled;
        cameraControl.StopMoving();

        _mapCamera.enabled = !_mapCamera.enabled;
        _mapCamera.GetComponent<CameraControl>().enabled = _mapCamera.enabled;
        _mapCamera.GetComponent<CameraControl>().StopMoving();

        if (_isMapMod)
        {
            cameraControl.DesactiveZoom();
            
        }
        else { cameraControl.ActiveZoom(); }
    }

    private void SelectGestionMapMod()
    {
        foreach (EntityManager i in Resources.FindObjectsOfTypeAll<EntityManager>())
        {
            if (_isMapMod) { i.OnSelected(); }
            else { i.OnDeselected(); }
        }

    }

    private void ConnectToEventNewEtentity()
    {
        foreach (BuildingController i in Resources.FindObjectsOfTypeAll<BuildingController>())
        {
            if (_isMapMod) { i.entitySpawnNow.AddListener(SelectGestionMapMod); }
            else { i.entitySpawnNow.RemoveListener(SelectGestionMapMod); }
        }
    }
}
