using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackAI : MonoBehaviour
{
    bool m_playerDetected = false;
    public AudioSource m_barkSource;
    public List<AudioClip> m_clips;
    Transform m_playerTransform;

    Rigidbody2D m_body;
    public float m_detectionRadius;
    public float m_attackRadius;
    public float m_minAngle, m_maxAngle;
    public bool m_shooting;
    public List<Transform> m_barrelPositions;

    public Transform m_barrelAimHolder;
    public GameObject m_bulletPrefab;
    public float m_bulletSpeed;

    Animator m_anim;
    Navmesh2DAgent m_agent;

    public int m_clipSize = 0;
    public float m_shotDelay = 0.0f;
    public float m_reloadDelay = 0.0f;

    float m_shotDelayTimer = 0.0f;
    float m_reloadDelayTimer = 0.0f;
    int m_clip = 0;
    int m_currBarrel = 0;
    

    bool m_roundChambered = false;


    bool m_facingLeft = false;
    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_agent = GetComponent<Navmesh2DAgent>();
        m_anim = GetComponent<Animator>();
        m_clip = m_clipSize;
    }

    void Reload()
    {
        m_reloadDelayTimer = m_reloadDelay;
        m_clip = m_clipSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_reloadDelayTimer > 0.0f)
        {
            m_reloadDelayTimer -= Time.deltaTime;
        }
        else
        {
            m_reloadDelayTimer = 0.0f;
        }
        if (m_shotDelayTimer > 0.0f)
        {
            m_shotDelayTimer -= Time.deltaTime;
        }
        else
        {
            m_shotDelayTimer = 0.0f;
        }
        m_anim.SetBool("IsWalking", m_agent.m_isMoving);
        if ((!Physics2D.Linecast(transform.position, m_playerTransform.position, LayerMask.GetMask("Ground") | LayerMask.GetMask("Environment"))
            && (transform.position - m_playerTransform.position).magnitude < m_attackRadius) &&
            !(
                Vector3.Angle(m_playerTransform.position - transform.position, transform.right) < m_minAngle || 
                Vector3.Angle(m_playerTransform.position - transform.position, transform.right) > m_maxAngle
            ))
        {
            if(m_barrelAimHolder != null) RotateTurret();
            Shoot();
            m_anim.SetBool("IsShooting", true);
            m_agent.Stop();

        }
        else if (m_playerDetected)
        {
            
            
            m_agent.MoveTo(m_playerTransform.position);
            if (m_agent.m_currentPath.Count == 0 && m_agent.m_navmesh.IsWalkable(transform.position, ( m_playerTransform.position - transform.position).normalized/2.0f) )
            {
                m_anim.SetBool("IsWalking", true);
                transform.position += (m_playerTransform.position - transform.position).normalized * m_agent.m_speed * Time.deltaTime;
            }
        }
        else if ((transform.position - m_playerTransform.position).magnitude < m_detectionRadius && !m_playerDetected)
        {
            
            
            m_agent.MoveTo(m_playerTransform.position);
            m_playerDetected = true;
            m_barkSource.clip = m_clips[Random.Range(0, m_clips.Count)];
            m_barkSource.Play();
        }
        else
        {
            
            m_agent.Stop();
            
        }
        if (m_shooting && (m_playerTransform.position - transform.position).x < 0.0f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            m_facingLeft = true;
        }
        else if (m_shooting)
        {
            transform.rotation = Quaternion.identity;
            m_facingLeft = false;
        }

    }
    public void Shoot()
    {
        if (m_reloadDelayTimer > 0.0f || m_shotDelayTimer > 0.0f)
        {
            return;
        }
        //return if player is outside of min and max angle
        if (Vector3.Angle(m_playerTransform.position - transform.position, transform.right) < m_minAngle || Vector3.Angle(m_playerTransform.position - transform.position, transform.right) > m_maxAngle)
        {
            return;
        }
        if (m_roundChambered) return;
        if(m_clip > 0)
        {
            m_clip--;
            m_anim.SetTrigger("Shoot");
            m_roundChambered = true;
            m_anim.speed = 1.0f / m_shotDelay;
            if(++m_currBarrel >= m_barrelPositions.Count)
            {
                m_currBarrel = 0;
            }
        }
        else
        {
            Reload();
        }
    }

    public void Fire()
    {
        m_roundChambered = false;
        m_anim.speed = 1.0f;
        m_shotDelayTimer = m_shotDelay;
        GameObject projectile = Instantiate(m_bulletPrefab, m_barrelPositions[m_currBarrel].position, this.transform.rotation, null);
        projectile.GetComponent<Rigidbody2D>().velocity = (m_barrelPositions[m_currBarrel].right).normalized * m_bulletSpeed;
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_detectionRadius);
        Gizmos.color = Color.red;
        
        if (m_maxAngle < m_minAngle)
        {
            m_maxAngle = m_minAngle;
        }
        
        float angle = m_minAngle;
        float angleStep = (m_maxAngle - m_minAngle) / 20;
        Vector3 prevPos = transform.position;
        Vector3 pos = transform.position;
        for (int i = 0; i < 20; i++)
        {
            pos = transform.position + Quaternion.Euler(0, 0, angle) * transform.right * m_attackRadius;
            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
            angle += angleStep;
        }
        Gizmos.DrawLine(pos, transform.position);
        
        for(int i = 0; i < m_barrelPositions.Count; i++)
        {
            if(i == m_currBarrel && m_reloadDelayTimer <= 0.0f) Gizmos.color = Color.green;
            else Gizmos.color = Color.red;
            
            Gizmos.DrawLine(m_barrelPositions[i].position, m_barrelPositions[i].position + m_barrelPositions[i].right * 0.2f);
            Gizmos.DrawSphere(m_barrelPositions[i].position, 0.05f);
        }
        Gizmos.color = Color.green;
        if (m_reloadDelayTimer > 0.0f)
        {
            Gizmos.DrawSphere(transform.position, ((m_reloadDelay - m_reloadDelayTimer)/m_reloadDelay) * 0.25f);
        }
        else
        {
            Gizmos.DrawSphere(transform.position, ((float)m_clip / (float)m_clipSize) * 0.25f);
        }
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }

    public void RotateTurret()
    {

        Vector3 direction = m_playerTransform.position - m_barrelAimHolder.position;
        if(Vector3.Dot(transform.right, Vector3.right) < 0.0f)
        {
            direction = -direction;
        }
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //clamp angle
        if (angle < m_minAngle)
        {
            angle = m_minAngle;
        }
        else if (angle > m_maxAngle)
        {
            angle = m_maxAngle;
        }
        
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.Euler(0, m_barrelAimHolder.eulerAngles.y, 0);
        
        

        m_barrelAimHolder.rotation = Quaternion.Slerp(m_barrelAimHolder.rotation, rotation, Time.deltaTime * 2.0f);
        
    }
}
