using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelSpawnInfo", menuName = "LevelMods/LevelSpawnInfo", order = 1)]
public class LevelSpawnInfo : ScriptableObject
{
    [Serializable]
    public struct Spawn {
        [SerializeField] public GameObject prefab;
        [SerializeField] public float ratioAmount;
    }

    [Header("Level")]
    [SerializeField] public LevelManager.LevelType levelType;

    [Header("Default Spawns")]
    [SerializeField] float defaultMinAmount;
    [SerializeField] float defaultMaxAmount;
    [SerializeField] public List<Spawn> defaultSpawns;

    [Header("Swarm Spawns")]
    [SerializeField] float swarmMinAmount;
    [SerializeField] float swarmMaxAmount;
    [SerializeField] public List<Spawn> swarmSpawns;

    [Header("Elite Spawns")]
    [SerializeField] float eliteMinAmount;
    [SerializeField] float eliteMaxAmount;
    [SerializeField] public List<Spawn> eliteSpawns;
}
