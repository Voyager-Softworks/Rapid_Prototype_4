using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navmesh2DAgent : MonoBehaviour
{
    public Navmesh2D m_navmesh;
    public float m_speed = 1.0f;
    public float m_rotationSpeed = 1.0f;
    public List<Vector2> m_currentPath;
    Rigidbody2D m_rb;
    private Vector3 m_targetPosition;

    public bool m_isMoving = false;
    public bool m_canFly = false;
    public bool m_canClimb = false;



    void Start()
    {
        m_navmesh = FindObjectOfType<Navmesh2D>();
        m_rb = GetComponent<Rigidbody2D>();
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
            Vector2 climbforce = Vector2.zero;
            if (m_canClimb && m_rb != null)
            {
                if(!m_navmesh.IsTraversible(target, Vector2.right) || (m_navmesh.IsWalkable(target, Vector2.right) && m_navmesh.IsTraversible(this.gameObject.transform.position, Vector2.down)))
                {
                    climbforce = Vector2.right * 1.0f;
                }
                else if(!m_navmesh.IsTraversible(target, Vector2.left) || (m_navmesh.IsWalkable(target, Vector2.left) && m_navmesh.IsTraversible(this.gameObject.transform.position, Vector2.down)))
                {
                    climbforce = Vector2.left * 1.0f;
                }
                m_rb.AddForce(climbforce * 15.0f);
                if (m_rb.IsTouchingLayers(LayerMask.GetMask("Ground")) && climbforce.magnitude > 0.1f)
                {
                    m_rb.AddForce(-Physics2D.gravity * 4.0f);
                    m_rb.AddForce(climbforce * 20.0f);
                }
                
                
            }
            
            float distance = direction.magnitude;
            float step = m_speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            if (distance <= step)
            {
                m_currentPath.RemoveAt(0);
            }
            if (climbforce.magnitude < 0.1f)
            {
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
                if (climbforce.x > 0.0f)
                {
                    transform.rotation = Quaternion.identity;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
            

        }
        else
        {
            Stop();
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
        
        m_currentPath = newPath;
        
        
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
