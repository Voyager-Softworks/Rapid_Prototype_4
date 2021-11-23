using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;
using UnityEngine.UI;

[Serializable]
public class Upgrade
{
    public UpgradeManager.UpgradeType type = UpgradeManager.UpgradeType.WEAPON;
    public UpgradeManager.WeaponType weaponType = UpgradeManager.WeaponType.NONE;
    public UpgradeManager.ModuleType moduleType = UpgradeManager.ModuleType.NONE;
    public bool unlocked = false;
    public int level = 0;
    public string description = "";
    public List<Resources.PlayerResource> cost = new List<Resources.PlayerResource>(){};
    public List<UpgradeLevel> levels;
}