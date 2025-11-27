using UnityEngine;
using UnityEngine.InputSystem;

public abstract class RebindSaveLoad
{
    public static void Load(InputActionAsset actions)
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }

    public static void Save(InputActionAsset actions)
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
