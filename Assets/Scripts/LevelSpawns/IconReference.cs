using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//if in editor mode
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IconReference : MonoBehaviour
{
    [Serializable]
    public class EnemyIcon{
        [SerializeField] public EnemyTest.EnemyType enemyType;
        [SerializeField] public Sprite icon;
    }
    [SerializeField] public List<EnemyIcon> enemyIcons;

    #region Editor Stuff
    public GameObject InteractCanvasPrefab;

    //if in editor mode, check if there is an InteractCanvas on this object
    #if UNITY_EDITOR
    [CustomEditor(typeof(IconReference))]
    public class IconReferenceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            IconReference myScript = (IconReference)target;

            //display all the icons in the list, with the enemy type not ediatble, and the icon editable
            for (int i = 0; i < myScript.enemyIcons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //text for the enemy type
                EditorGUILayout.LabelField(myScript.enemyIcons[i].enemyType.ToString());
                myScript.enemyIcons[i].icon = (Sprite)EditorGUILayout.ObjectField(myScript.enemyIcons[i].icon, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();
            }

            //if anytihng is changed, save the changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(myScript);
            }

            //button to updateall
            if (GUILayout.Button("Add Missing"))
            {
                myScript.AddMissing();
                EditorUtility.SetDirty(myScript);
            }
        }
    }

    public void AddMissing(){
        foreach (EnemyTest.EnemyType enemyType in Enum.GetValues(typeof(EnemyTest.EnemyType)))
        {
            //check if there is an icon for this enemy type
            if (GetIcon(enemyType) == null)
            {
                enemyIcons.Add(new EnemyIcon() { enemyType = enemyType, icon = null });
            }
        }
    }

    public EnemyIcon GetIcon(EnemyTest.EnemyType enemyType){
        foreach (EnemyIcon enemyIcon in enemyIcons)
        {
            if (enemyIcon.enemyType == enemyType)
            {
                return enemyIcon;
            }
        }
        return null;
    }

    #endif
    #endregion
}
