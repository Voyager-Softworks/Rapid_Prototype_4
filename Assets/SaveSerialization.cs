using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

//if in editor mode
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SaveSerialization : MonoBehaviour
{
    void Start()
    {
        LoadData();
    }
    
    public void LoadData()
    {
        Resources r = FindObjectOfType<Resources>();
        TownUpgrades t = FindObjectOfType<TownUpgrades>();
        UpgradeManager u = FindObjectOfType<UpgradeManager>();
        BountyManager b = FindObjectOfType<BountyManager>();
        PlayerStats p = FindObjectOfType<PlayerStats>();


        Save save = Load();

        if(save == null) return;

        r.playerResources = save.playerResources;

        t.level = save.townLevel;

        u.weaponUpgrades = save.weaponUpgrades;

        u.moduleUpgrades = save.moduleUpgrades;

        b.inactiveBounties = save.inactiveBounties;
        b.activeBounties = save.activeBounties;
        b.completedBounties = save.completedBounties;
        b.failedBounties = save.failedBounties;

        p.killList = save.killList;
        p.total_kills = save.total_kills;
        p.total_deaths = save.total_deaths;


    }

    public void SaveData()
    {
        Resources r = FindObjectOfType<Resources>();
        TownUpgrades t = FindObjectOfType<TownUpgrades>();
        UpgradeManager u = FindObjectOfType<UpgradeManager>();
        BountyManager b = FindObjectOfType<BountyManager>();
        PlayerStats p = FindObjectOfType<PlayerStats>();

        Save save = new Save();

        save.playerResources = r.playerResources;

        save.townLevel = t.level;

        save.weaponUpgrades = u.weaponUpgrades;

        save.moduleUpgrades = u.moduleUpgrades;

        save.inactiveBounties = b.inactiveBounties;
        save.activeBounties = b.activeBounties;
        save.completedBounties = b.completedBounties;
        save.failedBounties = b.failedBounties;

        save.killList = p.killList;
        save.total_kills = p.total_kills;
        save.total_deaths = p.total_deaths;

        Save(save);
    }

    static public void Save(Save _save)
    {
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            File.Delete(Application.persistentDataPath + "/save.dat");
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, _save);
        file.Close();
    }

    static public void DeleteSaveData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            File.Delete(Application.persistentDataPath + "/save.dat");
        }
    }

    static public bool SaveExists()
    {
        return File.Exists(Application.persistentDataPath + "/save.dat");
    }

    static public Save Load()
    {
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
            file.Position = 0;
            Save save = (Save)bf.Deserialize(file);
            file.Close();
            return save;
        }
        else
        {
            return null;
        }
    }

    #region Editor Stuff

    //if in editor mode, check if there is an InteractCanvas on this object
    #if UNITY_EDITOR
    [CustomEditor(typeof(SaveSerialization))]
    public class SaveSerializationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SaveSerialization console = (SaveSerialization)target;

            //add audiosource button
            if (GUILayout.Button("DELETE ALL SAVES"))
            {
                DeleteSaveData();
            }
        }
    }

    #endif
    #endregion
}
