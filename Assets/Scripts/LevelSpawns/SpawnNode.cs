using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnNode : MonoBehaviour
{
    [System.Serializable]
    public struct Spawnable
    {
        [SerializeField]
        public GameObject prefab;
        [SerializeField]
        public float probability;
        [SerializeField]
        public int difficultyRating;

        [SerializeField]
        public bool mustBeGrounded;

        [SerializeField]
        public int maxSpawns;

    }
    public List<Spawnable> spawnables;

    public string m_path = "";
    Navmesh2D navmesh;
    
    public SpawnNode_Data data;
    public int difficultyPoints= 0;
    public float spawnRadius = 1f;
    
    float cellRadius;
    // Start is called before the first frame update
    void Start()
    {
        navmesh = FindObjectOfType<Navmesh2D>();
        cellRadius = navmesh.m_cellradius;
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegenerateSpawnPoints()
    {
        data.SetData(new List<Navmesh2D.Cell>(FindObjectOfType<Navmesh2D>().GetCellsInRadius(transform.position, spawnRadius)));
    }

    //Spawns random prefabs in random cells
    public void Spawn()
    {
        int currDifficulty = 0;
        int[] currSpawns = new int[spawnables.Count];
        for (int i = 0; i < data.Count; i++)
        {
            if(currDifficulty > difficultyPoints)
            {
                return;
            }
            float random = Random.Range(0f, 1f);
            float cumulativeProbability = 0f;
            int cell = Random.Range(0, data.Count);
            
            for (int j = 0; j < spawnables.Count; j++)
            {
                cumulativeProbability += spawnables[j].probability;
                if (random <= cumulativeProbability && 
                (!spawnables[j].mustBeGrounded || data[cell].m_walkable) &&
                (currSpawns[j] < spawnables[j].maxSpawns) &&
                currDifficulty + spawnables[j].difficultyRating <= difficultyPoints)
                {
                    Vector3 position = data[cell].m_position;
                    GameObject spawned = Instantiate(spawnables[j].prefab, position, Quaternion.identity);
                    currSpawns[j]++;
                    currDifficulty += spawnables[j].difficultyRating;
                    cell = Random.Range(0, data.Count);
                    break;
                }
            }
        }
    }



    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmos()
    {
        if (navmesh == null)
            navmesh = FindObjectOfType<Navmesh2D>();
        cellRadius = navmesh.m_cellradius;

        if (data == null)
        {
            data = UnityEngine.Resources.Load<SpawnNode_Data>("SpawnNode/" + SceneManager.GetActiveScene().name + "_" + gameObject.name + "_SpawnNodeData");
        }
        
        // Draw a box for every spawn point
        
        foreach (Navmesh2D.Cell cell in data)
        {
            Gizmos.color = Color.blue - new Color(0, 0, 0, 0.9f);
            Gizmos.DrawCube(cell.m_position, new Vector3(cellRadius * 2, cellRadius * 2, 0));
            if(cell.m_walkable)
            {
                Gizmos.color = Color.green - new Color(0, 0, 0, 0.9f);
                Gizmos.DrawCube(cell.m_position, new Vector3(cellRadius * 2, cellRadius * 2, 0));
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        
    }
}
