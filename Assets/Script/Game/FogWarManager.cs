using UnityEngine;

public class FogWarManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(AggressifEntityManager i in Resources.FindObjectsOfTypeAll(typeof(AggressifEntityManager)) as AggressifEntityManager[])
        {
            if(i.gameObject.tag != gameObject.tag)
            {
                i.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
    }
}
