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

        EditorGUILayout.LabelField("Scene Instances", EditorStyles.boldLabel);

        //draw player prefab field and button to find it
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Player Prefab");
        resources.player = (GameObject)EditorGUILayout.ObjectField(resources.player, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            serializedObject.FindProperty("player").objectReferenceValue = GameObject.FindGameObjectWithTag("Player");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //draw player resources
        EditorGUILayout.LabelField("Player Resources", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Organic");
        resources.playerResources[(int)Resources.ResourceType.Organic].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.Organic].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.Organic).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.Organic].amount;
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scrap");
        resources.playerResources[(int)Resources.ResourceType.Scrap].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.Scrap].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.Scrap).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.Scrap].amount;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Power");
        resources.playerResources[(int)Resources.ResourceType.Power].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.Power].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.Power).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.Power].amount;
        EditorGUILayout.EndHorizontal();

        //space
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

        serializedObject.ApplyModifiedProperties();
    }
}
#endif