using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class BuilderManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _buildings;

    private List<int> ListOfBuildsIndex = new List<int>();
    private List<Vector3> ListOfBuildPosition = new List<Vector3>();
    public List<GameObject> getBuildings() { return _buildings; }

    private void Start()
    {
        gameObject.GetComponent<EntityController>().resetEvent.AddListener(ResetBuildingOrder);
    }
    public void DoAbuild(int nb, RaycastHit hit)
    {
        ListOfBuildsIndex.Add(nb);
        ListOfBuildPosition.Add(hit.point);
    }

    private void Update()
    {
        if(ListOfBuildsIndex.Count != 0)
        {

            bool location = gameObject.GetComponent<NavMeshController>().isStillOnTrajet();
            bool distance = Vector3.Distance(gameObject.transform.position, ListOfBuildPosition[0]) <= gameObject.GetComponent<NavMeshController>().HaveStoppingDistance() + 0.5;

            if (location && distance) { Build(); }
            else if(location && !distance)  {  gameObject.GetComponent<EntityController>().AddPath(ListOfBuildPosition[0]); }

        }
    }

    private void Build()
    {
        Collider[] colliders = DoAOverlap(ListOfBuildPosition[0]);

        if (colliders.Length == 1 ||( colliders.Length == 2 && (colliders[1] == gameObject || colliders[0] == gameObject)))
        {
            EntityManager gm = Instantiate(_buildings[ListOfBuildsIndex[0]], ListOfBuildPosition[0] + new Vector3(0,2,0), transform.rotation).GetComponent<EntityManager>();
            gm.gameObject.tag = gameObject.tag;
            gm.ActualiseSprite();
        }

        ListOfBuildPosition.RemoveAt(0);
        ListOfBuildsIndex.RemoveAt(0);
    }

    private void ResetBuildingOrder()
    {
        ListOfBuildPosition.Clear();
        ListOfBuildsIndex.Clear();
    }
    private Collider[] DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1);
    }
}
