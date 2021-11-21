using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceSpawner_Data : ScriptableObject
{
    [SerializeField]
    public List<ResourceSpawner.SpawnCell> spawnCells;
}

