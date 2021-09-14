using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingNavAgent : MonoBehaviour
{
    public Vector2 m_flowVector;
    public float m_speed;

    public float m_maxAerialVelocity;



    public float m_stoppingDistance;


    public bool m_seekPlayer = true;

    Transform m_playerTransform;

    Rigidbody2D m_body;

    Animator m_anim;

    // Start is called before the first frame update
    void Start()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_flowVector = Vector2.zero;
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_anim = GetComponent<Animator>();
    }



    // Update is called once per frame
    void Update()
    {


        Vector2 newVelocity = Vector2.zero;
        Vector2 AvoidanceVelocity = Vector2.zero;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.5f, Vector2.down, 1, LayerMask.GetMask("Ground"));
        foreach (var hit in hits)
        {
            AvoidanceVelocity += (Vector2)((hit.transform.position - transform.position) * Time.deltaTime);
        }
        AvoidanceVelocity.Normalize();
        m_flowVector.Normalize();
        if (m_seekPlayer)
        {
            if (Physics2D.Linecast(transform.position, m_playerTransform.position, LayerMask.GetMask("Ground") | LayerMask.GetMask("Environment"))
            || (transform.position - m_playerTransform.position).magnitude > m_stoppingDistance)
            {
                newVelocity = (m_flowVector + AvoidanceVelocity) * m_speed;
                m_body.velocity += (Vector2.Lerp(m_body.velocity, newVelocity, Time.deltaTime / 5.0f)) / 10.0f;
            }
        }
        //m_body.AddForce(newVelocity);

        if (m_body.velocity.magnitude > m_maxAerialVelocity)
        {

            m_body.velocity = m_body.velocity.normalized * m_maxAerialVelocity;
        }

        if (m_anim != null)
        {

            if (m_seekPlayer && m_body.velocity.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (m_seekPlayer)
            {
                transform.rotation = Quaternion.identity;
            }
        }
        m_flowVector = Vector2.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, m_body.velocity);
    }
}
