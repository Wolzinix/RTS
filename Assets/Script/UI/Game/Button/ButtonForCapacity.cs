using UnityEngine.UI;

public class ButtonForCapacity : ButtonOverlap
{
    protected CapacityController _capacity;
    private Image imageOfCapacity;
    public bool isActivable;

    protected override void Start()
    {
        base.Start();
        imageOfCapacity = GetComponentsInChildren<Image>()[1];
    }
    public void SetCapacity(CapacityController capacity)
    {
        _capacity = capacity;
        if(!imageOfCapacity)
        {
            imageOfCapacity = GetComponentsInChildren<Image>()[1];
        }
        imageOfCapacity.sprite = _capacity.sprite;
    }
    public CapacityController GetCapacity()
    {
        return _capacity;
    }
    protected override void Actualisation()
    {
        base.Actualisation();
        fenetre.ActualiseOverlay(_capacity);
    }

    protected override void  LateUpdate()
    {

        if (isActivable) { imageOfCapacity.enabled = !imageOfCapacity.enabled; }
        if(!isActivable && !imageOfCapacity.enabled) { imageOfCapacity.enabled = true; }
        base.LateUpdate();
    }

    public void Clignote(ActivableCapacity activable)
    {
        isActivable = activable.actif;
    }
}
