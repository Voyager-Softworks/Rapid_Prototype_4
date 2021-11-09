using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class BountyManager : MonoBehaviour
{

    public enum LevelType {
        HUB,
        WASTELAND,
        CAVES,
        VOLCANO,
        RUINS
    }

    public enum BountyType {
        FIND,
        KILL,
        ELITE
    }

    public enum BountyStatus {
        INACTIVE,
        ACTIVE,
        COMPLETE,
        FAILED
    }

    public enum BountyDifficulty {
        EASY,
        MEDIUM,
        HARD
    }

    [Header("Board")]
    public BountyBoard bountyBoard;

    [Header("Bounties")]
    public List<Bounty> inactiveBounties;
    public List<Bounty> activeBounties;
    public List<Bounty> completedBounties;
    public List<Bounty> failedBounties;
    public Bounty selectedBounty;

    [Header("Spawn Info")]
    public List<LevelSpawnInfo> spawnInfo;

    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level_Hub")
        {
            // Load the board
            LoadBoard();

            // Load the bounties
            LoadBounties();

            // Update the board with the bounties
            UpdateBoard();

            //Update selected bounty
            UpdateSelected();
        }
    }

    void LoadBoard()
    {
        bountyBoard = GameObject.Find("BountyBoard").GetComponent<BountyBoard>();
    }

    void LoadBounties()
    {
        int bountyCount = activeBounties.Count + inactiveBounties.Count;
        int maxBounties = bountyBoard.missionButtons.Count;

        //fill the bounty board bounties
        for (int i = bountyCount; i < maxBounties; i++)
        {
            //Gen new Bounty
            Bounty bounty = GenNewBounty();

            //add it to the possible bounties
            inactiveBounties.Add(bounty);
        }

        //sort the inactive bounties by difficulty
        inactiveBounties.Sort(delegate (Bounty x, Bounty y) {
            return x.bountyDifficulty.CompareTo(y.bountyDifficulty);
        });
    }

    Bounty GenNewBounty()
    {
        Bounty bounty = new Bounty();

        //get a random level type
        bounty.levelType = (LevelType)Random.Range(0, System.Enum.GetValues(typeof(LevelType)).Length);

        //get a random bounty type
        bounty.bountyType = (BountyType)Random.Range(0, System.Enum.GetValues(typeof(BountyType)).Length);

        //get a random bounty difficulty
        bounty.bountyDifficulty = (BountyDifficulty)Random.Range(0, System.Enum.GetValues(typeof(BountyDifficulty)).Length);

        //random reward
        bounty.reward = new Reward();
        Resources.PlayerResource scrap = new Resources.PlayerResource();
        scrap.type = Resources.ResourceType.SCRAP;
        scrap.amount = Random.Range(1, 10);
        bounty.reward.resources.Add(scrap);

        bounty.reward.unlocks = new List<int>(){1};

        return bounty;
    }

    void UpdateBoard()
    {
        //loop through all the mission buttons, and update them with the bounties
        for (int i = 0; i < bountyBoard.missionButtons.Count; i++)
        {
            GameObject _button = bountyBoard.missionButtons[i];
            TextMeshProUGUI[] text = _button.GetComponentsInChildren<TextMeshProUGUI>();

            //get inactive bounties
            List<Bounty> _bounties = inactiveBounties;
            //add on the active bounties
            _bounties.AddRange(activeBounties);

            //check if there is a bounty for the button
            if (i < _bounties.Count)
            {
                //get the bounty
                Bounty bounty = _bounties[i];

                //add button listener
                _button.GetComponent<Button>().onClick.AddListener( () => {
                    //update selected bounty
                    selectedBounty = bounty;
                    //update the board
                    UpdateSelected();
                });

                //set the button text
                foreach (TextMeshProUGUI _text in text)
                {
                    //check if name is "Type"
                    if (_text.transform.name.ToLower() == "type")
                    {
                        //set the text to the bounty type
                        _text.text = _bounties[i].bountyType.ToString();
                    }

                    //check if name is "Diff"
                    if (_text.transform.name.ToLower() == "diff")
                    {
                        //set the text to the bounty difficulty
                        switch (_bounties[i].bountyDifficulty)
                        {
                            case BountyDifficulty.EASY:
                                _text.text = "*";
                                break;
                            case BountyDifficulty.MEDIUM:
                                _text.text = "**";
                                break;
                            case BountyDifficulty.HARD:
                                _text.text = "***";
                                break;
                        }
                    }
                }

                //set the button to active
                _button.SetActive(true);
            }
            else
            {
                //set the button to inactive
                _button.SetActive(false);
            }
        }

        if (inactiveBounties.Count > 0){
            selectedBounty = inactiveBounties[0];
        }
        else if (activeBounties.Count > 0){
            selectedBounty = activeBounties[0];
        }
        else {
            selectedBounty = null;
        }
    }

    void UpdateSelected(){
        if (selectedBounty != null){
            bountyBoard.infoTitle.GetComponent<TextMeshProUGUI>().text = selectedBounty.bountyType.ToString();
            bountyBoard.infoLevel.GetComponent<TextMeshProUGUI>().text = selectedBounty.levelType.ToString();
            //generate the reward text
            string rewardText = "";
            foreach (Resources.PlayerResource resource in selectedBounty.reward.resources)
            {
                rewardText += resource.amount + " " + resource.type.ToString() + ", ";
            }
            foreach (int unlock in selectedBounty.reward.unlocks)
            {
                rewardText += unlock + ", ";
            }
            bountyBoard.infoReward.GetComponent<TextMeshProUGUI>().text = rewardText;
        }
    }

    void Update()
    {
        
    }
}