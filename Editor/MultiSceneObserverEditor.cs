using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(MultiSceneObserver))]
public class MultiSceneObserverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MultiSceneObserver myScript = (MultiSceneObserver)target;

        EditorGUILayout.Space(30);

        if (GUILayout.Button("Найти все элементы мульти сцены"))
        {
            List<MultiObject> multiObjects = FindObjectsOfType<MultiObject>().ToList();
            List<StatefulObject> objectsWitchState = FindObjectsOfType<StatefulObject>().ToList();
            myScript.SetMultiSceneObserver(multiObjects, objectsWitchState);
        }

        EditorUtility.SetDirty(myScript);
    }
}
