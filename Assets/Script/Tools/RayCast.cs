using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
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

        public static RaycastHit DoARayCastFromMouse(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) { return hit; }
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
    }
}