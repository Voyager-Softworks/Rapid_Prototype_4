using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    public enum UpgradeType
    {
        WEAPON,
        MODULE
    }

    public enum WeaponType {
        NONE,
        CANNON,
        MINIGUN,
        RAILGUN
    }

    public enum ModuleType {
        NONE,
        SLAM,
        DASH,
        JETPACK
    }

    private Resources resourceManager = null;

    [Header("Menu")]
    public UpgradeMenu upgradeMenu = null;

    [Header("Upgrades")]
    public List<Upgrade> weaponUpgrades = null;
    public List<Upgrade> moduleUpgrades = null;

    void Awake() {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this == null) return;
        if (gameObject == null) return;

        if (!GetComponent<DontDestroy>().CheckValidInstance()) return;

        if (resourceManager == null) resourceManager = GetComponent<Resources>();

        if (scene.name == "Level_Hub")
        {
            LoadUpgradeMenu();

            UpdateUpgrades();

            UpdateUpgradeMenu();
        }
    }

    private void LoadUpgradeMenu()
    {
        upgradeMenu = GameObject.FindObjectOfType<UpgradeMenu>();
    }

    private void UpdateUpgrades()
    {
        
    }

    private void UpdateUpgradeMenu()
    {
        
    }
}
