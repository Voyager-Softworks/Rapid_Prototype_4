using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(ResourceSpawner))]
public class ResourceSpawner_Editor : Editor {
    ResourceSpawner resourceSpawner;

    private void OnEnable()
    {
        resourceSpawner = (ResourceSpawner)target;

        if (resourceSpawner.data == null)
        {
            AssetDatabase.Refresh();
            resourceSpawner.data = ScriptableObject.CreateInstance<ResourceSpawner_Data>();
            AssetDatabase.CreateAsset(resourceSpawner.data, AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/ResourceSpawner/ResourceSpawnerData.asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Bake")) {
            resourceSpawner.BakeSpawnPoints();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(resourceSpawner.data);
            AssetDatabase.SaveAssets();
        }
    }
}
