using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackAI : MonoBehaviour
{

    Transform m_playerTransform;

    Rigidbody2D m_body;
    public float m_detectionRadius;
    public float m_attackRadius;

    public bool m_shooting;
    public List<Transform> m_barrelPositions;

    public GameObject m_bulletPrefab;
    public float m_bulletSpeed;

    Animator m_anim;
    NavAgent m_agent;

    bool m_facingLeft = false;
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
            m_shooting = true;
            m_anim.SetBool("IsShooting", true);
            m_agent.m_seekPlayer = false;

        }
        else if ((transform.position - m_playerTransform.position).magnitude < m_detectionRadius)
        {
            m_shooting = false;
            m_anim.SetBool("IsShooting", false);
            m_agent.m_seekPlayer = true;
        }
        else
        {
            m_shooting = false;
            m_agent.m_seekPlayer = false;
            m_anim.SetBool("IsShooting", false);
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
    public void Shoot(int _barrelIndex)
    {
        GameObject projectile = Instantiate(m_bulletPrefab, m_barrelPositions[_barrelIndex].position, transform.rotation, null);
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(m_facingLeft ? -m_bulletSpeed : m_bulletSpeed, 0.0f);
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
