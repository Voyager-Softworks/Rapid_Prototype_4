using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Bounty
{
    [Serializable]
    public class Condition
    {
        //constructor
        public Condition()
        {
            //Start();
        }

        public bool isComplete = false;
        virtual public void Start(){
            Debug.Log("Condition Start");
            isComplete = false;
        }
        virtual public bool CheckComplete()
        {
            Debug.Log("Condition IsSatisfied");
            return isComplete;
        }
    }

    [Serializable]
    public class Condition_Kills : Condition
    {
        //constructor
        public Condition_Kills(EnemyTest.EnemyType _type, int _amount, bool _isEliteOnly = false)
        {
            enemyType = _type;
            targetValue = _amount;
            isEliteOnly = _isEliteOnly;

            //Start();
        }

        public EnemyTest.EnemyType enemyType = EnemyTest.EnemyType.NONE;
        public bool isEliteOnly = false;
        public int startKills = 0;
        public int targetValue = 0;

        public override void Start()
        {
            base.Start();
            GameObject stats = GameObject.Find("Persistent");
            if (stats){
                PlayerStats ps = stats.GetComponent<PlayerStats>();
                if (ps){
                    startKills = ps.GetKills(enemyType, false);
                    startKills += ps.GetKills(enemyType, true);
                }
            }
        }

        public int GetCompletedKills()
        {
            GameObject stats = GameObject.Find("Persistent");
            if (stats){
                PlayerStats ps = stats.GetComponent<PlayerStats>();
                if (ps){
                    int totalKills = ps.GetKills(enemyType, false);
                    totalKills += ps.GetKills(enemyType, true);
                    int killsCompleted = totalKills - startKills;
                    return killsCompleted;
                }
            }
            return 0;
        }

        public int GetTargetKills()
        {
            return targetValue;
        }

        public override bool CheckComplete()
        {
            GameObject pers = GameObject.Find("Persistent");
            if (pers){
                PlayerStats ps = pers.GetComponent<PlayerStats>();
                if (ps){
                    int kills = 0;
                    if (!isEliteOnly) kills += ps.GetKills(enemyType, false);
                    kills += ps.GetKills(enemyType, true);

                    if (kills - startKills >= targetValue){
                        isComplete = true;
                    }
                }
            }
            return isComplete;
        }
    }

    [Serializable]
    public class Condition_Collect : Condition 
    {
        //constructor
        public Condition_Collect(Resources.ResourceType _type, int _amount)
        {
            itemType = _type;
            targetAmount = _amount;

            //Start();
        }

        public Resources.ResourceType itemType = Resources.ResourceType.SCRAP;
        public int targetAmount = 0;

        public override void Start()
        {
            base.Start();
        }

        public int GetCompletedAmount()
        {
            GameObject pers = GameObject.Find("Persistent");
            if (pers){
                Resources rcrs = pers.GetComponent<Resources>();
                if (rcrs){
                    return rcrs.GetResourceCount(itemType);
                }
            }
            return 0;
        }

        public int GetTargetAmount()
        {
            return targetAmount;
        }

        public override bool CheckComplete()
        {
            GameObject pers = GameObject.Find("Persistent");
            if (pers) {
                Resources rec = pers.GetComponent<Resources>();
                if (rec) {
                    int amount = rec.GetResourceCount(itemType);
                    if (amount >= targetAmount){
                        isComplete = true;
                    }
                    else{
                        isComplete = false;
                    }
                }
            }
            return isComplete;
        }
    }


    public LevelManager.LevelType levelType = LevelManager.LevelType.WASTELAND;
    public BountyManager.BountyType bountyType = BountyManager.BountyType.KILL;
    public BountyManager.BountyStatus bountyStatus = BountyManager.BountyStatus.INACTIVE;
    public BountyManager.BountyDifficulty bountyDifficulty = BountyManager.BountyDifficulty.EASY;
    public List<Resources.PlayerResource> bountyCost = new List<Resources.PlayerResource>(){};
    public Reward reward = null;

    public List<Condition> conditions = new List<Condition>(){};
}