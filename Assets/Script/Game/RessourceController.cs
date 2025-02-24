using UnityEngine;
using UnityEngine.Events;

public class RessourceController : MonoBehaviour
{

    [SerializeField] private int _gold;
    [SerializeField] private int _wood;

    RessourceUi _ui;
    public UnityEvent ressourcesAdd = new UnityEvent();


    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<ControlManager>())
        {
            _ui = FindAnyObjectByType<RessourceUi>();
            _ui.AddWood(_wood);
            _ui.AddGold(_gold);
        }

    }

    public void AddGold(int gold)
    {
        _gold += gold;
        if (GetComponent<ControlManager>())
        {
            _ui.AddGold(gold);
        }
        else { ressourcesAdd.Invoke();}
    }

    public void AddWood(int wood)
    {
        _wood += wood;
        if (GetComponent<ControlManager>())
        {
            _ui.AddWood(wood);
           
        }
        else{  ressourcesAdd.Invoke();}
    }

    public bool CompareWood(int wood)
    {
        return _wood >= wood;
    }

    public bool CompareGold(int gold)
    {
        return _gold >= gold;
    }
}
