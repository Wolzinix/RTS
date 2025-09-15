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

    private void ActualiseEveryIcon()
    {
        foreach (GameObject icon in IconObjectList)
        {
            int index = IconObjectList.IndexOf(icon);
            ActualiseOneIcon(icon,index);
        }
    }

    private void ActualiseFromIcon(int index)
    {
        for(int i = index;  i < IconObjectList.Count; i++)
        {
            ActualiseOneIcon(IconObjectList[i], index);
        }
    }

    private void ActualiseOneIcon(GameObject icon, int index)
    {
        Rect rectIcon = icon.GetComponent<RectTransform>().rect;
        icon.transform.localPosition = new Vector3(
                (int)(rectIcon.width * index),
                0,
                0);
    }

    public void AddEffect(StateEffect effect)
    {
        GameObject icon = Instantiate(iconPrefab, transform);
        IconObjectList.Add(icon);
        icon.GetComponent<IconEffect>().SetEffect(effect);
        ActualiseOneIcon(icon, IconObjectList.Count-1);
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
                ActualiseFromIcon(i);
                break;
            }
        }
    }
}
