using UnityEngine;

public abstract class UiAppeirBase : MonoBehaviour
{
    public abstract void AppearUI();
    public abstract void DisappearUI();
    public abstract void AppearUI(bool IsPlayer);

}
