using UnityEngine;

public  abstract class StateEffect : MonoBehaviour
{
    protected SelectableManager entityAffected;
    virtual protected void Start() {}

    virtual protected void Update() {}

    virtual protected void end() { }
}
