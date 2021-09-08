using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachineGun : MonoBehaviour
{
    public InputAction m_shootAction;

    [SerializeField] GameObject m_player;

    [SerializeField] GameObject m_bullet_prefab;

    [SerializeField] Transform m_shootPos;
    Vector3 m_restPos;

    [Header("Stats")]
    [SerializeField] float m_bullet_vel;
    [SerializeField] float m_startRPS;
    [SerializeField] float m_currentRPS;
    [SerializeField] float m_targetRPS;
    [SerializeField] float m_rampTime;
    [SerializeField] float m_decayTime;
    [SerializeField] float m_lastShotTime = 0;

    Vector3 vel = Vector3.zero;

    void Start()
    {
        m_restPos = transform.localPosition;
        m_currentRPS = m_startRPS;
    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (m_currentRPS == 0 || Time.time - m_lastShotTime >= 1.0f / m_currentRPS)
            {
                Shoot();
            }
            if (m_currentRPS < m_targetRPS) m_currentRPS += ((m_targetRPS - m_startRPS) / m_rampTime) * Time.deltaTime;
            else m_currentRPS = m_targetRPS;
        }
        else
        {
            if (m_currentRPS > m_startRPS) m_currentRPS -= ((m_targetRPS - m_startRPS) / m_decayTime) * Time.deltaTime;
            else m_currentRPS = m_startRPS;
        }
    }

    private void FixedUpdate()
    {
        vel *= 0.95f;
        transform.position += vel * Time.deltaTime;

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_restPos, 0.75f);
    }

    void Shoot()
    {
        m_lastShotTime = Time.time;

        vel -= transform.right * 10.0f;

        GameObject bullet = Instantiate(m_bullet_prefab, m_shootPos.position, transform.rotation, null);
        Rigidbody2D rb_bullet = bullet.GetComponent<Rigidbody2D>();

        rb_bullet.velocity = transform.right * m_bullet_vel;
        rb_bullet.velocity += m_player.GetComponent<Rigidbody2D>().velocity;

        Destroy(bullet, 10);
    }
}
