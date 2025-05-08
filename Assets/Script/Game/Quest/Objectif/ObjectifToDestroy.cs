using UnityEngine;
public class ObjectifToDestroy : Objectif
{
    [SerializeField] bool NeedToBeProtect;
    private void OnDestroy()
    {
        if(NeedToBeProtect)
        {
            failled.Invoke(this);
        }
        else { succes.Invoke(this); }
       
    }
}
