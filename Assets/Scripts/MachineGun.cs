using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachineGun : MonoBehaviour
{
    public InputAction m_shootAction;

    [Header("Refs")]
    [SerializeField] GameObject m_player;

    [SerializeField] GameObject m_bulletPrefab;

    [SerializeField] Transform m_shootPos;
    Vector3 m_restPos;

    Animator m_anim;

    [Header("Bullet")]
    [SerializeField] float m_bullet_vel;
    [SerializeField] float m_bulletDamageMulti = 1.0f;
    [SerializeField] float m_bulletDamageAdd = 0.0f;

    [Header("Stats")]
    [SerializeField] float m_startRPS;
    [SerializeField] float m_currentRPS;
    [SerializeField] float m_targetRPS;
    [SerializeField] float m_randomShotDelay;
    [SerializeField] float m_rampTime;
    [SerializeField] float m_decayTime;
    float m_lastShotTime = 0;

    [Header("Feel")]
    [SerializeField] float m_recoilMulti = 5.0f;
    [SerializeField] [Range(0.0f, 1.0f)] float m_returnRate = 0.75f;
    [SerializeField] bool m_leftClick = true;

    Vector3 vel = Vector3.zero;

    void Start()
    {
        m_restPos = transform.localPosition;
        m_currentRPS = m_startRPS;
        m_anim = GetComponent<Animator>();
    }

    void Update()
    {
        m_anim.ResetTrigger("Fire");
        if ((m_leftClick && Mouse.current.leftButton.isPressed) || (!m_leftClick && Mouse.current.rightButton.isPressed))
        {
            m_anim.SetBool("Shooting", true);
            if (m_currentRPS == 0 || Time.time - m_lastShotTime >= 1.0f / m_currentRPS)
            {
                m_anim.speed = m_currentRPS * (0.36f / 0.60f);
                //m_anim.speed = 1.0f;
                Shoot();
            }
            if (m_currentRPS < m_targetRPS) m_currentRPS += ((m_targetRPS - m_startRPS) / m_rampTime) * Time.deltaTime;
            else m_currentRPS = m_targetRPS;
        }
        else
        {
            m_anim.SetBool("Shooting", false);
            if (m_currentRPS > m_startRPS) m_currentRPS -= ((m_targetRPS - m_startRPS) / m_decayTime) * Time.deltaTime;
            else m_currentRPS = m_startRPS;
        }
    }

    private void FixedUpdate()
    {
        vel *= 0.95f;
        transform.position += vel * Time.deltaTime;

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_restPos, m_returnRate);
    }

    void Shoot()
    {
        m_lastShotTime = Time.time + Random.Range(0, m_randomShotDelay);

        m_anim.SetTrigger("Fire");

        vel -= transform.right * m_recoilMulti;

        GameObject bullet = Instantiate(m_bulletPrefab, m_shootPos.position, transform.rotation, null);
        Rigidbody2D rb_bullet = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletScript= bullet.GetComponent<Bullet>();
        bulletScript.m_damageMulti = m_bulletDamageMulti;
        bulletScript.m_damageAdd = m_bulletDamageAdd;

        rb_bullet.velocity = transform.right * transform.lossyScale.x * m_bullet_vel;
        rb_bullet.velocity += (Vector2)transform.up * Random.Range(-0.5f,1.5f);
        rb_bullet.velocity += m_player.GetComponent<Rigidbody2D>().velocity;

        Destroy(bullet, 10);
    }
}
