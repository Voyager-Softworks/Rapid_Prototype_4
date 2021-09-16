using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuicideBirdAI : MonoBehaviour
{
    public AudioSource m_aggrosource;
    public UnityEvent OnCollide;
    Transform m_playerTransform;
    bool m_seeking = false;
    Vector2 m_seekDirection;
    Rigidbody2D m_body;

    public float m_seekForce;
    public float m_detectionRadius;
    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if ((!Physics2D.Linecast(transform.position, m_playerTransform.position, LayerMask.GetMask("Ground") | LayerMask.GetMask("Environment"))
            || (transform.position - m_playerTransform.position).magnitude < m_detectionRadius) && (!m_seeking))
        {
            m_seeking = true;
            m_seekDirection = (m_playerTransform.position - transform.position).normalized;
            m_aggrosource.Play();

        }
        if (m_seeking) m_body.velocity += m_seekDirection * m_seekForce * Time.deltaTime;
        if (m_seeking && m_body.velocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (m_seeking)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnCollide.Invoke();
    }
    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_detectionRadius);
    }
}
