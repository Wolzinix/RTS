public class ButtonForCapacity : ButtonOverlap
{
    protected CapacityController _capacity;
    protected override void Start()
    {
        base.Start();
    }
    public void SetCapacity(CapacityController capacity)
    {
        _capacity = capacity;
        imageForOverlay = _capacity.sprite;

        if (!fenetre) { FalseStart(); }
        fenetre.GetComponent<OverlayRemplissage>().ActualiseOverlay(_capacity);
    }

    protected override void Actualisation()
    {
        fenetre.GetComponent<OverlayRemplissage>().ActualiseOverlay(_capacity);
    }
}
