using UnityEngine;
public class RessourceController : MonoBehaviour
{

    public int _gold = 2;
    public int _wood = 2;   

    RessourceUi _ui;


    // Start is called before the first frame update
    void Start()
    {
        _ui = FindAnyObjectByType<RessourceUi>();
        _ui.AddWood(_wood);
        _ui.AddGold(_gold);
    }

    public void AddGold(int gold)
    {
        _gold += gold;
        if(GetComponent<ControlManager>())
        {
            _ui.AddGold(gold);
        }
    }

    public void AddWood(int wood)
    {
        _wood += wood;
        if (GetComponent<ControlManager>())
        {
            _ui.AddWood(wood);
        }
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
