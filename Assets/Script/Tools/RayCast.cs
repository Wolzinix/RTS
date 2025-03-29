using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Script.Tools
{
    public static class RayCast 
    {
        public static Vector3 RaycastForGround(Vector3 pos)
        {
            Ray ray = new Ray(pos, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<NavMeshSurface>())
                {
                    return hit.point;
                }
            }
            return Vector3.zero;
        }
    }
}