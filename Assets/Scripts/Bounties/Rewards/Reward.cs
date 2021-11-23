using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reward
{
    public List<Resources.PlayerResource> resources = new List<Resources.PlayerResource>(){};
    public List<int> unlocks = new List<int>(){};
}