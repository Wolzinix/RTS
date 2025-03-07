using UnityEngine;
using UnityEngine.UI;

public class ChangeIcon : MonoBehaviour
{
    [SerializeField] Image ImageToChange;
    [SerializeField] Sprite ImageOn;
    [SerializeField] Sprite ImageOff;

    public void ChangeImage()
    {
        if(ImageToChange.sprite == ImageOn)  { ImageToChange.sprite = ImageOff; }
        else { ImageToChange.sprite = ImageOn; }
    }

}
