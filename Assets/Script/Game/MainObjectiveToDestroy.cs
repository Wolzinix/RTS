using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectiveToDestroy : MonoBehaviour
{
    [SerializeField] GameObject _UI;

    [SerializeField] GameObject _FinalScreen;

    private void OnDestroy()
    {
        _UI.SetActive(false);
        _FinalScreen.SetActive(true);
        Application.Quit();
    }
}
