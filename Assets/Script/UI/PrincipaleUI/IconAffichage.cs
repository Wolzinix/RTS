using System.Collections.Generic;
using UnityEngine;

public class IconAffichage : MonoBehaviour
{
    private List<GameObject> iconObjectList = new();

    [SerializeField] private GameObject iconPrefab;

    private void Start()
    {
    }

    public void AddEffect(StateEffect effect)
    {
        GameObject icon = Instantiate(iconPrefab,transform);
        iconObjectList.Add(icon);
        icon.GetComponent<IconEffect>().SetEffect(effect);
        ActualiseUI();
    }

    public void RemoveEffect()
    {
        int index = iconObjectList.FindIndex(x => x.GetComponent<IconEffect>()._effect == null);
        
        if (index > 0 && iconObjectList.Count > index)
        {
            iconObjectList.Remove(iconObjectList[index]);
        }
        ActualiseUI();
    }

    private void ActualiseUI()
    {
        foreach(GameObject icon in iconObjectList)
        {
            int index = iconObjectList.IndexOf(icon);
            Rect rectIcon = icon.GetComponent<RectTransform>().rect;

            icon.transform.position = new Vector3(
                    (int)(rectIcon.width * index),
                    0,
                    0);
        }
    }

    public void ClearEffect()
    {
        if(iconObjectList.Count > 0) 
        {
            foreach (GameObject i in iconObjectList)
            {
                Destroy(i);
            }
        }
        
        iconObjectList.Clear();
    }
}
