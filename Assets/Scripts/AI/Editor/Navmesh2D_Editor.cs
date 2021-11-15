using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Navmesh2D))]
public class Navmesh2D_Editor : Editor
{
    Navmesh2D navmesh;
    

    
    private void OnEnable()
    {
        navmesh = (Navmesh2D)target;
        if (navmesh.m_data == null)
        {
            AssetDatabase.Refresh();
            navmesh.m_data = ScriptableObject.CreateInstance<Navmesh2D_Data>();
            navmesh.m_data.Create(navmesh.m_width, navmesh.m_height, navmesh.m_cellradius, navmesh.m_origin);
            navmesh.GenerateNavmesh();


            AssetDatabase.CreateAsset(navmesh.m_data, AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Navmesh2D/NavmeshData2D.asset"));
            navmesh.m_path = AssetDatabase.GetAssetPath(navmesh.m_data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        

        if (GUILayout.Button("Bake Navmesh"))
        {
            
            navmesh.m_data.Create(navmesh.m_width, navmesh.m_height, navmesh.m_cellradius, navmesh.m_origin);
            navmesh.GenerateNavmesh();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(navmesh.m_data);
            AssetDatabase.SaveAssets();
            
        }
    }
    
    


}
