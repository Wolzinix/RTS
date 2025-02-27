using System;
using System.Collections.Generic;
using UnityEngine;

public class MapMod : MonoBehaviour
{
    public bool _isMapMod;

    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private List<GameObject> _mapObjects;

    public void MapModActive()
    {
        _isMapMod = !_isMapMod;

        CameraGestion();
        SelectGestionMapMod();
        ConnectToEventNewEtentity();
    }

    public void SetMainCamera(Camera camera) { _camera = camera; }
    public void SetMapCamera(Camera camera) { _mapCamera = camera; }
    public void SetMapObject(List<GameObject> list) { _mapObjects = list; }
    private void CameraGestion()
    {
        _camera.enabled = !_camera.enabled;
        CameraControl cameraControl = _camera.GetComponent<CameraControl>();
        cameraControl.StopMoving();
        cameraControl.gameObject.SetActive(_camera.enabled);

        _mapCamera.enabled = !_mapCamera.enabled;
        _mapCamera.GetComponent<CameraControl>().StopMoving();
        _mapCamera.gameObject.SetActive(_mapCamera.enabled);

        if (_isMapMod) { cameraControl.DesactiveZoom();}
        else { cameraControl.ActiveZoom(); }
    }

    private void SelectGestionMapMod()
    {
        foreach (GameObject w in _mapObjects)
        {
            foreach (SelectableManager i in w.GetComponentsInChildren<SelectableManager>()){ ActualiseOneUnit(i);}
        }
    }

    public void ActualiseOneUnit(SelectableManager entity)
    {
        if (_isMapMod)
        {
            if (entity.transform.GetComponentInChildren<SkinnedMeshRenderer>() && entity.transform.GetComponentInChildren<SkinnedMeshRenderer>().enabled ||
               entity.transform.GetComponentInChildren<MeshRenderer>() && entity.transform.GetComponentInChildren<MeshRenderer>().enabled)
            {
                entity.OnSelected();
                return;
            }
        }
        entity.OnDeselected();
    }

    private void ConnectToEventNewEtentity()
    {
        foreach (GameObject w in _mapObjects)
        {
            foreach (ProductBuildingController i in w.GetComponentsInChildren<ProductBuildingController>())
            {
                if (_isMapMod) { i.entitySpawnNow.AddListener(SelectGestionMapMod); }
                else { i.entitySpawnNow.RemoveListener(SelectGestionMapMod); }
            }
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
