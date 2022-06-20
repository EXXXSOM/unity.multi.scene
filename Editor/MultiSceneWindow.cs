using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class MultiSceneWindow : EditorWindow
{
    public const string DEFAULT_PATH_FOLDER_WITH_BAKED_MULTI_OBJECTS = "BakedMultiObjectData";
    private static string _pathFolderWithBakedMultiObjects = string.Empty;
    private static MultiSceneSettings _multiSceneSettings;

    public static string GetReadyPathFolderWithBakedAssets => "Assets/" + _multiSceneSettings.PathFolderWithBakedMultiObjects;

    static MultiSceneWindow()
    {
        LoadSettings();
        CreateFolderWithBackedObjects();
        Debug.Log("[MultiObjectManagerEditor]: Load settings...");
    }

    [MenuItem("Window/Multi Scene Settings")]
    public static void ShowWindow()
    {
        if (_multiSceneSettings == null)
            LoadSettings();

        CreateFolderWithBackedObjects();
        EditorWindow.GetWindow(typeof(MultiSceneWindow));
    }

    public static int GetNextGlobalIndex()
    {
        if (_multiSceneSettings == null)
            LoadSettings();

        int nextGlobalIndex = _multiSceneSettings.NextGlobalIndex;
        _multiSceneSettings.NextGlobalIndex++;
        EditorUtility.SetDirty(_multiSceneSettings);
        return nextGlobalIndex;
    }

    private static void InitWindow()
    {
        _pathFolderWithBakedMultiObjects = _multiSceneSettings.PathFolderWithBakedMultiObjects;
    }

    public static void LoadSettings()
    {
        _multiSceneSettings = Resources.Load<MultiSceneSettings>(MultiSceneManager.NAME_FILE_SETTINGS);
        if (_multiSceneSettings == null)
            _multiSceneSettings = CreateMultiSceneSettings();

        InitWindow();
    }

    public static void CreateFolderWithBackedObjects()
    {
        if (!AssetDatabase.IsValidFolder(GetReadyPathFolderWithBakedAssets))
        {
            string[] folders = GetReadyPathFolderWithBakedAssets.Split('/');
            string path = string.Empty;
            string createFolderName = folders[folders.Length - 1];
            for (int i = 0; i < folders.Length - 1; i++)
            {
                path += folders[i];
                AssetDatabase.CreateFolder(path, folders[i + 1]);
                if (folders.Length - 2 != i)
                    path += '/';
            }
        }
    }

    private static MultiSceneSettings CreateMultiSceneSettings()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.Refresh();
        }

        MultiSceneSettings multiSceneSettings = ScriptableObject.CreateInstance<MultiSceneSettings>();
        AssetDatabase.CreateAsset(multiSceneSettings, "Assets/Resources/" + MultiSceneManager.NAME_FILE_SETTINGS + ".asset");
        return multiSceneSettings;
    }

    void OnGUI()
    {
        GUILayout.Label("Multi Scene Settings", EditorStyles.boldLabel);
        _pathFolderWithBakedMultiObjects = EditorGUILayout.TextField("Baked path: Assets/", _pathFolderWithBakedMultiObjects);
        GUILayout.Label("NextGlobalIndex: " + _multiSceneSettings.NextGlobalIndex);
        _multiSceneSettings.MultiObjectsDataCapacity = EditorGUILayout.IntField("MultiObjectsDataCapacity", _multiSceneSettings.MultiObjectsDataCapacity);


        if (GUILayout.Button("Save settings"))
        {
            Debug.Log(_pathFolderWithBakedMultiObjects + " / " + _multiSceneSettings.PathFolderWithBakedMultiObjects);
            if (_pathFolderWithBakedMultiObjects != _multiSceneSettings.PathFolderWithBakedMultiObjects)
            {
                AssetDatabase.MoveAsset(GetReadyPathFolderWithBakedAssets, "Assets/" + _pathFolderWithBakedMultiObjects);
                _multiSceneSettings.PathFolderWithBakedMultiObjects = _pathFolderWithBakedMultiObjects;
                Debug.Log("Directory changed!");
            }

            EditorUtility.SetDirty(_multiSceneSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public static string GetPathBakedMultiObjectData()
    {
        return "Assets/" + _multiSceneSettings.PathFolderWithBakedMultiObjects;
    }
}