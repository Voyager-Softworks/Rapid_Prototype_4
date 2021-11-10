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

        //draw player instance field and button to find it
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Player Instance");
        resources.player = (GameObject)EditorGUILayout.ObjectField(resources.player, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            serializedObject.FindProperty("player").objectReferenceValue = GameObject.FindGameObjectWithTag("Player");
        }
        EditorGUILayout.EndHorizontal();

        //draw resource text instance field and button to find it
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Resource Text Instance");
        resources.resourceText = (GameObject)EditorGUILayout.ObjectField(resources.resourceText, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            serializedObject.FindProperty("resourceText").objectReferenceValue = GameObject.Find("Resource_Text");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //draw player resources
        EditorGUILayout.LabelField("Player Resources", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Organic");
        resources.playerResources[(int)Resources.ResourceType.ORGANIC].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.ORGANIC].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.ORGANIC).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.ORGANIC].amount;
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scrap");
        resources.playerResources[(int)Resources.ResourceType.SCRAP].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.SCRAP].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.SCRAP).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.SCRAP].amount;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Power");
        resources.playerResources[(int)Resources.ResourceType.POWER].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.POWER].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.POWER).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.POWER].amount;
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