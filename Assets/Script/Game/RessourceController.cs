using UnityEngine;
public class RessourceController : MonoBehaviour
{
    int _gold;
    int _wood;

    RessourceUi _ui;


    // Start is called before the first frame update
    void Start()
    {
        _ui = FindAnyObjectByType<RessourceUi>();
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
            _ui.AddWood(-wood);
        }
    }
}
