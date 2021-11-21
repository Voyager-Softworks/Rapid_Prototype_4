using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelSpawnInfo", menuName = "LevelMods/LevelSpawnInfo", order = 1)]
public class LevelSpawnInfo : ScriptableObject
{

    [Header("Level")]
    [SerializeField] public LevelManager.LevelType levelType;

    [Header("Spawns")]
    [SerializeField] public List<GameObject> regularSpawns;
    [SerializeField] public List<GameObject> bossSpawns;
    [SerializeField] public List<GameObject> findSpawns;
}
