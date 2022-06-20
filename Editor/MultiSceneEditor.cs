using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(MultiScene))]
public class MultiSceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MultiScene multiScene = (MultiScene)target;

        EditorGUILayout.Space(30);

        if (GUILayout.Button("Seach all elements multi scene"))
        {
            List<MultiObject> multiObjects = FindObjectsOfType<MultiObject>(true).ToList();
            List<StatefulObject> objectsWitchState = FindObjectsOfType<StatefulObject>(true).ToList();
            multiScene.SetMultiSceneObserver(multiObjects, objectsWitchState);
        }

        if (GUILayout.Button("Bake multi objects"))
        {
            MultiSceneWindow.CreateFolderWithBackedObjects();
            CreateBakedMultiObjectData(multiScene);
        }

        if (GUILayout.Button("Disable multiobjects in Editor"))
        {
            for (int i = 0; i < multiScene.GetAllStaticMultiObject.Count; i++)
            {
                multiScene.GetAllStaticMultiObject[i].gameObject.SetActive(false);
                multiScene.GetAllStaticMultiObject[i].tag = "EditorOnly";
            }
        }

        if (GUILayout.Button("Enable multiobjects in Editor"))
        {
            for (int i = 0; i < multiScene.GetAllStaticMultiObject.Count; i++)
            {
                multiScene.GetAllStaticMultiObject[i].gameObject.SetActive(true);
                multiScene.GetAllStaticMultiObject[i].tag = "Untagged";
            }
        }
        EditorUtility.SetDirty(multiScene);
    }

    private void CreateBakedMultiObjectData(MultiScene multiScene)
    {
        BakedMultiObjectData bakedMultiObjectData = ScriptableObject.CreateInstance<BakedMultiObjectData>();
        AssetDatabase.CreateAsset(bakedMultiObjectData, MultiSceneWindow.GetReadyPathFolderWithBakedAssets + "/" + multiScene.SceneName + ".asset");
        multiScene.BakedData = bakedMultiObjectData;
        MultiObjectData[] multiObjectDatas = new MultiObjectData[multiScene.GetAllStaticMultiObject.Count];
        for (int i = 0; i < multiScene.GetAllStaticMultiObject.Count; i++)
        {
            MultiObjectReference multiObjectReference = new MultiObjectReference(MultiSceneWindow.GetNextGlobalIndex(), multiScene.SceneName.GetHashCode());
            multiObjectDatas[i] = new MultiObjectData(multiScene.GetAllStaticMultiObject[i].name, multiScene.GetAllStaticMultiObject[i], multiObjectReference);
            multiObjectDatas[i].Update();
            multiScene.GetAllStaticMultiObject[i].SetupMetadata(multiObjectReference);
        };
        bakedMultiObjectData.MultiObjectDatas = multiObjectDatas;

        EditorUtility.SetDirty(bakedMultiObjectData);
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Debug.Log("MultiScene: Baked ready!");
    }
}
