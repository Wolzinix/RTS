using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextChanger : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public void SetText(String newText)
    {
        text.enabled = true;
        text.text = newText;
        StartCoroutine(Disapear());
    }

    IEnumerator Disapear()
    {
        yield return new WaitForSeconds(2);
        text.enabled = false;
    }
}
