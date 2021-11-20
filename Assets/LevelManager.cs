using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class LevelManager : MonoBehaviour
{
    public enum LevelType {
        HUB,
        WASTELAND,
        CAVES,
        VOLCANO,
        VAULT
    }

    public enum LevelMods {
        SWARM,
        ALLELITE,
        RADIATION,
        STORM
    }

    [Header("Board")]
    TeleportMenu teleportMenu = null;
    BountyManager bountyManager = null;
    SceneController sceneController = null;

    [Header("Level Stats")]
    public BountyManager.BountyDifficulty bountyDifficulty = BountyManager.BountyDifficulty.EASY;
    public BountyManager.BountyType bountyType = BountyManager.BountyType.KILL;
    public List<LevelMods> levelMods = new List<LevelMods>(){};


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

        bountyManager = GetComponent<BountyManager>();
        sceneController = GetComponent<SceneController>();

        if (scene.name == "Level_Hub")
        {
            LoadMenu();

            UpdateMenu();
        }
    }

    public void GenLevel(){
        if (bountyManager.activeBounties.Count > 0){
            bountyDifficulty = bountyManager.activeBounties[0].bountyDifficulty;
            bountyType = bountyManager.activeBounties[0].bountyType;

            //generate a few random mods
            int randomAmount = UnityEngine.Random.Range(0, 2);
            for (int i = 0; i < randomAmount; i++){
                levelMods.Add((LevelMods)UnityEngine.Random.Range(1, Enum.GetNames(typeof(LevelMods)).Length));
            }

            UpdateMenu();
        }
        else{

        }
    }

    private void UpdateMenu()
    {
        if (teleportMenu == null) return;

        //get title
        TextMeshProUGUI title = teleportMenu.title.GetComponent<TextMeshProUGUI>();
        //get description
        TextMeshProUGUI description = teleportMenu.description.GetComponent<TextMeshProUGUI>();
        //get mods
        TextMeshProUGUI mods = teleportMenu.mods.GetComponent<TextMeshProUGUI>();
        //get button
        Button button = teleportMenu.startButton.GetComponent<Button>();
        //get button text
        TextMeshProUGUI buttonText = teleportMenu.startButton.GetComponentInChildren<TextMeshProUGUI>();

        if (bountyManager.activeBounties.Count > 0)
        {
            title.text = bountyManager.activeBounties[0].levelType.ToString();
            description.text = "A dangerous place(holder text)";
            mods.text = "MODS: ";
            if (levelMods.Count > 0)
            {
                foreach (LevelMods mod in levelMods)
                {
                    mods.text += mod.ToString() + " ";
                }
                mods.text = mods.text.Remove(mods.text.Length - 1);
            }
            else
            {
                mods.text += "NONE";
            }
            

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                sceneController.LoadLevel(bountyManager.activeBounties[0].levelType);
            });
            buttonText.text = "START";
            button.interactable = true;

        }
        else
        {
            title.text = "SELECT BOUNTY";
            description.text = "";
            mods.text = "";
            buttonText.text = "-ERROR-";
            button.interactable = false;
        }
    }

    private void LoadMenu()
    {
        teleportMenu = GameObject.FindObjectOfType<TeleportMenu>(true);
    }
}
