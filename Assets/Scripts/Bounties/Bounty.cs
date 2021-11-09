using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Bounty
{
    public BountyManager.LevelType levelType = BountyManager.LevelType.WASTELAND;
    public BountyManager.BountyType bountyType = BountyManager.BountyType.KILL;
    public BountyManager.BountyStatus bountyStatus = BountyManager.BountyStatus.INACTIVE;
    public BountyManager.BountyDifficulty bountyDifficulty = BountyManager.BountyDifficulty.EASY;
    public Reward reward = null;
}