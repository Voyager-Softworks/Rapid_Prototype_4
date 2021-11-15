using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnNode_Data : ScriptableObject
{
    [SerializeField]
    List<Navmesh2D.Cell> spawnPoints;
    public Navmesh2D.Cell this[int index]{ get { return spawnPoints[index] ; } set { spawnPoints[index] = value; } }

    public void SetData(List<Navmesh2D.Cell> data)
    {
        spawnPoints = data;
    }

    public List<Navmesh2D.Cell>.Enumerator GetEnumerator()
    {
        return spawnPoints.GetEnumerator();
    }

    public int Count
    {
        get { return spawnPoints.Count; }
    }

}
