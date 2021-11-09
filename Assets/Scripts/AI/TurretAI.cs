using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    bool m_playerDetected = false;
    public AudioSource m_barkSource;
    public List<AudioClip> m_clips;
    public ParticleSystem p;
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
    public float m_rotateSpeed;

    public int m_clipSize = 0;
    public float m_shotDelay = 0.0f;
    public float m_reloadDelay = 0.0f;

    float m_shotDelayTimer = 0.0f;
    float m_reloadDelayTimer = 0.0f;
    int m_clip = 0;
    int m_currBarrel = 0;
    Animator m_anim;

    bool m_facingLeft = false;
    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_anim = GetComponent<Animator>();
        m_clip = m_clipSize;
    }

    //Reloads the clip
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
        if ((!Physics2D.Linecast(transform.position, m_playerTransform.position, LayerMask.GetMask("Ground") | LayerMask.GetMask("Environment"))
            && (transform.position - m_playerTransform.position).magnitude < m_attackRadius))
        {
            Shoot(m_currBarrel++);
            if(m_currBarrel >= m_barrelPositions.Count)
            {
                m_currBarrel = 0;
            }
            m_anim.SetBool("IsShooting", true);
            RotateTurret();

        }
        else if (m_playerDetected)
        {
            m_shooting = false;
            m_anim.SetBool("IsShooting", false);
            RotateTurret();
        }
        else if ((transform.position - m_playerTransform.position).magnitude < m_detectionRadius && !m_playerDetected)
        {
            m_shooting = false;
            m_anim.SetBool("IsShooting", false);
            
            m_playerDetected = true;
            if (m_barkSource)
            {
                m_barkSource.clip = m_clips[Random.Range(0, m_clips.Count)];
                m_barkSource.Play();
            }
        }
        else
        {
            m_shooting = false;
            
            m_anim.SetBool("IsShooting", false);
        }
        

    }
    public void Shoot(int _barrelIndex)
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

        
        if(m_clip > 0)
        {
            m_clip--;
            m_shotDelayTimer = m_shotDelay;
            GameObject projectile = Instantiate(m_bulletPrefab, m_barrelPositions[_barrelIndex].position, m_barrelAimHolder.rotation, null);
            projectile.GetComponent<Rigidbody2D>().velocity = (m_barrelPositions[_barrelIndex].position - m_barrelAimHolder.position).normalized * m_bulletSpeed;
        }
        else
        {
            Reload();
        }
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
        
        
        //Draw a circle section using lines to show the range of motion of the turret
        float angle = m_minAngle;
        float angleStep = (m_maxAngle - m_minAngle) / 20;
        Vector3 prevPos = transform.position;
        Vector3 pos = transform.position;
        for (int i = 0; i < 20; i++)
        {
            pos = transform.position + Quaternion.Euler(0, 0, angle) * Vector3.right * m_attackRadius;
            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
            angle += angleStep;
        }
        Gizmos.DrawLine(pos, transform.position);
        

    }
    
    
    public void RotateTurret()
    {

        Vector3 direction = m_playerTransform.position - transform.position;
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
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        m_barrelAimHolder.rotation = Quaternion.Slerp(m_barrelAimHolder.rotation, rotation, Time.deltaTime * m_rotateSpeed);
    }

    
    






    
    

}
