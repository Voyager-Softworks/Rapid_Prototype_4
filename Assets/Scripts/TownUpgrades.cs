using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class TownUpgrades : MonoBehaviour
{
    private Resources resourceManager = null;
    public GameObject player = null;

    public int level = 0;
    public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();

    public UnityEvent MaxLevelReached;

    [Header("Menu")]
    public TownMenu townMenu = null;

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

        if (!GetComponent<DontDestroy>().CheckValidInstance()) return;

        if (!player) player = GameObject.FindGameObjectWithTag("Player");

        resourceManager = GetComponent<Resources>();

        if (scene.name == "Level_Hub") {
            LoadMenu();

            UpdateMenu();
        }
    }

    private void UpdateMenu()
    {
        Button upgradeButton = townMenu.upgradeButton.GetComponent<Button>();
        TextMeshProUGUI upgradeButtonText = townMenu.upgradeButton.GetComponentInChildren<TextMeshProUGUI>();

        upgradeButton.onClick.RemoveAllListeners();
        //set text to "Upgrade"
        upgradeButtonText.text = "UPGRADE\n";
        //add current level cost to text
        if (level < upgradeLevels.Count) {
            foreach (Resources.PlayerResource cost in upgradeLevels[level].cost) {
                upgradeButtonText.text += cost.amount + " " + cost.type.ToString() + "\n";
            }
        }

        if (level >= upgradeLevels.Count) {
            upgradeButton.interactable = false;
            upgradeButtonText.text = "Max Level";
        } else {
            upgradeButton.interactable = true;
            upgradeButton.onClick.AddListener(() => {
                TryUpgrade();
            });
        }
    }

    private void LoadMenu()
    {
        townMenu = GameObject.FindObjectOfType<TownMenu>(true);
    }

    public void TryUpgrade(){
        if (resourceManager.TryConsumeResources(upgradeLevels[level].cost)) {
            level++;
            UpdateMenu();
        }

        if (level >= upgradeLevels.Count) {
            MaxLevelReached.Invoke();
        }
    }
}
