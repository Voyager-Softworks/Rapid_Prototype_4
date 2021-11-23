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

    [Serializable]
    public class ResourceIcon{
        [SerializeField] public Resources.ResourceType resourceType;
        [SerializeField] public Sprite icon;
    }

    [Serializable]
    public class WeaponIcon{
        [SerializeField] public UpgradeManager.WeaponType weaponType;
        [SerializeField] public Sprite icon;
    }

    [Serializable]
    public class ModuleIcon{
        [SerializeField] public UpgradeManager.ModuleType moduleType;
        [SerializeField] public Sprite icon;
    }

    [SerializeField] public List<EnemyIcon> enemyIcons;
    [SerializeField] public List<ResourceIcon> resourceIcons;
    [SerializeField] public List<WeaponIcon> weaponIcons;
    [SerializeField] public List<ModuleIcon> moduleIcons;

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

            //header for enemy icons
            EditorGUILayout.LabelField("Enemy Icons", EditorStyles.boldLabel);

            //display all the icons in the list, with the enemy type not ediatble, and the icon editable
            for (int i = 0; i < myScript.enemyIcons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //text for the enemy type
                EditorGUILayout.LabelField(myScript.enemyIcons[i].enemyType.ToString());
                myScript.enemyIcons[i].icon = (Sprite)EditorGUILayout.ObjectField(myScript.enemyIcons[i].icon, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();
            }

            //header for resource icons
            EditorGUILayout.LabelField("Resource Icons", EditorStyles.boldLabel);

            //display all the icons in the list, with the resource type not ediatble, and the icon editable
            for (int i = 0; i < myScript.resourceIcons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //text for the resource type
                EditorGUILayout.LabelField(myScript.resourceIcons[i].resourceType.ToString());
                myScript.resourceIcons[i].icon = (Sprite)EditorGUILayout.ObjectField(myScript.resourceIcons[i].icon, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();
            }

            //header for weapon icons
            EditorGUILayout.LabelField("Weapon Icons", EditorStyles.boldLabel);

            //display all the icons in the list, with the weapon type not ediatble, and the icon editable
            for (int i = 0; i < myScript.weaponIcons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //text for the weapon type
                EditorGUILayout.LabelField(myScript.weaponIcons[i].weaponType.ToString());
                myScript.weaponIcons[i].icon = (Sprite)EditorGUILayout.ObjectField(myScript.weaponIcons[i].icon, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();
            }

            //header for module icons
            EditorGUILayout.LabelField("Module Icons", EditorStyles.boldLabel);

            //display all the icons in the list, with the module type not ediatble, and the icon editable
            for (int i = 0; i < myScript.moduleIcons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //text for the module type
                EditorGUILayout.LabelField(myScript.moduleIcons[i].moduleType.ToString());
                myScript.moduleIcons[i].icon = (Sprite)EditorGUILayout.ObjectField(myScript.moduleIcons[i].icon, typeof(Sprite), false);
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
        //enemies
        foreach (EnemyTest.EnemyType enemyType in Enum.GetValues(typeof(EnemyTest.EnemyType)))
        {
            //check if there is an icon for this enemy type
            if (GetIcon(enemyType) == null)
            {
                enemyIcons.Add(new EnemyIcon() { enemyType = enemyType, icon = null });
            }
        }

        //resources
        foreach (Resources.ResourceType resourceType in Enum.GetValues(typeof(Resources.ResourceType)))
        {
            //check if there is an icon for this resource type
            if (GetIcon(resourceType) == null)
            {
                resourceIcons.Add(new ResourceIcon() { resourceType = resourceType, icon = null });
            }
        }

        //weapons
        foreach (UpgradeManager.WeaponType weaponType in Enum.GetValues(typeof(UpgradeManager.WeaponType)))
        {
            //check if there is an icon for this weapon type
            if (GetIcon(weaponType) == null)
            {
                weaponIcons.Add(new WeaponIcon() { weaponType = weaponType, icon = null });
            }
        }

        //modules
        foreach (UpgradeManager.ModuleType moduleType in Enum.GetValues(typeof(UpgradeManager.ModuleType)))
        {
            //check if there is an icon for this module type
            if (GetIcon(moduleType) == null)
            {
                moduleIcons.Add(new ModuleIcon() { moduleType = moduleType, icon = null });
            }
        }
    }

    #endif
    #endregion

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

    public ResourceIcon GetIcon(Resources.ResourceType resourceType){
        foreach (ResourceIcon resourceIcon in resourceIcons)
        {
            if (resourceIcon.resourceType == resourceType)
            {
                return resourceIcon;
            }
        }
        return null;
    }

    public WeaponIcon GetIcon(UpgradeManager.WeaponType weaponType){
        foreach (WeaponIcon weaponIcon in weaponIcons)
        {
            if (weaponIcon.weaponType == weaponType)
            {
                return weaponIcon;
            }
        }
        return null;
    }

    public ModuleIcon GetIcon(UpgradeManager.ModuleType moduleType){
        foreach (ModuleIcon moduleIcon in moduleIcons)
        {
            if (moduleIcon.moduleType == moduleType)
            {
                return moduleIcon;
            }
        }
        return null;
    }
}
