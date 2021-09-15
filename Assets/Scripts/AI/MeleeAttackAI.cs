using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackAI : MonoBehaviour
{
    Transform m_playerTransform;


    public float m_detectionRadius;
    public float m_attackRadius;

    public bool m_attacking;

    Animator m_anim;
    NavAgent m_agent;
    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_agent = GetComponent<NavAgent>();
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if ((!Physics2D.Linecast(transform.position, m_playerTransform.position, LayerMask.GetMask("Ground") | LayerMask.GetMask("Environment"))
            && (transform.position - m_playerTransform.position).magnitude < m_attackRadius))
        {
            m_attacking = true;
            m_anim.SetBool("IsShooting", true);
            m_agent.m_seekPlayer = false;

        }
        else if ((transform.position - m_playerTransform.position).magnitude < m_detectionRadius)
        {
            m_attacking = false;
            m_anim.SetBool("IsShooting", false);
            m_agent.m_seekPlayer = true;
        }
        else
        {
            m_attacking = false;
            m_agent.m_seekPlayer = false;
            m_anim.SetBool("IsShooting", false);
        }
        if (m_attacking && (m_playerTransform.position - transform.position).x < 0.0f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (m_attacking)
        {
            transform.rotation = Quaternion.identity;
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
