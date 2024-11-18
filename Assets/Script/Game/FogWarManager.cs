using UnityEngine;

public class FogWarManager : MonoBehaviour
{
    void Start()
    {
        foreach(AggressifEntityManager i in Resources.FindObjectsOfTypeAll<AggressifEntityManager>() as AggressifEntityManager[])
        {
            if(i.gameObject.tag != gameObject.tag)
            {
                AddFromFog(i);
            }
        }
    }

    public void FogGestion(AggressifEntityManager entity, bool hide)
    {
        if (entity.gameObject.tag != gameObject.tag)
        {
            if (hide) { AddFromFog(entity); }
            else { RemoveFromFog(entity); }
        }
    }

    private void RemoveFromFog(AggressifEntityManager entity)
    {
        entity.GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    private void AddFromFog(AggressifEntityManager entity)
    {
        //entity.GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
