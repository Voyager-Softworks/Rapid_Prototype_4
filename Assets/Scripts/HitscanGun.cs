using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;




public class HitscanGun : MonoBehaviour
{
    [Header("Controls")]
    public InputAction fireAction;

    [Header("Firing Cone")]
    public float m_attackRadius;
    public float m_coneAngle;
    public Transform m_muzzle;

    [Header("Settings")]
    public float m_damage;
    public bool m_fullAuto;
    public Rigidbody2D m_body;

    public bool m_recoil;
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
    public ParticleSystem m_fireParticles;
    public ParticleSystem m_smokeParticles;
    public AudioSource m_fireAudioSource;
    public Animator m_anim;
    
    private GameObject persistent = null;
    private bool canShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        fireAction.Enable();
        m_anim = GetComponent<Animator>();

        persistent = DontDestroy.instance;
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
        if(canShoot && m_fullAuto && fireAction.ReadValue<float>() > 0 && m_currHeat < m_maxHeat)
        {
            m_anim.SetBool("IsShooting", true);
            if(m_fireParticles.isStopped)
            {
                m_fireParticles.Play();
            }
            if(!m_fireAudioSource.isPlaying)
            {
                m_fireAudioSource.Play();
            }
            if(m_recoil)
            {
                if (m_body) m_body.AddForce((-transform.right * 100000.0f) * new Vector2(1.0f, 0.3f));
            }
        }
        else if (m_fullAuto)
        {
            m_anim.SetBool("IsShooting", false);
            if(m_fireParticles.isPlaying)
            {
                m_fireParticles.Stop();
            }
            if(m_fireAudioSource.isPlaying)
            {
                m_fireAudioSource.Stop();
            }
        }
        if(m_smokeParticles != null)
        {
            if(m_currHeat > 0)
            {
                if(!m_smokeParticles.isPlaying)
                {
                    m_smokeParticles.Play();
                }
                
                ParticleSystem.EmissionModule e = m_smokeParticles.emission;
                e.rateOverTime = new ParticleSystem.MinMaxCurve((m_currHeat / m_maxHeat) * 150.0f);
            }
            else
            {
                m_smokeParticles.Stop();
            }
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

        FireAtEnemies();

        // Play the fire effect
        if(m_fireEffect != null) m_fireEffect.Play();

        if(!m_fullAuto)
        {
            m_anim.SetTrigger("Shoot");
        }
    }


    //Get all enemies in the attack radius and apply damage to them if they are within the cone
    void FireAtEnemies()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(m_muzzle.position, m_attackRadius);
        foreach (Collider2D col in hitColliders)
        {
            if (col.GetComponent<EnemyTest>())
            {
                Vector3 toEnemy = col.transform.position - m_muzzle.position;
                
                if (WithinCone(m_muzzle.position, m_muzzle.right, toEnemy, m_coneAngle/2.0f))
                {
                    col.GetComponent<EnemyTest>().TakeDamage(m_damage * m_damageFalloff.Evaluate(toEnemy.magnitude/m_attackRadius) * (persistent.GetComponent<UpgradeManager>().GetWeaponLevel(transform.name.ToLower().Contains("front")) + 1), EnemyTest.DamageType.Universal);
                }
            }
        }
    }



    //Check if a vector is within an angle from another vector using the dot product
    bool WithinCone(Vector3 coneOrigin, Vector3 coneDirection, Vector3 vectorToCheck, float coneAngle)
    {
        //Get the dot product between the cone direction and the vector to check
        float angle = Vector3.Dot(coneDirection, vectorToCheck);

        //Get the distance between the cone origin and the vector to check
        float distance = vectorToCheck.magnitude;

        //If the angle is greater than the cone angle, the vector is outside the cone
        if (angle > (1.0f - (coneAngle/360.0f)) && distance < m_attackRadius)
        {
            return true;
        }

        return false;
    }



    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float angle = -m_coneAngle * 0.5f;
        float angleStep = m_coneAngle / 20;

        Vector3 prevPos = m_muzzle.position;
        Vector3 pos = m_muzzle.position;
        for (int i = 0; i < 20; i++)
        {
            pos = m_muzzle.position + Quaternion.Euler(0, 0, angle) * m_muzzle.right * m_attackRadius;
            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
            angle += angleStep;
        }
        Gizmos.DrawLine(pos, m_muzzle.position);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_muzzle.position, (1.0f - (m_currHeat / m_maxHeat)) * 0.15f);

    }

    

}
