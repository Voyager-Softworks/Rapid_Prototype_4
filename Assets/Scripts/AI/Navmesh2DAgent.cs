using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navmesh2DAgent : MonoBehaviour
{
    public Navmesh2D m_navmesh;
    public float m_speed = 1.0f;
    public float m_rotationSpeed = 1.0f;
    public List<Vector2> m_currentPath;

    private Vector3 m_targetPosition;

    public bool m_isMoving = false;
    public bool m_canFly = false;



    void Start()
    {
        m_navmesh = FindObjectOfType<Navmesh2D>();
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 targetPosition)
    {
        SetNewTargetPosition(targetPosition);
        m_isMoving = true;
    }

    public void Stop()
    {
        m_isMoving = false;
        m_currentPath = null;
    }

    public void SetNewTargetPosition(Vector3 targetPosition)
    {
        m_targetPosition = targetPosition;
        List<Vector2> newPath = m_navmesh.FindPath(transform.position, m_targetPosition, m_canFly);
        if (newPath != null && newPath.Count > 0)
        {
            m_currentPath = newPath;
        }
        
    }

    
    void OnDrawGizmosSelected()
    {
        if (m_currentPath != null)
        {
            for (int i = 0; i < m_currentPath.Count - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(m_currentPath[i], m_currentPath[i + 1]);
            }
        }
    }
}
