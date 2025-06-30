using UnityEngine;
using UnityEngine.UI;

public class ButtonOverlap : Button
{
    public  GameObject prefabFenetre;
    bool fenetreIsCreate;
    GameObject fenetre;
    protected override void Start()
    {
        base.Start();

        Vector3 coord = transform.position;
        coord += new Vector3(0, 25, 0);
        fenetre = Instantiate(prefabFenetre, coord, Quaternion.identity);
        fenetre.SetActive(false);
    }
    void LateUpdate()
    {
        if (IsHighlighted() && !fenetreIsCreate) 
        {
            fenetreIsCreate = true;
            fenetre.SetActive(true);
            Debug.Log("ahahahaah"); 
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
