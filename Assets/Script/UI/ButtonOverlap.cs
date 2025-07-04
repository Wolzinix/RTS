using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOverlap : Button
{
    [SerializeField] GameObject prefabFenetre;
    bool fenetreIsCreate;
    GameObject fenetre;
    [SerializeField] Image imageForAbility;
    Image imageOfButton;
    TMP_Text _texte;
    CapacityController _capacity;
    public bool isActivable;
    protected override void Start()
    {
        base.Start();
        imageOfButton = GetComponent<Image>();
        if (!_texte ||!fenetre)
        {
            FalseStart();
        }
    }

    public void SetCapacity(CapacityController capacity)
    {
        _capacity = capacity;
        imageForAbility.sprite = _capacity.sprite;

        if(!_texte || !fenetre) { FalseStart(); }
        _texte.text = capacity.Name;
        fenetre.GetComponent<OverlayRemplissage>().ActualiseOverlay(_capacity);
    }

    private void FalseStart()
    {
        _texte = GetComponentInChildren<TMP_Text>();
        Vector3 coord = transform.position;
        coord += new Vector3(-25, 75, 0);
        fenetre = Instantiate(prefabFenetre);

        Sprite image = imageForAbility.sprite;
        fenetre.GetComponent<OverlayRemplissage>().SetUpOverlay(coord, "lalalalalala", image);
        fenetre.SetActive(false);
    }
    void LateUpdate()
    {
        if(isActivable) { imageOfButton.enabled = !imageOfButton.enabled; }
        
        if (IsHighlighted() && !fenetreIsCreate) 
        {
            fenetreIsCreate = true;
            fenetre.SetActive(true);
            fenetre.GetComponent<OverlayRemplissage>().ActualiseOverlay(_capacity);
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

}
