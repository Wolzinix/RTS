
using UnityEngine;
using UnityEngine.UI;

public class ButtonOverlap : Button
{
    [SerializeField] GameObject prefabFenetre;
    protected bool fenetreIsCreate;
    protected GameObject fenetre;
    protected Sprite imageForOverlay;
    protected Image imageOfButton;
    public bool isActivable;
    protected override void Start()
    {
        base.Start();
        imageOfButton = GetComponent<Image>();
        if (!fenetre)
        {
            FalseStart();
        }
    }
    protected void FalseStart()
    {
        Vector3 coord = transform.position;
        coord += new Vector3(-25, 75, 0);
        fenetre = Instantiate(prefabFenetre);

        Sprite image = imageForOverlay;
        fenetre.GetComponent<OverlayRemplissage>().SetUpOverlay(coord, "lalalalalala", image);
        fenetre.SetActive(false);
    }
    protected virtual void LateUpdate()
    {
        if(isActivable) { imageOfButton.enabled = !imageOfButton.enabled; }
        
        if (IsHighlighted() && !fenetreIsCreate) 
        {
            fenetreIsCreate = true;
            fenetre.SetActive(true);
            Actualisation();
        }
        else
        {
            if(fenetre && !IsHighlighted())
            {
                fenetreIsCreate = false;
                fenetre.SetActive(false);
            }
        }
    }
    public void Clignote(ActivableCapacity activable)
    {
        isActivable = activable.actif;
    }

    protected virtual void Actualisation(){}
}
