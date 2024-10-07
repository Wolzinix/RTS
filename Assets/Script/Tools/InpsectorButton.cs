using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RessourceSpawning))]
public class InspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        RessourceSpawning myTarget = (RessourceSpawning)target;

        if (GUILayout.Button("Build Object"))
        {
            myTarget.DoA();
        }

        if (GUILayout.Button("Accept Object"))
        {
            myTarget.ClearList();
        }
    }
}
