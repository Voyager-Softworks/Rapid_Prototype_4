using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [System.Serializable]
    public enum SpawnRotation
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    [System.Serializable]
    public struct SpawnCell
    {
        [SerializeField]
        public Vector3 position;
        [SerializeField]
        public List<SpawnRotation> rotations;

    }

    [System.Serializable]
    public struct ResourceNode
    {
        public GameObject prefab;
        public float probability;
        public bool mustBeUpright;
        public int minSpawns;
        public int maxSpawns;
    }
    [SerializeField]
    public ResourceSpawner_Data data;
    public List<ResourceNode> spawnables;
    Navmesh2D m_navmesh;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn()
    {
        int[] spawnCounts = new int[spawnables.Count];
        for (int i = 0; i < spawnables.Count; i++)
        {
            int spawnCount = Random.Range(spawnables[i].minSpawns, spawnables[i].maxSpawns);
            while (spawnCounts[i] < spawnCount)
            {
                SpawnCell cell = data.spawnCells[Random.Range(0, data.spawnCells.Count)];
                bool spawnSuccess = false;
                foreach (SpawnRotation rotation in cell.rotations)
                {
                    if (spawnSuccess) break;
                    GameObject newObj = null;
                    switch (rotation)
                    {
                        
                        case SpawnRotation.UP:
                            spawnSuccess = true;
                            newObj = Instantiate(spawnables[i].prefab, cell.position - (Vector3.up/2.0f), Quaternion.Euler(0, 0, 0));
                            spawnCounts[i]++;
                            break;
                        case SpawnRotation.DOWN:
                            if(!spawnables[i].mustBeUpright)
                            {
                                spawnSuccess = true;
                                spawnCounts[i]++;
                                newObj = Instantiate(spawnables[i].prefab, cell.position - (Vector3.down/2.0f), Quaternion.Euler(0, 0, 180));
                                break;
                            }
                            break;
                        case SpawnRotation.LEFT:
                        if(!spawnables[i].mustBeUpright)
                            {
                                spawnSuccess = true;
                                spawnCounts[i]++;
                                newObj = Instantiate(spawnables[i].prefab, cell.position - (Vector3.left/2.0f), Quaternion.Euler(0, 0, 90));
                                break;
                            }
                            break;
                        case SpawnRotation.RIGHT:
                        if(!spawnables[i].mustBeUpright)
                            {
                                spawnSuccess = true;
                                spawnCounts[i]++;
                                newObj = Instantiate(spawnables[i].prefab, cell.position - (Vector3.right/2.0f), Quaternion.Euler(0, 0, 270));
                                break;
                            }
                            break;
                    }
                    if(newObj != null)
                    {
                        //newObj.GetComponentInChildren<SpriteRenderer>().flipX = Random.Range(0, 2) == 0;//
                    }
                }
                
            }
        }
    }


    public void BakeSpawnPoints()
    {
        data.spawnCells = new List<SpawnCell>();
        m_navmesh = FindObjectOfType<Navmesh2D>();
        foreach (Navmesh2D.Cell cell in m_navmesh.m_data.m_grid)
        {
            if(!cell.m_traversable || cell.m_hazard) continue;
            SpawnCell spawnCell = new SpawnCell();
            spawnCell.position = cell.m_position;
            spawnCell.rotations = new List<SpawnRotation>();
            if (!m_navmesh.IsTraversible(cell.m_position, Vector3.up))
            {
                spawnCell.rotations.Add(SpawnRotation.DOWN);
            }
            if (!m_navmesh.IsTraversible(cell.m_position, Vector3.down))
            {
                spawnCell.rotations.Add(SpawnRotation.UP);
            }
            if (!m_navmesh.IsTraversible(cell.m_position, Vector3.left))
            {
                spawnCell.rotations.Add(SpawnRotation.RIGHT);
            }
            if (!m_navmesh.IsTraversible(cell.m_position, Vector3.right))
            {
                spawnCell.rotations.Add(SpawnRotation.LEFT);
            }
            if(spawnCell.rotations.Count > 0)
            {
                data.spawnCells.Add(spawnCell);
            }
            
        }
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        foreach (SpawnCell cell in data.spawnCells)
        {
            foreach (SpawnRotation rotation in cell.rotations)
            {
                Vector3 position = cell.position;
                switch (rotation)
                {
                    case SpawnRotation.UP:
                        position -= Vector3.up/2.0f;
                        break;
                    case SpawnRotation.DOWN:
                        position -= Vector3.down/2.0f;
                        break;
                    case SpawnRotation.LEFT:
                        position -= Vector3.left/2.0f;
                        break;
                    case SpawnRotation.RIGHT:
                        position -= Vector3.right/2.0f;
                        break;
                }
                Gizmos.DrawSphere(position, 0.1f);
            }
        }
    }
}
