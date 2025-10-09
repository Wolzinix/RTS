using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Assets.Script.Tools
{
    public static class RayCast 
    {
        public static Vector3 RaycastForGround(GameObject transformReturn, Vector3 pos, float sizeY = Mathf.Infinity)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, sizeY,1<<3))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                return hit.point;
            }
            return transformReturn.transform.position;
        }

        public static Vector3 RaycastForGround(GameObject transformReturn, Vector3 pos, LayerMask layerMask , float sizeY = Mathf.Infinity)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, sizeY, layerMask))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                return hit.point;
            }
            return transformReturn.transform.position;
        }

        public static Vector3 RaycastForGround(Vector3 pos, LayerMask layerMask, float sizeY = Mathf.Infinity)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, sizeY, layerMask))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                return hit.point;
            }
            return Vector3.zero;
        }

        public static Vector3 RaycastForGround(Vector3 pos, float sizeY = Mathf.Infinity)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, sizeY, 1 << 3))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                return hit.point;
            }
            return Vector3.zero;
        }

        public static RaycastHit DoARayCastFromMouse(Camera camera )
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
            return hit;
        }

        public static RaycastHit DoARayCastFromMouse()
        {
            Camera camera = Camera.main;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
            return hit;
        }

        public static RaycastHit DoARayCastToGroundFromMouse(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 3, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
            return hit;
        }

        public static RaycastHit DoARayCastToGroundFromMouse()
        {
            Camera camera = Camera.main;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 3, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
            return hit;
        }

        public static List<RaycastResult> DoUIRayCastFromMouse()
        {
            PointerEventData eventData = new(EventSystem.current);
            List<RaycastResult> results = new();
            eventData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(eventData, results);

            return results;
        }

        public static Vector3 RaycastOnNavMeshGround(Vector3 pos, float sizeY = Mathf.Infinity)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, sizeY, 1 << 3))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                if (hit.collider.gameObject.GetComponent<NavMeshSurface>())
                {
                    if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 0.2f, NavMesh.AllAreas))
                    {
                        return new Vector3(hit.point.x, hit.point.y + (pos.y - hit.point.y), hit.point.z);
                    }
                }
            }
            return Vector3.zero;
        }

        public static List<RaycastHit> DoCircleRaycast(GameObject go, float range =50 , float numberOfRay = 40)
        {
            float delta = 360 / numberOfRay;

            List<RaycastHit> listOfGameObejct = new ();

            for (int i = 0; i < numberOfRay; i++)
            {
                Vector3 dir = Quaternion.Euler(0, i * delta, 0) * go.transform.forward;

                Ray ray = new (go.transform.position, dir);

                listOfGameObejct.Union(Physics.RaycastAll(ray, range));
            }

            return listOfGameObejct;
        }


        public static Collider[] DoASphereOverlap(Vector3 spawnPosition, LayerMask _excludeLayer)
        {
            return Physics.OverlapSphere(spawnPosition, 1, ~_excludeLayer, QueryTriggerInteraction.Ignore);
        }

        public static Collider[] DoASphereOverlap(Vector3 spawnPosition, float distance)
        {
            return Physics.OverlapSphere(spawnPosition, distance);
        }

        public static int DoASphereOverlap(Vector3 spawnPosition, float distance, LayerMask layermask)
        {
            return Physics.OverlapSphere(spawnPosition, distance, layermask).Length;
        }



        public static Vector3 RayToTuchGroundWithMapLayer(Vector3 pos, float maxdistance, LayerMask layerMask, List<TerrainLayer> terrainLayer)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, maxdistance, layerMask))
            {
                Terrain terrain = hit.collider.gameObject.GetComponent<Terrain>();
                if (terrain || hit.collider.gameObject.GetComponent<NavMeshSurface>())
                {
                    if (terrain)
                    {
                        float[,,] splatmap = terrain.terrainData.GetAlphamaps(
                            Mathf.FloorToInt((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x * terrain.terrainData.alphamapWidth),
                            Mathf.FloorToInt((pos.z - terrain.transform.position.z) / terrain.terrainData.size.z * terrain.terrainData.alphamapHeight),
                            1,
                            1
                        );
                        float Visible = 0;
                        int texindex = 0;
                        for (int i = 0; i < splatmap.GetLength(2); i++)
                        {
                            if (splatmap[0, 0, i] > Visible)
                            {
                                Visible = splatmap[0, 0, i];
                                texindex = i;
                            }
                        }

                        if (terrainLayer.Contains(terrain.terrainData.terrainLayers[texindex])) { return new Vector3(pos.x, hit.point.y, pos.z); }
                    }
                    else { return new Vector3(pos.x, hit.point.y, pos.z); }
                }
            }
            return Vector3.zero;
        }
    }
}