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
        //ichor
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ichor");
        resources.playerResources[(int)Resources.ResourceType.ICHOR].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.ICHOR].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.ICHOR).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.ICHOR].amount;
        EditorGUILayout.EndHorizontal();
        
        //scrap
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scrap");
        resources.playerResources[(int)Resources.ResourceType.SCRAP].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.SCRAP].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.SCRAP).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.SCRAP].amount;
        EditorGUILayout.EndHorizontal();

        //crystal
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Crystal");
        resources.playerResources[(int)Resources.ResourceType.CRYSTAL].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.CRYSTAL].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.CRYSTAL).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.CRYSTAL].amount;
        EditorGUILayout.EndHorizontal();

        //exotic
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Exotic");
        resources.playerResources[(int)Resources.ResourceType.EXOTIC].amount = EditorGUILayout.IntField(resources.playerResources[(int)Resources.ResourceType.EXOTIC].amount);
        serializedObject.FindProperty("playerResources").GetArrayElementAtIndex((int)Resources.ResourceType.EXOTIC).FindPropertyRelative("amount").intValue = resources.playerResources[(int)Resources.ResourceType.EXOTIC].amount;
        EditorGUILayout.EndHorizontal();



        //space
        EditorGUILayout.Space();
        
        //draw resource prefabs and buttons to find them in the project files
        EditorGUILayout.LabelField("Resource Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        //ichor
        EditorGUILayout.LabelField("Ichor");
        resources.m_ICHORPrefab = (GameObject)EditorGUILayout.ObjectField(resources.m_ICHORPrefab, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            resources.m_ICHORPrefab = UnityEngine.Resources.Load<GameObject>("UI_ichor");
        }
        EditorGUILayout.EndHorizontal();
        //scrap
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scrap");
        resources.m_SCRAPPrefab = (GameObject)EditorGUILayout.ObjectField(resources.m_SCRAPPrefab, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            resources.m_SCRAPPrefab = UnityEngine.Resources.Load<GameObject>("UI_scrap");
        }
        EditorGUILayout.EndHorizontal();
        //crystal
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Crystal");
        resources.m_CRYSTALPrefab = (GameObject)EditorGUILayout.ObjectField(resources.m_CRYSTALPrefab, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            resources.m_CRYSTALPrefab = UnityEngine.Resources.Load<GameObject>("UI_crystal");
        }
        EditorGUILayout.EndHorizontal();
        //exotic
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Exotic");
        resources.m_EXOTICPrefab = (GameObject)EditorGUILayout.ObjectField(resources.m_EXOTICPrefab, typeof(GameObject), true);
        if (GUILayout.Button("Find"))
        {
            resources.m_EXOTICPrefab = UnityEngine.Resources.Load<GameObject>("UI_exotic");
        }
        EditorGUILayout.EndHorizontal();


        serializedObject.ApplyModifiedProperties();
    }
}
#endif