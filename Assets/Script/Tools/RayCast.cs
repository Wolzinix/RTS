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
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, sizeY,1<<3))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                return hit.point;
            }
            return transformReturn.transform.position;
        }

        public static Vector3 RaycastForGround(GameObject transformReturn, Vector3 pos, LayerMask layerMask , float sizeY = Mathf.Infinity)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, sizeY, layerMask))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                return hit.point;
            }
            return transformReturn.transform.position;
        }

        public static Vector3 RaycastForGround(Vector3 pos, LayerMask layerMask, float sizeY = Mathf.Infinity)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, sizeY, layerMask))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                return hit.point;
            }
            return Vector3.zero;
        }

        public static Vector3 RaycastForGround(Vector3 pos, float sizeY = Mathf.Infinity)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, sizeY, 1 << 3))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                return hit.point;
            }
            return Vector3.zero;
        }

        public static RaycastHit DoARayCastFromMouse(Camera camera )
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
            return hit;
        }

        public static RaycastHit DoARayCastFromMouse()
        {
            Camera camera = Camera.main;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
            return hit;
        }

        public static RaycastHit DoARayCastToGroundFromMouse(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 3, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
            return hit;
        }

        public static RaycastHit DoARayCastToGroundFromMouse()
        {
            Camera camera = Camera.main;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 3, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
            return hit;
        }

        public static List<RaycastResult> DoUiRayCastFromMouse()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();
            eventData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(eventData, results);

            return results;
        }

        public static Vector3 RaycastForGroundNavMesh(Vector3 pos, float sizeY = Mathf.Infinity)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, sizeY, 1 << 3))
            {
                Debug.DrawLine(pos, hit.point, Color.red, 10f);
                if (hit.collider.gameObject.GetComponent<NavMeshSurface>())
                {
                    NavMeshHit navHit = new NavMeshHit();
                    if (NavMesh.SamplePosition(hit.point, out navHit, 0.2f, NavMesh.AllAreas))
                    {
                        return new Vector3(hit.point.x, hit.point.y + (pos.y - hit.point.y), hit.point.z);
                    }
                }
            }
            return Vector3.zero;
        }

        public static List<RaycastHit> DoCircleRaycast(GameObject go, float range, float numberOfRay = 40)
        {
            ;
            float delta = 360 / numberOfRay;

            List<RaycastHit> listOfGameObejct = new List<RaycastHit>();

            for (int i = 0; i < numberOfRay; i++)
            {
                Vector3 dir = Quaternion.Euler(0, i * delta, 0) * go.transform.forward;

                Ray ray = new Ray(go.transform.position, dir);

                listOfGameObejct.Union(Physics.RaycastAll(ray, range));
            }

            return listOfGameObejct;
        }
    }
}