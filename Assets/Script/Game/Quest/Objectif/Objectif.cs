using UnityEngine;
using UnityEngine.Events;

public abstract class Objectif : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Objectif> succes;
    [HideInInspector] public UnityEvent<Objectif> failled;
}
