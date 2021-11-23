using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class BountyManager : MonoBehaviour
{

    public enum BountyType {
        COLLECT,
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
    private LevelManager levelManager = null;
    private IconReference iconReference = null;
    private PlayerStats playerStats = null;

    [Header("Board")]
    public BountyBoard bountyBoard = null;

    [Header("Bounties")]
    public List<Bounty> inactiveBounties = new List<Bounty>(){};
    public List<Bounty> activeBounties = new List<Bounty>(){};
    public List<Bounty> completedBounties = new List<Bounty>(){};
    public List<Bounty> failedBounties = new List<Bounty>(){};
    public Bounty selectedBounty = null;
    public RewardPossible rewardPossible = null;

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
        if (levelManager == null) levelManager = GetComponent<LevelManager>();
        if (iconReference == null) iconReference = GetComponent<IconReference>();
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();

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

        if (bountyBoard){
            Button button = bountyBoard.refreshButton.GetComponent<Button>();
            //bind refresh button to clear then generate bounties
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                selectedBounty = null;
                inactiveBounties.Clear();
                // Load the bounties
                LoadBounties();
                UpdateBoard();
            });
        }
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
            if (bounty == null) {
                i--;
                continue;
            }

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
        bounty.levelType = (LevelManager.LevelType)Random.Range(0, System.Enum.GetValues(typeof(LevelManager.LevelType)).Length);

        //get spawn info for this level type
        LevelSpawnInfo spawnInfo = GetSpawnInfo(bounty.levelType);
        if (spawnInfo == null) {
            return null;
        }

        //get a random bounty type
        bounty.bountyType = (BountyType)Random.Range(0, System.Enum.GetValues(typeof(BountyType)).Length);
        if (spawnInfo.bossSpawns.Count <= 0 && bounty.bountyType == BountyType.BOSS) {
            return null;
        }

        //get a random bounty difficulty
        bounty.bountyDifficulty = (BountyDifficulty)Random.Range(0, System.Enum.GetValues(typeof(BountyDifficulty)).Length);

        //diff multiplier
        float diffMult = ((int)bounty.bountyDifficulty) + 1;

        //random cost
        //if not wasteland, add scrap cost
        if (bounty.levelType != LevelManager.LevelType.WASTELAND) {
            Resources.PlayerResource scrapCost2 = new Resources.PlayerResource();
            scrapCost2.type = Resources.ResourceType.SCRAP;
            scrapCost2.amount = (int)(Random.Range(diffMult, diffMult * 2));
            bounty.bountyCost.Add(scrapCost2);
        }

        //random reward
        bounty.reward = new Reward();
        //random reward using reward possible
        Resources.PlayerResource rewardResources = rewardPossible.GetRandomReward(bounty.bountyDifficulty);
        if (rewardResources != null) {
            bounty.reward.resources.Add(rewardResources);
        }


        //add condition
        switch (bounty.bountyType)
        {
            case BountyType.COLLECT:
                //get the resource type to collect from the spawn info
                Resources.ResourceType resourceType = spawnInfo.regularResources[Random.Range(0, spawnInfo.regularResources.Count)];
                //get amount to collect
                int resourceAmount = (int)(Random.Range(1, 10) * diffMult);
                //make
                Bounty.Condition_Collect collect = new Bounty.Condition_Collect(resourceType, resourceAmount);
                //add
                bounty.conditions.Add(collect);
            break;
            case BountyType.KILL: 
                //get type of enemy to kill using spawn info
                EnemyTest.EnemyType enemyType = spawnInfo.regularSpawns[Random.Range(0, spawnInfo.regularSpawns.Count)].GetComponent<EnemyTest>().m_enemyType;
                //get amount
                int amount = (int)(Random.Range(1, 10) * diffMult);
                //make
                Bounty.Condition_Kills kills = new Bounty.Condition_Kills(enemyType, amount);
                //add
                bounty.conditions.Add(kills);
            break;
            case BountyType.ELITE:
                //get type
                EnemyTest.EnemyType eliteType = spawnInfo.regularSpawns[Random.Range(0, spawnInfo.regularSpawns.Count)].GetComponent<EnemyTest>().m_enemyType;
                //get amount
                int eliteAmount = (int)(Random.Range(1, 5) * diffMult);
                //make
                Bounty.Condition_Kills eliteKills = new Bounty.Condition_Kills(eliteType, eliteAmount, true);
                bounty.conditions.Add(eliteKills);
            break;
            case BountyType.BOSS:
                //get type
                EnemyTest.EnemyType bossType = spawnInfo.bossSpawns[Random.Range(0, spawnInfo.bossSpawns.Count)].GetComponent<EnemyTest>().m_enemyType;
                //get amount
                int bossAmount = 1;
                Bounty.Condition_Kills bossKills = new Bounty.Condition_Kills(bossType, bossAmount);
                bounty.conditions.Add(bossKills);
            break;
        }

        return bounty;
    }

    private LevelSpawnInfo GetSpawnInfo(LevelManager.LevelType levelType)
    {
        foreach (LevelSpawnInfo spawnInfo in this.spawnInfo)
        {
            if (spawnInfo.levelType == levelType)
            {
                return spawnInfo;
            }
        }
        return null;
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

            //update the image of the selected bounty
            switch (selectedBounty.bountyType)
            {
                case BountyType.COLLECT:
                    //cast to collect
                    Bounty.Condition_Collect collect = (Bounty.Condition_Collect)selectedBounty.conditions[0];
                    //update icon
                    bountyBoard.icon.GetComponentInChildren<Image>().sprite = iconReference.GetIcon(collect.itemType).icon;
                    //update amount
                    int currentAmount = resourceManager.GetResourceCount(collect.itemType);
                    int neededAmount = collect.targetAmount;
                    bountyBoard.icon.GetComponentInChildren<TextMeshProUGUI>().text = currentAmount + "/" + neededAmount;
                    break;
                case BountyType.KILL:
                    //cast the condition to kills
                    Bounty.Condition_Kills killCond = (Bounty.Condition_Kills)selectedBounty.conditions[0];
                    //update icon
                    bountyBoard.icon.GetComponentInChildren<Image>().sprite = iconReference.GetIcon(killCond.enemyType).icon;
                    //update amount
                    int totalKills = playerStats.GetKills(killCond.enemyType, false);
                    totalKills += playerStats.GetKills(killCond.enemyType, true);
                    int killsCompleted = totalKills - killCond.startKills;
                    int killsNeeded = killCond.targetValue;
                    if (selectedBounty.bountyStatus == BountyStatus.INACTIVE)
                    {
                        killsCompleted = 0;
                    }
                    bountyBoard.icon.GetComponentInChildren<TextMeshProUGUI>().text = killsCompleted.ToString() + "/" + killsNeeded.ToString();
                    break;
                case BountyType.ELITE:
                    //cast the condition to elite kills
                    Bounty.Condition_Kills eliteCond = (Bounty.Condition_Kills)selectedBounty.conditions[0];
                    //update icon
                    bountyBoard.icon.GetComponentInChildren<Image>().sprite = iconReference.GetIcon(eliteCond.enemyType).icon;
                    //update amount
                    int totalEliteKills = playerStats.GetKills(eliteCond.enemyType, true);
                    int eliteKillsCompleted = totalEliteKills - eliteCond.startKills;
                    int eliteKillsNeeded = eliteCond.targetValue;
                    bountyBoard.icon.GetComponentInChildren<TextMeshProUGUI>().text = eliteKillsCompleted.ToString() + "/" + eliteKillsNeeded.ToString();
                    break;
                case BountyType.BOSS:
                    //cast the condition to boss kills
                    Bounty.Condition_Kills bossCond = (Bounty.Condition_Kills)selectedBounty.conditions[0];
                    //update icon
                    bountyBoard.icon.GetComponentInChildren<Image>().sprite = iconReference.GetIcon(bossCond.enemyType).icon;
                    //update amount
                    int totalBossKills = playerStats.GetKills(bossCond.enemyType, false);
                    totalBossKills += playerStats.GetKills(bossCond.enemyType, true);
                    int bossKillsCompleted = totalBossKills - bossCond.startKills;
                    int bossKillsNeeded = bossCond.targetValue;
                    bountyBoard.icon.GetComponentInChildren<TextMeshProUGUI>().text = bossKillsCompleted.ToString() + "/" + bossKillsNeeded.ToString();
                    break;
            }


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
            
            //generate the cost test
            string costText = "COST:\n";
            foreach (Resources.PlayerResource resource in selectedBounty.bountyCost)
            {
                costText += resource.amount + " " + resource.type.ToString() + ", ";
            }
            if (costText == "COST:\n")
            {
                costText = "COST:\nNONE";
            }
            else
            {
                //remove the last character
                costText = costText.Remove(costText.Length - 2);
            }
            bountyBoard.infoCost.GetComponent<TextMeshProUGUI>().text = costText;

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

            if (!resourceManager.TryConsumeResources(selectedBounty.bountyCost))
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

            if (levelManager) levelManager.GenLevel();
            
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

                //Remove from active
                activeBounties.Remove(selectedBounty);

                //Add to failed
                failedBounties.Add(selectedBounty);

                selectedBounty.bountyStatus = BountyStatus.FAILED;
            }

            selectedBounty = null;

            //update the board
            UpdateBoard();
        }

        if (levelManager) levelManager.UpdateMenu();
    }

    void CollectBounty()
    {
        if (selectedBounty != null)
        {
            //check if bounty is already active, or in the active list, if so, complete
            if (selectedBounty.bountyStatus == BountyStatus.COLLECT)
            {
                switch (selectedBounty.bountyType)
                {
                    case BountyType.BOSS:
                        //nothing
                    break;
                    case BountyType.COLLECT:
                        //cast the bounty as a collect bounty
                        Bounty.Condition_Collect collectCond = (Bounty.Condition_Collect)selectedBounty.conditions[0];
                        //make list of resources to collect
                        List<Resources.PlayerResource> toCollectList = new List<Resources.PlayerResource>();
                        //add each resource to the list
                        Resources.ResourceType type = collectCond.itemType;
                        int amount = collectCond.targetAmount;
                        Resources.PlayerResource resource = new Resources.PlayerResource();
                        resource.type = type;
                        resource.amount = amount;
                        toCollectList.Add(resource);

                        if (!resourceManager.TryConsumeResources(toCollectList)){
                            //update the board
                            UpdateBoard();
                            return;
                        }
                    break;
                    case BountyType.KILL:
                        //nothing
                    break;
                }

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

        if (levelManager) levelManager.UpdateMenu();
    }

    void Update()
    {
        CheckConditions();
    }

    public void CheckConditions(){
        //check all active bounty conditions
        foreach (Bounty bounty in activeBounties)
        {
            if (bounty.conditions.Count <= 0){
                //if no conditions, complete the bounty
                bounty.bountyStatus = BountyStatus.COLLECT;
                //update the board
                UpdateBoard();
            }
            else {
                bool allComplete = true;
                foreach (Bounty.Condition condition in bounty.conditions)
                {
                    //if condition is met, set bounty to complete
                    if (condition.CheckComplete())
                    {
                    }
                    else {
                        allComplete = false;
                    }

                }

                if (allComplete)
                {
                    bounty.bountyStatus = BountyStatus.COLLECT;
                }
                else{
                    bounty.bountyStatus = BountyStatus.ACTIVE;
                }

                //update the board
                if (bountyBoard) UpdateBoard();
            }
        }
    }
}