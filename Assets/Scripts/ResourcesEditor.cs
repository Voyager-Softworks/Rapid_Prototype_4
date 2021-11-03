using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;

[CustomEditor(typeof(Resources))]
public class ResourcesEditor : Editor
{
    public override void OnInspectorGUI()
    {

        Resources resources = (Resources)target;
        
        Undo.RecordObject(resources, "resources");

        EditorGUILayout.LabelField("Scene Instances", EditorStyles.boldLabel);

        //draw player prefab field and button to find it
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Player Prefab");
        resources.player = (GameObject)EditorGUILayout.ObjectField(resources.player, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            resources.player = GameObject.FindGameObjectWithTag("Player");
            PrefabUtility.RecordPrefabInstancePropertyModifications(resources.player);
        }
        EditorGUILayout.EndHorizontal();

        //draw player resources
        EditorGUILayout.LabelField("Player Resources", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Organic");
        resources.g_playerResources[Resources.ResourceType.Organic] = EditorGUILayout.IntField(resources.g_playerResources[Resources.ResourceType.Organic]);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scrap");
        resources.g_playerResources[Resources.ResourceType.Scrap] = EditorGUILayout.IntField(resources.g_playerResources[Resources.ResourceType.Scrap]);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Power");
        resources.g_playerResources[Resources.ResourceType.Power] = EditorGUILayout.IntField(resources.g_playerResources[Resources.ResourceType.Power]);
        EditorGUILayout.EndHorizontal();

        //add a space between the player resources and the enemy resources
        EditorGUILayout.Space();
        
        //draw resource prefabs and buttons to find them in the project files
        EditorGUILayout.LabelField("Resource Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Organic");
        resources.m_organicPrefab = (GameObject)EditorGUILayout.ObjectField(resources.m_organicPrefab, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            resources.m_organicPrefab = UnityEngine.Resources.Load<GameObject>("Organic");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scrap");
        resources.m_scrapPrefab = (GameObject)EditorGUILayout.ObjectField(resources.m_scrapPrefab, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            resources.m_scrapPrefab = UnityEngine.Resources.Load<GameObject>("Scrap");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Power");
        resources.m_powerPrefab = (GameObject)EditorGUILayout.ObjectField(resources.m_powerPrefab, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            resources.m_powerPrefab = UnityEngine.Resources.Load<GameObject>("Power");
        }
        EditorGUILayout.EndHorizontal();

    }
}
#endif