using UnityEngine;

public class FogWarManager : MonoBehaviour
{
    void Start()
    {
        foreach (SelectableManager i in FindObjectsOfType<SelectableManager>())
        {
            if (i.gameObject.tag != gameObject.tag)
            {
                AddFromFog(i);
            }
        }
    }

    public void FogGestion(SelectableManager entity, bool hide)
    {
        if (entity.gameObject.tag != gameObject.tag)
        {
            if (hide) { AddFromFog(entity); }
            else { RemoveFromFog(entity); }
        }
    }

    private void RemoveFromFog(SelectableManager entity)
    {
        entity.GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    private void AddFromFog(SelectableManager entity)
    {
        entity.GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
