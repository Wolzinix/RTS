using UnityEngine;

public class ButtonForEntity : ButtonOverlap
{
    [SerializeField] EntityManager _EntityManager;
    public void SetEntity(EntityManager EntityManager)
    {
        _EntityManager = EntityManager;
        imageForOverlay = _EntityManager.GetSprit();

        if (!fenetre) { FalseStart(); }
        fenetre.GetComponent<OverlayRemplissage>().ActualiseOverlay(_EntityManager);
    }

    protected override void Actualisation()
    {
        fenetre.GetComponent<OverlayRemplissage>().ActualiseOverlay(_EntityManager);
    }
}
