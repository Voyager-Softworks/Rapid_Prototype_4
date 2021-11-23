using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ProjectileGun : MonoBehaviour
{
    [Header("Controls")]
    public InputAction fireAction;

    [Header("Shooting")]
    public Transform m_muzzle;
    public GameObject m_projectilePrefab;
    public float m_projectileSpeed;

    [Header("Settings")]
    public float m_damage;
    public AnimationCurve m_damageFalloff;
    public float m_fireDelay;
    float m_fireTimer;
    public float m_reloadTime;
    float m_reloadTimer;

    [Header("Cooldown")]
    public float m_maxHeat;
    float m_currHeat;
    public float m_heatPerShot;
    public float m_heatDecayPerSecond;

    [Header("Effects")]
    public SFX_Effect m_fireEffect;

    [Header("Animation")]
    Animator m_animator;


    private bool canShoot = false;


    // Start is called before the first frame update
    void Start()
    {
        fireAction.Enable();
        m_fireTimer = m_fireDelay;
        m_reloadTimer = 0.0f;
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        m_fireTimer -= Time.deltaTime;
        if (m_fireTimer <= 0)
        {
            m_fireTimer = 0;
        }
        m_reloadTimer -= Time.deltaTime;
        if (m_reloadTimer <= 0)
        {
            m_reloadTimer = 0;
        }
        if (m_currHeat > 0)
        {
            m_currHeat -= m_heatDecayPerSecond * Time.deltaTime;
            if (m_currHeat < 0)
            {
                m_currHeat = 0;
            }
        }

        canShoot = true;
        bool noUIcontrolsInUse = !EventSystem.current.IsPointerOverGameObject();
        if (!noUIcontrolsInUse) canShoot = false;

        if(canShoot && fireAction.ReadValue<float>() > 0 && m_fireTimer <= 0 && m_reloadTimer <= 0)
        {
            Fire();
        }
        
    }

    public void Fire()
    {
        if (m_currHeat >= m_maxHeat || m_fireTimer > 0 || m_reloadTimer > 0)
        {
            return;
        }

        m_currHeat += m_heatPerShot;
        m_fireTimer = m_fireDelay;
        m_animator.SetTrigger("Shoot");
        Rigidbody2D projectile = Instantiate(m_projectilePrefab, m_muzzle.position, m_muzzle.rotation).GetComponent<Rigidbody2D>();
        projectile.velocity = m_projectileSpeed * m_muzzle.right;

        // Play the fire effect
        m_fireEffect.Play();
    }


    



    


    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_muzzle.position, (1.0f - (m_currHeat / m_maxHeat)) * 0.15f);
        Gizmos.DrawLine(m_muzzle.position, m_muzzle.position + m_muzzle.forward * 0.2f);

    }
}
