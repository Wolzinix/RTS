using UnityEngine;
using UnityEngine.UI;

public class ButtonOverlap : Button
{
    [SerializeField] GameObject prefabFenetre;
    bool fenetreIsCreate;
    GameObject fenetre;
    [SerializeField] Image imageForAbility;
    protected override void Start()
    {
        base.Start();

        Vector3 coord = transform.position;
        coord += new Vector3(25, 75, 0);
        fenetre = Instantiate(prefabFenetre);

        Sprite image = imageForAbility.sprite;
        fenetre.GetComponent<OverlayRemplissage>().SetUpOverlay(coord, "lalalalalala", image);
        fenetre.SetActive(false);
    }
    void LateUpdate()
    {
        if (IsHighlighted() && !fenetreIsCreate) 
        {
            fenetreIsCreate = true;
            fenetre.SetActive(true);
            Sprite image = imageForAbility.sprite;
            fenetre.GetComponent<OverlayRemplissage>().ActualiseOverlay("lalalalalala", image);
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
}
