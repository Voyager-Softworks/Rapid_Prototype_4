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

    public bool m_shooting;
    public List<Transform> m_barrelPositions;

    public Transform m_barrelAimHolder;
    public GameObject m_bulletPrefab;
    public float m_bulletSpeed;
    public float m_rotateSpeed;

    Animator m_anim;

    bool m_facingLeft = false;
    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if ((!Physics2D.Linecast(transform.position, m_playerTransform.position, LayerMask.GetMask("Ground") | LayerMask.GetMask("Environment"))
            && (transform.position - m_playerTransform.position).magnitude < m_attackRadius))
        {
            m_shooting = true;
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
        GameObject projectile = Instantiate(m_bulletPrefab, m_barrelPositions[_barrelIndex].position, m_barrelAimHolder.rotation, null);
        projectile.GetComponent<Rigidbody2D>().velocity = (m_barrelPositions[_barrelIndex].position - m_barrelAimHolder.position).normalized * m_bulletSpeed;
    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRadius);
    }
    

    public void RotateTurret()
    {
        Vector3 direction = m_playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        m_barrelAimHolder.rotation = Quaternion.Slerp(m_barrelAimHolder.rotation, rotation, Time.deltaTime * m_rotateSpeed);
    }

    
    






    
    

}
