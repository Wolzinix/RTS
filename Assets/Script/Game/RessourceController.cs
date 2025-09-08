using UnityEngine;
using UnityEngine.Events;

public class RessourceController : MonoBehaviour
{
    [SerializeField] private int _gold;
    [SerializeField] private int _wood;

    [HideInInspector] public UnityEvent ressourcesAdd = new UnityEvent();

    [HideInInspector] public UnityEvent<int, int> ressourcesAddUI = new UnityEvent<int,int>();
    private ControlManager _controlManager;

    void Start()
    {
        _controlManager = GetComponent<ControlManager>();
        if (_controlManager)
        {
            ressourcesAddUI.Invoke(_gold, _wood);
        }
    }

    public void AddGold(int gold)
    {
        _gold += gold;
        if (_controlManager)
        {
            ressourcesAddUI.Invoke(gold, 0);
        }
        else { ressourcesAdd.Invoke();}
    }

    public void AddWood(int wood)
    {
        _wood += wood;

        if (_controlManager)
        {
            ressourcesAddUI.Invoke(0, wood);

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

    public int GetWood() { return _wood; }
    public int GetGold() { return _gold; }

    public bool CompareRessource(int wood , int gold)
    {
        return _wood >= wood && _gold >= gold;
    }
}
