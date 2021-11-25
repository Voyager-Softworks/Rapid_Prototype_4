using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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
            if(spawnNode.m_path != "")
            {
                spawnNode.data = AssetDatabase.LoadAssetAtPath<SpawnNode_Data>(spawnNode.m_path);
                if(spawnNode.data == null)
                {
                    AssetDatabase.Refresh();
                    spawnNode.data = ScriptableObject.CreateInstance<SpawnNode_Data>();
                    
                    AssetDatabase.CreateAsset(spawnNode.data, "Assets/Resources/SpawnNode/" + EditorSceneManager.GetActiveScene().name + "_" + spawnNode.gameObject.name + "_SpawnNodeData.asset");
                    spawnNode.m_path = AssetDatabase.GetAssetPath(spawnNode.data);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                AssetDatabase.Refresh();
                    spawnNode.data = ScriptableObject.CreateInstance<SpawnNode_Data>();
                    
                    AssetDatabase.CreateAsset(spawnNode.data, "Assets/Resources/SpawnNode/" + EditorSceneManager.GetActiveScene().name + "_" + spawnNode.gameObject.name + "_SpawnNodeData.asset");
                    spawnNode.m_path = AssetDatabase.GetAssetPath(spawnNode.data);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
            }
            
            AssetDatabase.Refresh();
        }
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Bake")) {
            AssetDatabase.Refresh();
            spawnNode.RegenerateSpawnPoints();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(spawnNode.data);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

