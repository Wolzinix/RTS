using UnityEngine;
using UnityEngine.Events;

public class RessourceController : MonoBehaviour
{

    [SerializeField] private int _gold;
    [SerializeField] private int _wood;

    RessourceUi _ui;
    [HideInInspector] public UnityEvent ressourcesAdd = new UnityEvent();

    private ControlManager _controlManager;

    void Start()
    {
        _controlManager = GetComponent<ControlManager>();
        if (_controlManager)
        {
            _ui = FindAnyObjectByType<RessourceUi>();
            _ui.AddWood(_wood);
            _ui.AddGold(_gold);
        }

    }

    public void AddGold(int gold)
    {
        _gold += gold;
        if (_controlManager)
        {
            _ui.AddGold(gold);
        }
        else { ressourcesAdd.Invoke();}
    }

    public void AddWood(int wood)
    {
        _wood += wood;
        if (_controlManager)
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
