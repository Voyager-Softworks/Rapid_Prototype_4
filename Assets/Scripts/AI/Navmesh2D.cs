using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navmesh2D : MonoBehaviour
{
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
            m_traversable = true;
            m_walkable = false;
            m_position = _position;
            m_index = _index;
        }

        

        /// <summary>
        /// Sets the traversable.
        /// </summary>
        /// <param name="_traversable">If true, _traversable.</param>
        public void SetTraversable(bool _traversable) => m_traversable = _traversable;

        public bool m_traversable;
        public bool m_walkable;
        public Vector2 m_index;
        public Vector3 m_position;

        public float m_f;
        public float m_g;
        public float m_h;

        public Cell m_parent;
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

    //Gets the cell corresponding to the specified position in world space
    public Cell QueryPosition(Vector2 _pos)
    {
        Vector2 index = new Vector2(((_pos.x - m_origin.x) / (m_cellradius * 2))-m_cellradius, ((_pos.y - m_origin.y) / (m_cellradius * 2))-m_cellradius);
        int x = Mathf.RoundToInt(index.x);
        int y = Mathf.RoundToInt(index.y);
        if (x < 0 || x >= m_width || y < 0 || y >= m_height)
        {
            return null;
        }
        return m_grid[x, y];
    }

    //Checks if a neighboring cell in a given direction is walkable using QueryPosition
    public bool IsWalkable(Vector2 _pos, Vector2 _dir)
    {
        Cell cell = QueryPosition(_pos);
        if (cell == null)
        {
            return false;
        }
        if (cell.m_walkable == false)
        {
            return false;
        }
        Cell neighbor = QueryPosition(_pos + _dir);
        if (neighbor == null)
        {
            return false;
        }
        if (neighbor.m_walkable == false)
        {
            return false;
        }
        return true;
    }

    //Checks if a neighboring cell in a given direction is Traversible using QueryPosition
    public bool IsTraversible(Vector2 _pos, Vector2 _dir)
    {
        Cell cell = QueryPosition(_pos);
        if (cell == null)
        {
            return false;
        }
        if (cell.m_traversable == false)
        {
            return false;
        }
        Cell neighbor = QueryPosition(_pos + _dir);
        if (neighbor == null)
        {
            return false;
        }
        if (neighbor.m_traversable == false)
        {
            return false;
        }
        return true;
    }

    //Use A* to path from one cell to another cell using non-traversible cells as obstacles
    public List<Cell> Path(Cell _start, Cell _goal, bool _canFly)
    {
        List<Cell> open = new List<Cell>();
        List<Cell> closed = new List<Cell>();

        foreach (Cell cell in m_grid)
        {
            cell.m_f = Mathf.Infinity;
            cell.m_g = Mathf.Infinity;
            cell.m_h = Mathf.Infinity;
            cell.m_parent = null;
        }
        _start.m_g = 0.0f;
        _start.m_h = Heuristic(_start, _goal);
        _start.m_f = _start.m_g + _start.m_h;
        open.Add(_start);
        while (open.Count > 0)
        {
            Cell current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].m_f < current.m_f)
                {
                    current = open[i];
                }
            }
            if (current == _goal)
            {
                return RetracePath(_start, _goal);
            }
            open.Remove(current);
            closed.Add(current);
            foreach (Cell neighbor in GetNeighbors(current))
            {
                if (!(IsWalkable(neighbor.m_position, current.m_position - neighbor.m_position) || (_canFly && IsTraversible(neighbor.m_position, current.m_position - neighbor.m_position))))
                {
                    continue;
                }
                if (closed.Contains(neighbor))
                {
                    continue;
                }
                float new_g = current.m_g + Vector3.Distance(current.m_position, neighbor.m_position);
                if (new_g < neighbor.m_g)
                {
                    neighbor.m_g = new_g;
                    neighbor.m_parent = current;
                    neighbor.m_h = Vector3.Distance(neighbor.m_position, _goal.m_position);
                    neighbor.m_f = neighbor.m_g + neighbor.m_h;
                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                    
                }
                
                
            }
        }
        return null;
    }

    private float Heuristic(Cell start, Cell goal)
    {
        return Vector3.Distance(start.m_position, goal.m_position);
    }

    bool Contains(List<Cell> _list, Cell _node)
    {
        foreach (Cell node in _list)
        {
            if (node == _node)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerable<Cell> GetNeighbors(Cell current)
    {
        List<Cell> neighbors = new List<Cell>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                Vector2 index = current.m_index + new Vector2(i, j);
                if (index.x < 0 || index.x >= m_width || index.y < 0 || index.y >= m_height)
                {
                    continue;
                }
                neighbors.Add(m_grid[(int)index.x, (int)index.y]);
            }
        }
        return neighbors;
    }

    private List<Cell> RetracePath(Cell start, Cell goal)
    {
        List<Cell> path = new List<Cell>();
        Cell current = goal;
        while (current != start)
        {
            path.Add(current);
            
            current = current.m_parent;
        }
        path.Reverse();
        return path;
    }
    
    public List<Vector2> FindPath(Vector3 _start, Vector3 _goal, bool _canFly = false)
    {
        
        Cell start = QueryPosition(new Vector2(_start.x, _start.y));
        Cell goal = QueryPosition(new Vector2(_goal.x, _goal.y));
        if (start == null || goal == null)
        {
            
            return null;
            
        }
        List<Cell> path = Path(start, goal, _canFly);
        List<Vector2> path2 = new List<Vector2>();
        if(path == null) return path2;
        foreach (Cell cell in path)
        {
            path2.Add((Vector2)cell.m_position);
        }
        return path2;
    }

    //Draws a path gizmo using lines drawn between cells
    public void DrawPath(List<Cell> _path)
    {
        if (_path == null)
        {
            return;
        }
        for (int i = 0; i < _path.Count - 1; i++)
        {
            Debug.DrawLine(_path[i].m_position, _path[i + 1].m_position, Color.red, 0.5f);
        }
    }


    /// <summary>
    /// Generates the navmesh.
    /// </summary>
    public void GenerateNavmesh()
    {

        Queue<Cell> cells_to_process = new Queue<Cell>();
        int layermask = LayerMask.GetMask("Player", "Ground");
        foreach (Cell c in m_grid)
        {
            c.m_traversable = true;

            Collider2D hit = Physics2D.OverlapBox(c.m_position, (Vector3.one * m_cellradius), 0, layermask);
            if (hit != null)
            {
                if (hit.CompareTag("Ground"))
                {
                    c.m_traversable = false;
                }
            }
        }
        for (var x = 0; x < m_width; x++)
        {
            for (var y = 0; y < m_height; y++)
            {
                if (y > 0)
                {
                    if(m_grid[x, y].m_traversable && !m_grid[x, y-1].m_traversable) m_grid[x, y].m_walkable = true;
                }
                
            }
        }
    }

    /// <summary>
    /// Gizmos for the Grid
    /// </summary>
    private void GridGizmo()
    {
        
        for (int i = 0; i < m_width; i++)
        {
            for (int j = 0; j < m_height; j++)
            {
                Vector3 pos = new Vector3(((m_cellradius * 2) * i + m_cellradius) + m_origin.x, ((m_cellradius * 2) * j + m_cellradius) + m_origin.y, 0);
                Gizmos.color = Color.black  - new Color(0.0f, 0.0f, 0.0f, 0.7f);
                Gizmos.DrawWireCube(pos, Vector3.one * m_cellradius * 2);
                if (m_grid != null)
                {
                    
                    if (m_grid[i, j].m_walkable)
                    {
                        Gizmos.color = Color.cyan - new Color(0.0f, 0.0f, 0.0f, 0.95f);
                        Gizmos.DrawCube(pos, new Vector3(m_cellradius*2.0f, 1.0f, 1.0f));
                        Gizmos.color = Color.green - new Color(0.0f, 0.0f, 0.0f, 0.95f);
                        Gizmos.DrawCube(pos - new Vector3(0.0f, m_cellradius - 0.1f, 0.0f), new Vector3(m_cellradius*2.0f, 0.2f, 1.0f));
                        Gizmos.color = Color.black;
                    }
                    else if (m_grid[i, j].m_traversable)
                    {
                        Gizmos.color = Color.cyan - new Color(0.0f, 0.0f, 0.0f, 0.95f);
                        Gizmos.DrawCube(pos, new Vector3(m_cellradius*2.0f, 1.0f, 1.0f));
                        Gizmos.color = Color.black - new Color(0.0f, 0.0f, 0.0f, 0.7f);
                    }
                    else
                    {
                        Gizmos.color = Color.red - new Color(0.0f, 0.0f, 0.0f, 0.95f);
                        Gizmos.DrawCube(pos, new Vector3(m_cellradius*2.0f, 1.0f, 1.0f));
                        Gizmos.color = Color.black - new Color(0.0f, 0.0f, 0.0f, 0.7f);
                    }
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        GridGizmo();
    }

    // Start is called before the first frame update
    void Start()
    {
        Create();
        GenerateNavmesh();
    }


}
