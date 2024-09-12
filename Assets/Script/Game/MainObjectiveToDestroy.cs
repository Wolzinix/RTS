using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjectiveToDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        Application.Quit();
    }
}
