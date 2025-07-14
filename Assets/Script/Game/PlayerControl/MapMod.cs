using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera),typeof(CameraControl))]
public class MapMod : MonoBehaviour
{
    public bool _isMapMod;

    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private List<GameObject> _mapObjects;
    private CameraControl cameraControl;

    private void Start()
    {
        cameraControl = _mapCamera.GetComponent<CameraControl>();
        cameraControl.SetCamera(_camera);
        cameraControl.ActiveZoom();
        _isMapMod = false;
    }
    public void MapModActive()
    {
        _isMapMod = !_isMapMod;

        CameraGestion();
        SelectGestionMapMod();
        ConnectToEventNewEtentity();
    }

    public Camera GetActiveCamera()
    {
        if (_isMapMod) { return _mapCamera; }
        else { return _camera; }
    }
    public void SetMainCamera(Camera camera) { _camera = camera; }
    public void SetMapCamera(Camera camera) { _mapCamera = camera; }
    public void SetMapObject(List<GameObject> list) { _mapObjects = list; }
    private void CameraGestion()
    {
        cameraControl.StopMoving();

        _camera.enabled = !_camera.enabled;
        _mapCamera.enabled = !_mapCamera.enabled;
        if (_camera.enabled) { cameraControl.SetCamera(_camera); }
        else { cameraControl.SetCamera(_mapCamera); }

        if (_isMapMod) { cameraControl.DesactiveZoom(); }
        else { cameraControl.ActiveZoom(); }
    }

    private void SelectGestionMapMod()
    {
        foreach(SelectableManager i in FindObjectsByType<SelectableManager>(FindObjectsSortMode.None))
        {
            StartCoroutine(ActualiseEntity(i));
        }
    }
    IEnumerator ActualiseEntity (SelectableManager entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        yield return null;

        ActualiseOneUnit(entity);
    }
    public void ActualiseOneUnit(SelectableManager entity)
    {
        if (_isMapMod)
        {
            if (entity.CurrentShape && 
                (entity.CurrentShape.GetComponentInChildren<SkinnedMeshRenderer>() && entity.CurrentShape.GetComponentInChildren<SkinnedMeshRenderer>().enabled ||
                 entity.CurrentShape.GetComponentInChildren<MeshRenderer>() && entity.CurrentShape.GetComponentInChildren<MeshRenderer>().enabled)
               )
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
