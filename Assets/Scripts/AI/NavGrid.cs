// Bachelor of Software Engineering
// Media Design School
// Auckland
// New Zealand
//
// (c) 2021 Media Design School
//
// File Name   : NavGrid.cs
// Description : Flowfield-esque pathfinding implementation.
// Author      : Nerys Thamm
// Mail        : nerys.thamm@mds.ac.nz

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The nav grid.
/// </summary>

public class NavGrid : MonoBehaviour
{
    public int m_frameInterval = 1;
    int framecount = 0;
    public bool makeflowfield = false;

    /// <summary>
    /// The cell.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="_position">The _position.</param>
        /// <param name="_index">The _index.</param>
        public Cell(Vector3 _position, Vector2 _index)
        {
            m_distance = 255;
            m_traversable = true;
            m_walkable = false;
            m_direction = Vector2.zero;
            m_position = _position;
            m_index = _index;
        }

        /// <summary>
        /// Sets the distance.
        /// </summary>
        /// <param name="_distance">The _distance.</param>
        public void SetDistance(float _distance) => m_distance = _distance;

        /// <summary>
        /// Sets the traversable.
        /// </summary>
        /// <param name="_traversable">If true, _traversable.</param>
        public void SetTraversable(bool _traversable) => m_traversable = _traversable;

        public float m_distance;
        public bool m_traversable;
        public bool m_walkable;
        public Vector2 m_index;
        public Vector2 m_direction;
        public Vector3 m_position;
    }

    public Cell[,] m_grid;

    public Vector3 m_origin = Vector3.zero;
    public int m_width = 20;
    public int m_height = 20;
    public float m_cellradius = 0.5f;

    /// <summary>
    /// Creates the Grid.
    /// </summary>
    public void Create()
    {
        m_grid = new Cell[m_width, m_height];
        //Populate the grid
        for (int i = 0; i < m_width; i++)
        {
            for (int j = 0; j < m_height; j++)
            {
                Vector3 pos = new Vector3(((m_cellradius * 2) * i + m_cellradius) + m_origin.x, ((m_cellradius * 2) * j + m_cellradius) + m_origin.y, 0);
                m_grid[i, j] = new Cell(pos, new Vector2(i, j));
            }
        }
    }

    /// <summary>
    /// Generates the flowfield.
    /// </summary>
    public void GenerateFlowfield()
    {
        
        Queue<Cell> cells_to_process = new Queue<Cell>();
        int layermask = LayerMask.GetMask("Player", "Ground");
        foreach (Cell c in m_grid)
        {
            c.m_distance = 6500;
            c.m_direction = Vector2.zero;
            c.m_traversable = true;

            Collider2D hit = Physics2D.OverlapBox(c.m_position, Vector3.one/2, 0, layermask);
            if (hit != null)
            {
                if (hit.CompareTag("Ground"))
                {
                    c.m_traversable = false;
                    c.m_distance = 6500;
                }
                else if (hit.CompareTag("Player"))
                {
                    c.m_distance = 0;
                    cells_to_process.Enqueue(c);
                }
            }
        }
        
        // populate direction vectors 
        Cell cell;
        Vector2[] offsets = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(0, 0) };
        Vector2 neighborpos;
        while (cells_to_process.Count > 0)
        {
            cell = cells_to_process.Dequeue();//Get cell from the queue
            
            //Set best defaults
            float bestdistance = 6500;
            Vector2 bestoffset = Vector2.zero;
            //Iterate through each offset
            if(cell.m_index.y > 0 && !m_grid[(int)cell.m_index.x, (int)cell.m_index.y-1].m_traversable) cell.m_walkable = true;
            foreach (Vector2 offset in offsets)
            {
                neighborpos = cell.m_index + offset; //Get index of the offset cell
                
                if (!((neighborpos.x < 0 || neighborpos.x >= m_width) || (neighborpos.y < 0 || neighborpos.y >= m_height))) //Check that the cell is in bounds
                {
                    //If the Neighboring cell has a larger distance than this one, set the neighboring cells distance to be this plus one
                    if (m_grid[(int)neighborpos.x, (int)neighborpos.y].m_distance > cell.m_distance + 1 )
                    {
                        if(m_grid[(int)neighborpos.x, (int)neighborpos.y].m_traversable)
                        {
                            m_grid[(int)neighborpos.x, (int)neighborpos.y].m_distance = cell.m_distance + 1;
                            cells_to_process.Enqueue(m_grid[(int)neighborpos.x, (int)neighborpos.y]);
                        }
                        
                    }
                    //if the neighbor distance is smaller than the current best, set it as the best
                    if (m_grid[(int)neighborpos.x, (int)neighborpos.y].m_distance < bestdistance)
                    {
                        bestdistance = m_grid[(int)neighborpos.x, (int)neighborpos.y].m_distance;
                        bestoffset = offset;
                    }
                
                }
            }
            if (!cell.m_walkable && cell.m_traversable)
            {
                bestoffset += Vector2.down;
            }
            cell.m_direction = bestoffset;
        }
    }

    /// <summary>
    /// Resets the flow field.
    /// </summary>
    public void ResetFlowField()
    {
        foreach (Cell c in m_grid)
        {
            c.m_distance = 255;
            c.m_direction = Vector2.zero;
            c.m_traversable = true;
        }
    }

    /// <summary>
    /// Updates the enemy vectors.
    /// </summary>
    public void UpdateEnemyVectors()
    {
        //Check each cell for enemies and update that enemy with the correct flow field vecotr data
        int layermask = LayerMask.GetMask("Enemy");
        foreach (Cell cell in m_grid)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(cell.m_position, Vector3.one * m_cellradius * 2, 0, layermask);
            foreach (Collider2D hit in hits)
            {
                if (hit != null)
                {
                    //hit.gameObject.GetComponent<Swarm>().m_flowfieldvector = cell.m_direction.normalized;
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        GridGizmo();
    }

    /// <summary>
    /// Gizmos for the Grid
    /// </summary>
    private void GridGizmo()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < m_width; i++)
        {
            for (int j = 0; j < m_height; j++)
            {
                Vector3 pos = new Vector3(((m_cellradius * 2) * i + m_cellradius) + m_origin.x, ((m_cellradius * 2) * j + m_cellradius) + m_origin.y, 0);
                
                Gizmos.DrawWireCube(pos, Vector3.one * m_cellradius * 2);
                if (m_grid != null)
                {
                    Gizmos.DrawRay(pos, m_grid[i, j].m_direction.normalized);
                }
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Create();
        
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateEnemyVectors();
        if(makeflowfield)
        {
            makeflowfield = false;
            GenerateFlowfield();
        }
    }
    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        framecount++;
        if(framecount % m_frameInterval == 0) GenerateFlowfield();
    }
}