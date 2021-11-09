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
    public BountyBoard bountyBoard = null;

    [Header("Bounties")]
    public List<Bounty> inactiveBounties = new List<Bounty>(){};
    public List<Bounty> activeBounties = new List<Bounty>(){};
    public List<Bounty> completedBounties = new List<Bounty>(){};
    public List<Bounty> failedBounties = new List<Bounty>(){};
    public Bounty selectedBounty = null;

    [Header("Spawn Info")]
    public List<LevelSpawnInfo> spawnInfo;

    void Awake() {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!GetComponent<DontDestroy>().CheckValidInstance()) return;
        if (this == null) return;
        if (gameObject == null) return;

        selectedBounty = null;

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

        bountyBoard.infoAcceptButton.GetComponent<Button>().onClick.AddListener(AcceptBounty);
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
            List<Bounty> _bounties = new List<Bounty>(inactiveBounties);
            //add on the active bounties
            _bounties.AddRange(activeBounties);

            //check if there is a bounty for the button
            if (i < _bounties.Count)
            {
                //get the bounty
                Bounty bounty = _bounties[i];

                //add button listener
                _button.GetComponent<Button>().onClick.RemoveAllListeners();
                _button.GetComponent<Button>().onClick.AddListener( () => {
                    //update selected bounty
                    selectedBounty = (selectedBounty == bounty) ? null : bounty;
                    //update the board
                    UpdateBoard();
                });

                //set the button text
                foreach (TextMeshProUGUI _text in text)
                {
                    //check if name is "Type"
                    if (_text.transform.name.ToLower() == "type")
                    {
                        //set the text to the bounty type
                        _text.text = bounty.bountyType.ToString();
                    }

                    //check if name is "Diff"
                    if (_text.transform.name.ToLower() == "diff")
                    {
                        //set the text to the bounty difficulty
                        switch (bounty.bountyDifficulty)
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
                
                //if bounty is active, make green, otherwise white
                if (bounty.bountyStatus == BountyStatus.ACTIVE)
                {
                    _button.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    _button.GetComponent<Image>().color = Color.white;
                }

                //if this bounty is selected, make it darker
                if (selectedBounty == bounty)
                {
                    _button.GetComponent<Image>().color = _button.GetComponent<Image>().color * 0.75f;
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

        UpdateSelected();
    }

    void UpdateSelected(){
        if (selectedBounty != null){
            bountyBoard.infoPannel.SetActive(true);

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

            //if selected bounty is active
            if (selectedBounty.bountyStatus == BountyStatus.ACTIVE){
                //make accept button say "In Progress"
                bountyBoard.infoAcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "In Progress";
                //make the accept button inactive
                bountyBoard.infoAcceptButton.GetComponent<Button>().interactable = false;
            }
            else {
                //make accept button say "Accept"
                bountyBoard.infoAcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "Accept";
                //make the accept button active
                bountyBoard.infoAcceptButton.GetComponent<Button>().interactable = true;
            }
        }
        else{
            bountyBoard.infoPannel.SetActive(false);
        }
    }

    void AcceptBounty()
    {
        if (selectedBounty != null)
        {
            //check if bounty is already active, or in the active list, if so, do nothing
            if (selectedBounty.bountyStatus == BountyStatus.ACTIVE || activeBounties.Contains(selectedBounty))
            {
                return;
            }

            //remove the bounty from the inactive bounties
            inactiveBounties.Remove(selectedBounty);

            //add the bounty to the active bounties
            activeBounties.Add(selectedBounty);

            selectedBounty.bountyStatus = BountyStatus.ACTIVE;

            //update the board
            UpdateBoard();
        }
    }

    void Update()
    {
        
    }
}