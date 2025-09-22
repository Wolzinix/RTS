using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : MonoBehaviour
{
    private int numberOfObjectifs;
    [SerializeField] private List<Objectif> objects;
    protected virtual void AllObjectifSucces() { }
    private void RemoveObjectifSucces(Objectif objectif)
    {
        numberOfObjectifs -= 1;
        objects.Remove(objectif);
        if(numberOfObjectifs <= 0) { AllObjectifSucces(); }
    }
    private void RemoveObjectifFailled(Objectif objectif)
    {
        numberOfObjectifs -= 1;
        objects.Remove(objectif);
        if (numberOfObjectifs <= 0) { AllObjectifSucces(); }
    }
    void Start()
    {
        numberOfObjectifs = objects.Count;
        foreach (Objectif i in objects)
        {
            i.succes.AddListener(RemoveObjectifSucces);
            i.failled.AddListener(RemoveObjectifFailled);
        }
    }
}
