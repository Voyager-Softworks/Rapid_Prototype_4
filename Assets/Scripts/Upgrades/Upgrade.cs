using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;

[Serializable]
public class Upgrade
{
    public UpgradeManager.UpgradeType type = UpgradeManager.UpgradeType.WEAPON;
    public int level = 0;
    public UpgradeManager.WeaponType weaponType = UpgradeManager.WeaponType.NONE;
    public UpgradeManager.ModuleType moduleType = UpgradeManager.ModuleType.NONE;
    public List<UpgradeLevel> levels;
}