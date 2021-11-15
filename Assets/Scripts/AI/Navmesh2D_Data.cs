using System.Collections;
using System.Collections.Generic;
using UnityEngine;





[System.Serializable]
public class Navmesh2D_Data : ScriptableObject
{
    [SerializeField]
    
    public Vector2Int size;
    [SerializeField]
    public List<Navmesh2D.Cell> m_grid;
    public Navmesh2D.Cell this[int x, int y]{ get { return m_grid[y * size.x + x] ; } set { m_grid[y * size.x + x] = value; } }

    /// <summary>
    /// Creates the Grid.
    /// </summary>
    public void Create(int width, int height, float cellRadius, Vector2 origin)
    {
        m_grid = new List<Navmesh2D.Cell>();
        size = new Vector2Int(width, height);
        
        //Populate the grid
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 pos = new Vector3(((cellRadius * 2) * j + cellRadius) + origin.x, ((cellRadius * 2) * i + cellRadius) + origin.y, 0);
                m_grid.Add(new Navmesh2D.Cell(pos, new Vector2(j, i)));
            }
        }
    }

}
