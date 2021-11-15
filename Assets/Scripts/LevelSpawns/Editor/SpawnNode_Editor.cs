using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnNode))]
public class SpawnNode_Editor : Editor 
{
    SpawnNode spawnNode;

    private void OnEnable()
    {
        spawnNode = (SpawnNode)target;

        if (spawnNode.data == null)
        {
            AssetDatabase.Refresh();
            spawnNode.data = ScriptableObject.CreateInstance<SpawnNode_Data>();
            AssetDatabase.CreateAsset(spawnNode.data, AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/SpawnNode/SpawnNodeData.asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Bake")) {
            spawnNode.RegenerateSpawnPoints();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(spawnNode.data);
            AssetDatabase.SaveAssets();
        }
    }
}

