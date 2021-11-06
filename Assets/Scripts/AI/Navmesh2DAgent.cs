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
    public bool m_canClimb = false;



    void Start()
    {
        m_navmesh = FindObjectOfType<Navmesh2D>();
    }
    

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if(!m_isMoving) return;
        if (m_currentPath != null && m_currentPath.Count > 0)
        {
            Vector2 target = m_currentPath[0];
            Vector2 direction = target - (Vector2)transform.position;
            float distance = direction.magnitude;
            float step = m_speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            if (distance <= step)
            {
                m_currentPath.RemoveAt(0);
            }
            if ((target - (Vector2)transform.position).x < 0.0f)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }

        }
        else
        {
            m_isMoving = false;
        }
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
        List<Vector2> newPath = m_navmesh.FindPath(transform.position, m_targetPosition, m_canFly, m_canClimb);
        if (newPath != null && newPath.Count > 0)
        {
            m_currentPath = newPath;
        }
        
    }

    
    void OnDrawGizmos()
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
