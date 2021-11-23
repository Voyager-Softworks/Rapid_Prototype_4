using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{

    // [SerializeField]
    // public List<Upgrade> weaponUpgrades;
    // [SerializeField]
    // public List<Upgrade> moduleUpgrades;
    [SerializeField]
    public Resources.PlayerResource[] playerResources;
    [SerializeField]
    public List<Bounty> inactiveBounties;
    [SerializeField]
    public List<Bounty> activeBounties;
    [SerializeField]
    public List<Bounty> completedBounties;
    [SerializeField]
    public List<Bounty> failedBounties;
    [SerializeField]
    public List<PlayerStats.KillTracker> killList;
    [SerializeField]
    public int total_kills;
    [SerializeField]
    public int total_deaths;
    [SerializeField]
    public int townLevel;


}
