using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : MonoBehaviour
{
    public Vector2 m_flowVector;
    public float m_speed;
    public float m_maxGroundedVelocity;
    public float m_maxAerialVelocity;
    public float m_jumpForce;

    public float m_jumpCooldown;
    float m_jumpCooldownTimer = 0.0f;
    public float m_groundCheckOffset = 0.0f;
    public float m_stoppingDistance;
    public bool m_grounded;

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

    void Jump()
    {
        if (!m_grounded || m_jumpCooldownTimer > 0.0f) return;
        m_jumpCooldownTimer = m_jumpCooldown;
        m_body.AddForce(Vector2.up * m_jumpForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_jumpCooldownTimer > 0.0f)
        {
            m_jumpCooldownTimer -= Time.deltaTime;

        }
        m_grounded = Physics2D.BoxCast(transform.position + (Vector3.down * m_groundCheckOffset), new Vector3(0.8f, 0.25f, 0), 0.0f, Vector2.up, 1.0f, LayerMask.GetMask("Ground"));


        Vector2 newVelocity = Vector2.zero;
        if (m_seekPlayer)
        {
            if (Physics2D.Linecast(transform.position, m_playerTransform.position, LayerMask.GetMask("Ground") | LayerMask.GetMask("Environment"))
            || (transform.position - m_playerTransform.position).magnitude > m_stoppingDistance)
            {
                if (m_flowVector.x > 0)
                {
                    newVelocity.x = m_speed;
                }
                else if (m_flowVector.x < 0)
                {
                    newVelocity.x = -m_speed;
                }
                if (m_flowVector.y > 0)
                {
                    Jump();
                }
            }
        }
        m_body.AddForce(newVelocity);
        if (m_grounded)
        {
            if (m_body.velocity.magnitude > m_maxGroundedVelocity)
            {
                Vector3 newV = m_body.velocity.normalized * m_maxGroundedVelocity;
                newV.y = m_body.velocity.y;
                m_body.velocity = newV;
            }
        }
        else
        {
            if (m_body.velocity.magnitude > m_maxAerialVelocity)
            {
                Vector3 newV = m_body.velocity.normalized * m_maxAerialVelocity;
                newV.y = m_body.velocity.y;
                m_body.velocity = newV;
            }
        }
        if (m_anim != null)
        {
            m_anim.SetBool("IsWalking", Mathf.Abs(m_body.velocity.x) > 0.02f && m_grounded);
            m_anim.SetBool("IsGrounded", m_grounded);
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + (Vector3.down * m_groundCheckOffset), new Vector3(0.8f, 0.25f, 0));
    }
}
