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
    [SerializeField] GameObject m_bulletSound;
    [SerializeField] GameObject m_OverHeatSound;

    [SerializeField] GameObject m_shootPart;
    [SerializeField] bool m_spawnOnShoot = false;
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

    [Header("Cooldown")]
    [SerializeField] float m_heatValue = 0.0f;
    [SerializeField] float m_heatUpRate = 0.25f;
    [SerializeField] float m_coolDownRate = 0.5f;
    [SerializeField] float m_startShootMax = 0.75f;

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

    GameObject _shootPart;

    bool canShoot = false;

    void Update()
    {
        if (!((m_leftClick && Mouse.current.leftButton.isPressed) || (!m_leftClick && Mouse.current.rightButton.isPressed)))
        {
            canShoot = false;
        }

        if (m_heatValue <= m_startShootMax && ((m_leftClick && Mouse.current.leftButton.isPressed) || (!m_leftClick && Mouse.current.rightButton.isPressed)))
        {
            canShoot = true;
        }

        if (m_heatValue >= 1.0f)
        {
            Instantiate(m_OverHeatSound, transform.position, Quaternion.identity, null);
            canShoot = false;
        }

        m_anim.ResetTrigger("Fire");
        if (canShoot && ((m_leftClick && Mouse.current.leftButton.isPressed) || (!m_leftClick && Mouse.current.rightButton.isPressed)))
        {
            if (!m_spawnOnShoot && !_shootPart) _shootPart = Instantiate(m_shootPart, m_shootPos.position, transform.rotation, transform);
            m_anim.SetBool("Shooting", true);
            if (m_currentRPS == 0 || Time.time - m_lastShotTime >= 1.0f / m_currentRPS)
            {
                m_anim.speed = m_currentRPS * (0.36f / 0.60f);
                //m_anim.speed = 1.0f;
                Shoot();
            }
            if (m_currentRPS < m_targetRPS) m_currentRPS += ((m_targetRPS - m_startRPS) / m_rampTime) * Time.deltaTime;
            else m_currentRPS = m_targetRPS;

            m_heatValue += m_heatUpRate * Time.deltaTime;
        }
        else
        {
            if (!m_spawnOnShoot && _shootPart) Destroy(_shootPart);
            m_anim.speed = 1.0f;
            m_anim.SetBool("Shooting", false);
            if (m_currentRPS > m_startRPS) m_currentRPS -= ((m_targetRPS - m_startRPS) / m_decayTime) * Time.deltaTime;
            else m_currentRPS = m_startRPS;

            m_heatValue -= m_coolDownRate * Time.deltaTime;
        }

        m_heatValue = Mathf.Clamp(m_heatValue, 0.0f, 1.0f);
        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        vel *= 0.95f;
        transform.position += vel * Time.deltaTime;

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_restPos, m_returnRate);
    }

    public void UpdateVisuals()
    {
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.color = new Color(1, (1.5f - m_heatValue) / 1.0f, (1.5f - m_heatValue) / 1.0f);
    }

    void Shoot()
    {
        m_lastShotTime = Time.time + Random.Range(0, m_randomShotDelay);

        m_anim.SetTrigger("Fire");

        GameObject sound = Instantiate(m_bulletSound, null);
        Destroy(sound, 5.0f);

        AudioSource m_as = sound.GetComponent<AudioSource>();
        m_as.pitch = Random.Range(0.9f, 1.1f);
        m_as.Play();

        if (m_spawnOnShoot)
        {
            GameObject _part = Instantiate(m_shootPart, m_shootPos.position, Quaternion.identity, null);
            Destroy(_part, 3.0f);
        }

        vel -= transform.right * m_recoilMulti;

        GameObject bullet = Instantiate(m_bulletPrefab, m_shootPos.position, transform.rotation, null);
        Rigidbody2D rb_bullet = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.m_damageMulti = m_bulletDamageMulti;
        bulletScript.m_damageAdd = m_bulletDamageAdd;

        rb_bullet.velocity = transform.right * transform.lossyScale.x * m_bullet_vel * Random.Range(0.9f, 1.1f);
        rb_bullet.velocity += (Vector2)transform.up * Random.Range(-0.5f,1.5f);
        rb_bullet.velocity += m_player.GetComponent<Rigidbody2D>().velocity;

        Destroy(bullet, 10);
    }

    public void AddDamage(float _amount)
    {
        m_bulletDamageAdd += _amount;
    }

    public void AddRPS(float _amount)
    {
        m_targetRPS += _amount;
    }

    public void ReduceRampTime(float _amount)
    {
        m_rampTime -= _amount;
        if (m_rampTime <= 0) m_rampTime = 0.1f;
    }
}
