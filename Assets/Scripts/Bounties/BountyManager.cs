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
        ELITE,
        BOSS
    }

    public enum BountyStatus {
        INACTIVE,
        ACTIVE,
        COLLECT,
        COMPLETE,
        FAILED
    }

    public enum BountyDifficulty {
        EASY,
        MEDIUM,
        HARD
    }

    private Resources resourceManager = null;

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

    int maxActiveBounties = 1;




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
        if (resourceManager == null) resourceManager = GetComponent<Resources>();

        //if any active, select them automatically
        if (activeBounties.Count > 0) {
            selectedBounty = activeBounties[0];
        }
        else{
            selectedBounty = null;
        }

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
        bountyBoard = GameObject.FindObjectOfType<BountyBoard>(true);
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

        //diff multiplier
        float diffMult = ((int)bounty.bountyDifficulty) + 1;

        //random penalty
        //create and add organic penalty
        Resources.PlayerResource organicPenalty = new Resources.PlayerResource();
        organicPenalty.type = Resources.ResourceType.ORGANIC;
        organicPenalty.amount = (int)(Random.Range(1, 10) * diffMult);
        bounty.bountyPenalty.Add(organicPenalty);

        //random reward
        bounty.reward = new Reward();
        //create and add scrap reward
        Resources.PlayerResource scrapReward = new Resources.PlayerResource();
        scrapReward.type = Resources.ResourceType.SCRAP;
        scrapReward.amount = (int)(Random.Range(1, 10) * diffMult);
        bounty.reward.resources.Add(scrapReward);
        //create and add organic reward
        Resources.PlayerResource organicReward = new Resources.PlayerResource();
        organicReward.type = Resources.ResourceType.ORGANIC;
        organicReward.amount = (int)(Random.Range(1, 10) * diffMult);
        bounty.reward.resources.Add(organicReward);
        //create and add power reward
        Resources.PlayerResource powerReward = new Resources.PlayerResource();
        powerReward.type = Resources.ResourceType.POWER;
        powerReward.amount = (int)(Random.Range(1, 10) * diffMult);
        bounty.reward.resources.Add(powerReward);


        //add condition
        switch (bounty.bountyType)
        {
            case BountyType.FIND:
                //add condition
            break;
            case BountyType.KILL: 
                //add condition
                Bounty.Condition_Kills kills = new Bounty.Condition_Kills(EnemyTest.EnemyType.BANDIT_GRUNT, 4);
                bounty.conditions.Add(kills);
            break;
            case BountyType.ELITE:
                //add condition
                Bounty.Condition_Kills eliteKills = new Bounty.Condition_Kills(EnemyTest.EnemyType.BANDIT_HOVER, 2);
                bounty.conditions.Add(eliteKills);
            break;
            case BountyType.BOSS:
                //add condition
                Bounty.Condition_Kills bossKills = new Bounty.Condition_Kills(EnemyTest.EnemyType.BANDIT_TANK, 1);
                bounty.conditions.Add(bossKills);
            break;
        }

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
                    _button.GetComponent<Image>().color = Color.yellow;
                }
                else if (bounty.bountyStatus == BountyStatus.COLLECT) {
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
            bountyBoard.infoLevel.GetComponent<TextMeshProUGUI>().text = "LEVEL:\n" + selectedBounty.levelType.ToString();
            //generate the reward text
            string rewardText = "REWARD:\n";
            foreach (Resources.PlayerResource resource in selectedBounty.reward.resources)
            {
                rewardText += resource.amount + " " + resource.type.ToString() + ", ";
            }
            foreach (int unlock in selectedBounty.reward.unlocks)
            {
                rewardText += unlock + ", ";
            }
            //remove the last character
            rewardText = rewardText.Remove(rewardText.Length - 2);
            bountyBoard.infoReward.GetComponent<TextMeshProUGUI>().text = rewardText;
            
            //generate the penalty test
            string penaltyText = "PENALTY:\n";
            foreach (Resources.PlayerResource resource in selectedBounty.bountyPenalty)
            {
                penaltyText += resource.amount + " " + resource.type.ToString() + ", ";
            }
            //remove the last character
            penaltyText = penaltyText.Remove(penaltyText.Length - 2);
            bountyBoard.infoPenalty.GetComponent<TextMeshProUGUI>().text = penaltyText;

            // Add accept listener
            bountyBoard.infoAcceptButton.GetComponent<Button>().onClick.RemoveAllListeners();

            //if selected bounty is active
            if (selectedBounty.bountyStatus == BountyStatus.ACTIVE){
                //make accept button say "active"
                bountyBoard.infoAcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "ABANDON";

                //add decline listener
                bountyBoard.infoAcceptButton.GetComponent<Button>().onClick.AddListener(DeclineBounty);

                //make button red
                bountyBoard.infoAcceptButton.GetComponent<Image>().color = Color.red;

                //enable button
                bountyBoard.infoAcceptButton.GetComponent<Button>().interactable = true;
            }
            else if (selectedBounty.bountyStatus == BountyStatus.COLLECT) {
                //make the accept button say "COLLECT"
                bountyBoard.infoAcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "COLLECT";

                //add collect listener
                bountyBoard.infoAcceptButton.GetComponent<Button>().onClick.AddListener(CollectBounty);

                //make button green
                bountyBoard.infoAcceptButton.GetComponent<Image>().color = Color.green;

                //enable button
                bountyBoard.infoAcceptButton.GetComponent<Button>().interactable = true;
            }
            else {
                //if number of active bounties is greater than the max
                if (activeBounties.Count >= maxActiveBounties){
                    //make accept button say "full"
                    bountyBoard.infoAcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "FULL";

                    //make button grey
                    bountyBoard.infoAcceptButton.GetComponent<Image>().color = Color.grey;

                    //disable button
                    bountyBoard.infoAcceptButton.GetComponent<Button>().interactable = false;
                }
                else {
                    //make accept button say "accept"
                    bountyBoard.infoAcceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "ACCEPT";

                    //add accept listener
                    bountyBoard.infoAcceptButton.GetComponent<Button>().onClick.AddListener(AcceptBounty);

                    //make button green
                    bountyBoard.infoAcceptButton.GetComponent<Image>().color = Color.green;

                    //enable button
                    bountyBoard.infoAcceptButton.GetComponent<Button>().interactable = true;
                }
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

            //if amount of active bounties is greater than the max, do nothing
            if (activeBounties.Count >= maxActiveBounties)
            {
                return;
            }

            //remove the bounty from the inactive bounties
            inactiveBounties.Remove(selectedBounty);

            //add the bounty to the active bounties
            activeBounties.Add(selectedBounty);

            selectedBounty.bountyStatus = BountyStatus.ACTIVE;

            //start all conditions
            foreach (Bounty.Condition condition in selectedBounty.conditions)
            {
                condition.Start();
            }

            //update the board
            UpdateBoard();
        }
    }

    void DeclineBounty()
    {
        if (selectedBounty != null)
        {
            //check if bounty is already active, or in the active list, if so, retire
            if (selectedBounty.bountyStatus == BountyStatus.ACTIVE || activeBounties.Contains(selectedBounty))
            {

                //check if the player has enough resources to abandon the bounty
                if (resourceManager.TryConsumeResources(selectedBounty.bountyPenalty))
                {
                    //Remove from active
                    activeBounties.Remove(selectedBounty);

                    //Add to failed
                    failedBounties.Add(selectedBounty);

                    selectedBounty.bountyStatus = BountyStatus.FAILED;
                }
            }

            selectedBounty = null;

            //update the board
            UpdateBoard();
        }
    }

    void CollectBounty()
    {
        if (selectedBounty != null)
        {
            //check if bounty is already active, or in the active list, if so, complete
            if (selectedBounty.bountyStatus == BountyStatus.COLLECT)
            {
                //Remove from active
                activeBounties.Remove(selectedBounty);

                //Add to completed
                completedBounties.Add(selectedBounty);

                selectedBounty.bountyStatus = BountyStatus.COMPLETE;
            }

            //add resources
            resourceManager.AddResources(selectedBounty.reward.resources);

            selectedBounty = null;

            //update the board
            UpdateBoard();
        }
    }

    void Update()
    {
        CheckConditions();
    }

    public void CheckConditions(){
        //check all active bounty conditions
        foreach (Bounty bounty in activeBounties)
        {
            //if collected or complete, skip
            if (bounty.bountyStatus == BountyStatus.COLLECT || bounty.bountyStatus == BountyStatus.COMPLETE)
            {
                continue;
            }

            if (bounty.conditions.Count <= 0){
                //if no conditions, complete the bounty
                bounty.bountyStatus = BountyStatus.COLLECT;
                //update the board
                UpdateBoard();
            }
            else{
                foreach (Bounty.Condition condition in bounty.conditions)
                {
                    //if condition is met, set bounty to complete
                    if (condition.CheckComplete())
                    {
                        bounty.bountyStatus = BountyStatus.COLLECT;
                        //update the board
                        UpdateBoard();
                    }
                }
            }
        }
    }
}