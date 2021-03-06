using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackAI : MonoBehaviour
{
    bool m_playerDetected = false;
    public bool m_controlRotation = true;
    public AudioSource m_barkSource;
    public List<AudioClip> m_aggroclips;
    public AudioClip m_attackClip;
    Transform m_playerTransform;


    public float m_detectionRadius;
    public float m_attackRadius;

    public bool m_attacking;

    public float m_Damage;

    public float m_cooldown;
    float m_cooldownTimer;

    Animator m_anim;
    Navmesh2DAgent m_agent;
    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_agent = GetComponent<Navmesh2DAgent>();
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        m_anim.SetBool("IsWalking", m_agent.m_isMoving);
        if ((!Physics2D.Linecast(transform.position, m_playerTransform.position, LayerMask.GetMask("Ground") | LayerMask.GetMask("Environment"))
            && (transform.position - m_playerTransform.position).magnitude < m_attackRadius))
        {
            m_attacking = true;
            m_anim.SetBool("IsShooting", true);
            m_agent.Stop();

        }
        else if (m_playerDetected && m_cooldownTimer <= 0)
        {
            m_attacking = false;
            m_anim.SetBool("IsShooting", false);
            m_agent.MoveTo(m_playerTransform.position);
        }
        else if ((transform.position - m_playerTransform.position).magnitude < m_detectionRadius && !m_playerDetected && m_cooldownTimer <= 0)
        {
            m_attacking = false;
            m_anim.SetBool("IsShooting", false);
            m_agent.MoveTo(m_playerTransform.position);
            m_playerDetected = true;
            if (m_barkSource != null && m_aggroclips.Count > 0)
            {
                m_barkSource.clip = m_aggroclips[Random.Range(0, m_aggroclips.Count)];
                m_barkSource.Play();
            }
        }
        else
        {
            m_attacking = false;
            m_agent.Stop();
            m_anim.SetBool("IsShooting", false);
        }
        if(m_cooldownTimer > 0)
        {
            m_cooldownTimer -= Time.deltaTime;
            m_agent.m_isMoving = false;
            m_agent.Stop();
        }
        else
        {
            m_cooldownTimer = 0;
        }
        if(m_controlRotation)
        {
            if (m_attacking && (m_playerTransform.position - transform.position).x < 0.0f)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (m_attacking)
            {
                transform.rotation = Quaternion.identity;
            }
        }

    }

    public void m_Attack()
    {
        if ((transform.position - m_playerTransform.position).magnitude < m_attackRadius)
        {
            m_playerTransform.gameObject.GetComponent<PlayerHealth>().Damage(m_Damage);
            m_cooldownTimer = m_cooldown;
            m_barkSource.clip = m_attackClip;
            m_barkSource.Play();
            
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
        Gizmos.DrawWireSphere(transform.position, m_attackRadius);
    }
}
