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
            Start();
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

    public class Condition_Kills : Condition
    {
        //constructor
        public Condition_Kills(EnemyTest.EnemyType _type, int _amount)
        {
            enemyType = _type;
            targetValue = _amount;

            //Start();
        }

        public EnemyTest.EnemyType enemyType = EnemyTest.EnemyType.NONE;
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

        public override bool CheckComplete()
        {
            GameObject stats = GameObject.Find("Persistent");
            if (stats){
                PlayerStats ps = stats.GetComponent<PlayerStats>();
                if (ps){
                    int kills = ps.GetKills(enemyType, false);
                    kills += ps.GetKills(enemyType, true);

                    if (kills - startKills >= targetValue){
                        isComplete = true;
                    }
                }
            }
            return isComplete;
        }
    }


    public BountyManager.LevelType levelType = BountyManager.LevelType.WASTELAND;
    public BountyManager.BountyType bountyType = BountyManager.BountyType.KILL;
    public BountyManager.BountyStatus bountyStatus = BountyManager.BountyStatus.INACTIVE;
    public BountyManager.BountyDifficulty bountyDifficulty = BountyManager.BountyDifficulty.EASY;
    public List<Resources.PlayerResource> bountyPenalty = new List<Resources.PlayerResource>(){};
    public Reward reward = null;

    public List<Condition> conditions = new List<Condition>(){};
}