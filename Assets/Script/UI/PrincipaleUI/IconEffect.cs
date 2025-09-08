using UnityEngine;
using UnityEngine.UI;

public class IconEffect : MonoBehaviour
{
    public StateEffect _effect;
    private Image sprite;
    void Start()
    {
        sprite = GetComponent<Image>();
    }

    public void SetEffect(StateEffect effect)
    {
        _effect = effect;
        if(!sprite) { sprite = GetComponent<Image>(); }
        sprite.sprite = _effect.sprite;
    }
}
