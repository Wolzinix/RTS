using UnityEngine;
using UnityEngine.UI;

public class ButtonOverlap : Button
{
    protected OverlayRemplissage fenetre;
    public bool stopFenetre;
    protected override void Start()
    {
        base.Start();
        fenetre = FindAnyObjectByType<OverlayRemplissage>(FindObjectsInactive.Include);
    }
    protected virtual void LateUpdate()
    {
        if (IsHighlighted() && !stopFenetre) 
        {
            fenetre.gameObject.SetActive(true);
            Actualisation();
            stopFenetre = true;
        }
        else if(!IsHighlighted() && stopFenetre)
        {
            fenetre.gameObject.SetActive(false);
            stopFenetre = false;
        }
    }
    protected virtual void Actualisation()
    {
        Vector3 coord = transform.position;
        coord += new Vector3(-25, GetComponent<RectTransform>().rect.height *2 , 0);

        fenetre.SetUpOverlay(coord);
    }
}
