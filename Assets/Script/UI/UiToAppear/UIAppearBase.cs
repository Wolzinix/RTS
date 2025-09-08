using UnityEngine;

public abstract class UIAppearBase : MonoBehaviour
{
    public abstract void AppearUI();
    public abstract void DisappearUI();
    public abstract void AppearUI(bool IsPlayer);

}
