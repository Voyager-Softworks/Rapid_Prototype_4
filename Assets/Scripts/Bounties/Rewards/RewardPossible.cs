using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "RewardPossible", menuName = "Bounties/Rewards/RewardPossible", order = 1)]
public class RewardPossible : ScriptableObject {
    [Serializable]
    public class ResourceReward {
        public Resources.ResourceType resourceType;
        public int minAmount;
        public int maxAmount;
    }

    [Serializable]
    public class DifficultyReward {
        public BountyManager.BountyDifficulty difficulty = BountyManager.BountyDifficulty.EASY;
        public List<ResourceReward> resources = new List<ResourceReward>();
    }

    [SerializeField] public List<DifficultyReward> difficulties = new List<DifficultyReward>();

    #if UNITY_EDITOR
    [CustomEditor(typeof(RewardPossible))]
    public class RewardPossibleEditor : Editor {
        public override void OnInspectorGUI() {
            //DrawDefaultInspector();

            RewardPossible rewardPossible = (RewardPossible)target;

            // for each difficulty
            for (int i = 0; i < rewardPossible.difficulties.Count; i++) {
                // draw the difficulty
                BountyManager.BountyDifficulty difficulty = rewardPossible.difficulties[i].difficulty;
                //bold font
                EditorGUILayout.LabelField(difficulty.ToString(), EditorStyles.boldLabel);

                // for each resource
                for (int j = 0; j < rewardPossible.difficulties[i].resources.Count; j++) {
                    // draw the resource name, and editable min and max amounts on the same line
                    ResourceReward resourceReward = rewardPossible.difficulties[i].resources[j];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(resourceReward.resourceType.ToString(), GUILayout.Width(100));
                    EditorGUILayout.LabelField("Min:", GUILayout.Width(30));
                    resourceReward.minAmount = EditorGUILayout.IntField(resourceReward.minAmount, GUILayout.Width(30));
                    EditorGUILayout.LabelField("Max:", GUILayout.Width(30));
                    resourceReward.maxAmount = EditorGUILayout.IntField(resourceReward.maxAmount, GUILayout.Width(30));
                    EditorGUILayout.EndHorizontal();
                }

                //space
                EditorGUILayout.Space();
            }

            //if anythings changed, save the asset
            if (GUI.changed) {
                EditorUtility.SetDirty(rewardPossible);
            }
        }
    }

    public void Reset(){
        difficulties.Clear();

        //for each difficulty add a new difficulty
        for (int i = 0; i < Enum.GetNames(typeof(BountyManager.BountyDifficulty)).Length; i++) {
            DifficultyReward newDifficulty = new DifficultyReward();
            newDifficulty.difficulty = (BountyManager.BountyDifficulty)i;

            //for each resource add a new resource
            for (int j = 0; j < Enum.GetNames(typeof(Resources.ResourceType)).Length; j++) {
                //if resource is exotic, skip
                if (j == (int)Resources.ResourceType.EXOTIC) {
                    continue;
                }

                ResourceReward newResource = new ResourceReward();
                newResource.resourceType = (Resources.ResourceType)j;
                newResource.minAmount = 0;
                newResource.maxAmount = 0;

                newDifficulty.resources.Add(newResource);
            }

            difficulties.Add(newDifficulty);
        }

        //save
        EditorUtility.SetDirty(this);
    }
    #endif

    public Resources.PlayerResource GetRandomReward(BountyManager.BountyDifficulty difficulty) {
        //get the difficulty
        DifficultyReward rewardDifficulty = difficulties.Find(x => x.difficulty == difficulty);

        if (rewardDifficulty == null) {
            return null;
        }

        //get a random resource
        ResourceReward rewardResource = rewardDifficulty.resources[UnityEngine.Random.Range(0, rewardDifficulty.resources.Count)];

        //get a random amount
        int rewardAmount = UnityEngine.Random.Range(rewardResource.minAmount, rewardResource.maxAmount);

        Resources.PlayerResource reward = new Resources.PlayerResource();
        reward.type = rewardResource.resourceType;
        reward.amount = rewardAmount;

        //return the resource
        return reward;
    }      
}