using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    public GameObject player = null;
    private PlayerWeapons playerWeapons = null;
    public WeaponType leftEquippedWeapon = WeaponType.CANNON;
    public WeaponType rightEquippedWeapon = WeaponType.CANNON;

    [Header("Menu")]
    public UpgradeMenu upgradeMenu = null;

    [Header("Upgrades")]
    public List<Upgrade> weaponUpgrades = null;
    public List<Upgrade> moduleUpgrades = null;

    void Awake() {
        if (!GetComponent<DontDestroy>().CheckValidInstance()) return;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this == null) return;
        if (gameObject == null) return;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player) playerWeapons = player.GetComponent<PlayerWeapons>();

        if (!GetComponent<DontDestroy>().CheckValidInstance()) return;

        if (resourceManager == null) resourceManager = GetComponent<Resources>();

        if (scene.name == "Level_Hub")
        {
            LoadUpgradeMenu();

            UpdateUpgrades();

            UpdateUpgradeMenu();
        }

        UpdateEquippedWeapons();
    }

    private void Update() {
        //check if in hub level
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Level_Hub") {
            //if upgrade menu exists
            if (upgradeMenu) {
                //if upgrade menu is open
                if (upgradeMenu.gameObject.activeSelf) {
                    bool anyTrue = false;
                    //loop through all weaponItems in upgrade menu
                    for (int i = 0; i < upgradeMenu.weaponItems.Count; i++) {
                        //if weaponItem is not active, skip
                        if (!upgradeMenu.weaponItems[i].gameObject.activeSelf || i > weaponUpgrades.Count) continue;

                        //convert mouse position to world position
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

                        Vector3[] rectCorners = new Vector3[4];
                        upgradeMenu.weaponItems[i].GetComponent<RectTransform>().GetWorldCorners(rectCorners);

                        //check if mouse is within rectCorners, enable upgradeMenu infoPanel
                        if (mousePos.x > rectCorners[0].x && mousePos.x < rectCorners[2].x && mousePos.y > rectCorners[0].y && mousePos.y < rectCorners[2].y) {
                            upgradeMenu.infoPanel.SetActive(true);
                            upgradeMenu.infoPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(weaponUpgrades[i].description);
                            anyTrue = true;
                            upgradeMenu.infoPanel.transform.position = new Vector3(mousePos.x, mousePos.y + 0.5f, 0);
                            break;
                        }
                    }

                    if (!anyTrue) {
                        upgradeMenu.infoPanel.SetActive(false);
                    }
                }
                else{
                    upgradeMenu.infoPanel.SetActive(false);
                }
            }
        }
    }

    private void UpdateEquippedWeapons()
    {
        playerWeapons.EquipWeapon(leftEquippedWeapon, true);
        playerWeapons.EquipWeapon(rightEquippedWeapon, false);
    }

    private void LoadUpgradeMenu()
    {
        upgradeMenu = GameObject.FindObjectOfType<UpgradeMenu>(true);
    }

    private void UpdateUpgrades()
    {

    }

    private void UpdateUpgradeMenu()
    {
        //loop through all of the board weapon upgrade items, and add an upgrade
        for (int i = 0; i < upgradeMenu.weaponItems.Count; i++)
        {
            if (i < weaponUpgrades.Count)
            {
                Upgrade upgrade = weaponUpgrades[i];

                GameObject upgradeItem = upgradeMenu.weaponItems[i];
                upgradeItem.SetActive(true);
                TextMeshProUGUI text = upgradeItem.GetComponentInChildren<TextMeshProUGUI>();
                text.text = upgrade.weaponType.ToString();

                GameObject equipL = upgradeItem.transform.Find("L").gameObject;
                GameObject equipR = upgradeItem.transform.Find("R").gameObject;
                equipL.GetComponent<Button>().onClick.RemoveAllListeners();
                equipR.GetComponent<Button>().onClick.RemoveAllListeners();
                //update the equip button colours to green if the player has equipped the weapon
                if (leftEquippedWeapon == upgrade.weaponType)
                {
                    equipL.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    equipL.GetComponent<Image>().color = Color.white;
                }
                
                if (rightEquippedWeapon == upgrade.weaponType)
                {
                    equipR.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    equipR.GetComponent<Image>().color = Color.white;
                }

                //bind the equip buttons to PlayerWeapons EquipWeapon
                equipL.GetComponent<Button>().onClick.AddListener(() => {
                    leftEquippedWeapon = upgrade.weaponType;
                    UpdateEquippedWeapons();
                    UpdateUpgradeMenu();
                });
                equipR.GetComponent<Button>().onClick.AddListener(() => {
                    rightEquippedWeapon = upgrade.weaponType;
                    UpdateEquippedWeapons();
                    UpdateUpgradeMenu();
                });

                Button button = upgradeItem.GetComponentInChildren<Button>();
                button.onClick.RemoveAllListeners();
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

                List<Image> images = new List<Image>(button.GetComponentsInChildren<Image>());
                //remove the first image, which is the button
                images.RemoveAt(0);
                List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>(button.GetComponentsInChildren<TextMeshProUGUI>());
                //remove the first text, which is the button text
                texts.RemoveAt(0);

                //if not unlocked, button says buy
                if (!upgrade.unlocked)
                {
                    buttonText.text = "BUY";
                    
                    //link button to buy upgrade
                    button.onClick.AddListener(() => {
                        if (resourceManager.TryConsumeResources(upgrade.cost))
                        {
                            upgrade.unlocked = true;
                            UpdateUpgradeMenu();
                        }
                    });

                    equipL.SetActive(false);
                    equipR.SetActive(false);

                    for (int j = 0; j < images.Count; j++)
                    {
                        if (j < upgrade.cost.Count)
                        {
                            Resources.PlayerResource cost = upgrade.cost[j];

                            Image costImage = images[j];
                            costImage.enabled = true;
                            TextMeshProUGUI costText = texts[j];
                            costText.enabled = true;

                            if (cost.type == Resources.ResourceType.ORGANIC) costImage.color = Color.red;
                            else if (cost.type == Resources.ResourceType.SCRAP) costImage.color = Color.grey;
                            else if (cost.type == Resources.ResourceType.POWER) costImage.color = Color.green;

                            costText.text = cost.amount.ToString();
                        }
                        else
                        {
                            Image costImage = images[j];
                            costImage.enabled = false;
                            TextMeshProUGUI costText = texts[j];
                            costText.enabled = false;
                        }
                    }
                }
                else if (upgrade.levels.Count > upgrade.level)
                {
                    buttonText.text = "UPGRADE";

                    //link button to upgrade weapon
                    button.onClick.AddListener(() => {
                        
                        if (upgrade.levels.Count > upgrade.level)
                        {
                            if (resourceManager.TryConsumeResources(upgrade.levels[upgrade.level].cost))
                            {
                                upgrade.levels[upgrade.level].onUpgrade.Invoke();
                                upgrade.level++;
                                UpdateUpgradeMenu();
                            }
                        }
                    });

                    equipL.SetActive(true);
                    equipR.SetActive(true);

                    for (int j = 0; j < images.Count; j++)
                    {
                        if (upgrade.levels.Count > upgrade.level && j < upgrade.levels[upgrade.level].cost.Count)
                        {
                            Resources.PlayerResource cost = upgrade.levels[upgrade.level].cost[j];

                            Image costImage = images[j];
                            costImage.enabled = true;
                            TextMeshProUGUI costText = texts[j];
                            costText.enabled = true;

                            if (cost.type == Resources.ResourceType.ORGANIC) costImage.color = Color.red;
                            else if (cost.type == Resources.ResourceType.SCRAP) costImage.color = Color.grey;
                            else if (cost.type == Resources.ResourceType.POWER) costImage.color = Color.green;

                            costText.text = cost.amount.ToString();
                        }
                        else
                        {
                            Image costImage = images[j];
                            costImage.enabled = false;
                            TextMeshProUGUI costText = texts[j];
                            costText.enabled = false;
                        }
                    }

                }
                else{
                    buttonText.text = "MAXED";
                    equipL.SetActive(true);
                    equipR.SetActive(true);
                    
                    for (int j = 0; j < images.Count; j++)
                    {
                        Image costImage = images[j];
                        costImage.enabled = false;
                        TextMeshProUGUI costText = texts[j];
                        costText.enabled = false;
                    }
                }
            }
            else
            {
                GameObject upgradeItem = upgradeMenu.weaponItems[i];
                upgradeItem.SetActive(false);
            }
        }
    }
}
