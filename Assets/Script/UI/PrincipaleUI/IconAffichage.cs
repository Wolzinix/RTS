using System.Collections.Generic;
using UnityEngine;

public class IconAffichage : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;

    private List<GameObject> IconObjectList = new();


    private void OnEnable()
    {
        ClearEffect();
    }
    private void OnDisable()
    {
        ClearEffect();
    }
    public void ClearEffect()
    {
        foreach (GameObject i in IconObjectList)
        {
            Destroy(i);
        }
        
        IconObjectList.Clear();
    }

    public void AddEffect(StateEffect effect)
    {
        GameObject icon = Instantiate(iconPrefab, transform);
        IconObjectList.Add(icon);
        icon.GetComponent<IconEffect>().SetEffect(effect);
    }

    public void RemoveEffect(StateEffect effect)
    {
        for (int i = IconObjectList.Count - 1; i >= 0; i--)
        {
            IconEffect iconeEffect = IconObjectList[i].GetComponent<IconEffect>();
            if (iconeEffect._effect == null || iconeEffect._effect == effect || iconeEffect._effect.IsFinish())
            {
                Destroy(IconObjectList[i]);
                IconObjectList.Remove(IconObjectList[i]);
                break;
            }
        }
    }
}
