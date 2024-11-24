using UnityEngine;

public  abstract class StateEffect : MonoBehaviour
{
    protected SelectableManager entityAffected;
    protected float duration;
    protected float actualTime;
    virtual protected void Start() 
    {
    }

    virtual protected void Update() {}

    virtual protected void end() { }
}
