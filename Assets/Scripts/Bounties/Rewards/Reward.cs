using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reward", menuName = "Bounties/Rewards/Reward", order = 1)]
public class Reward : ScriptableObject
{
    public List<Resources.PlayerResource> resources = new List<Resources.PlayerResource>(){};
    public List<int> unlocks = new List<int>(){};
}
