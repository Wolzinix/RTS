using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _buildings;

    private int _nb = -1;
    private Vector3 _spawnPosition = Vector3.zero;
    
    public List<GameObject> getBuildings()
    {
        return _buildings;
    }

    private void Start()
    {
        gameObject.GetComponent<EntityController>().resetEvent.AddListener(ResetBuildingOrder);
    }
    public void DoAbuild(int nb, RaycastHit hit)
    {
        _nb = nb;
        gameObject.GetComponent<EntityController>().AddPath(hit.point);
        _spawnPosition = hit.point;

    }

    private void Update()
    {
        if(_nb != -1)
        {

            bool location = gameObject.GetComponent<NavMeshController>().isStillOnTrajet();
            bool distance = Vector3.Distance(gameObject.transform.position, _spawnPosition) <= gameObject.GetComponent<NavMeshController>().HaveStoppingDistance() + 0.5;
            if (location && distance)
            {
                Build();
            }

        }
    }

    private void Build()
    {
        Collider[] colliders = DoAOverlap(_spawnPosition);

        if (colliders.Length == 1 ||( colliders.Length == 2 && (colliders[1] == gameObject || colliders[0] == gameObject)))
        {
            EntityManager gm = Instantiate(_buildings[_nb], _spawnPosition + new Vector3(0,2,0), transform.rotation).GetComponent<EntityManager>();
            gm.gameObject.tag = gameObject.tag;
            gm.ActualiseSprite();
        }

        _nb = -1;
        _spawnPosition = Vector3.zero;
    }

    private void ResetBuildingOrder()
    {
        _nb = -1;
        _spawnPosition = Vector3.zero;
    }
    private Collider[] DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1);
    }
}
