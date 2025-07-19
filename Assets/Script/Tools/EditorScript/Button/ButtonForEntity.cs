using UnityEngine;

public class ButtonForEntity : ButtonOverlap
{
    [SerializeField] EntityManager _EntityManager;
    public void SetEntity(EntityManager EntityManager)
    {
        _EntityManager = EntityManager;
        image.sprite = _EntityManager.GetSprit();
    }

    protected override void Actualisation()
    {
        base.Actualisation();
        fenetre.ActualiseOverlay(_EntityManager);
    }
}
