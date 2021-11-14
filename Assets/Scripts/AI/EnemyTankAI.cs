using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankAI : MonoBehaviour
{
    enum TankState
    {
        Inactive,
        NormalAttackPhase,
        MissileAttackPhase,
        ReloadingPhase
    }

    TankState state;
    public AudioSource m_barkSource;
    public List<AudioClip> m_aggroclips;
    public AudioClip m_attackClip;
    Transform m_playerTransform;
    public GameObject m_missilePrefab;


    public float m_detectionRadius;
    public float m_attackRadius;
    
    public float m_normalAttackDuration = 5f;
    public float m_missileAttackDuration = 5f;
    public float m_reloadDuration = 5f;

    float m_normalAttackTimer;
    float m_missileAttackTimer;
    float m_reloadTimer;

    public float m_speed = 5f;
    Animator m_anim;
    Navmesh2D m_nav;
    Rigidbody2D m_rb;

    TurretAI m_turret;
    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_rb = GetComponent<Rigidbody2D>();
        m_nav = FindObjectOfType<Navmesh2D>();
        m_anim = GetComponent<Animator>();
        state = TankState.Inactive;
        m_normalAttackTimer = m_normalAttackDuration;
        m_missileAttackTimer = m_missileAttackDuration;
        m_reloadTimer = m_reloadDuration;
        m_turret = GetComponentInChildren<TurretAI>();
        m_turret.m_manualdisable = true;
    }

    void Reload()
    {
        m_reloadTimer = m_reloadDuration;
        state = TankState.ReloadingPhase;
        m_anim.SetTrigger("Reload");
        
    }

    void MissileAttack()
    {
        m_missileAttackTimer = m_missileAttackDuration;
        state = TankState.MissileAttackPhase;
        StartCoroutine("MissileAttackCoroutine");
        m_anim.SetTrigger("MissileAttack");
        m_turret.m_manualdisable = true;
    }

    // Coroutine to instantiate 3 missiles with a delay
    IEnumerator MissileAttackCoroutine()
    {
        
        Instantiate(m_missilePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(m_missilePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(m_missilePrefab, transform.position, Quaternion.identity);
    }

    void NormalAttack()
    {
        m_turret.m_manualdisable = false;
        m_normalAttackTimer = m_normalAttackDuration;
        state = TankState.NormalAttackPhase;
        m_anim.SetTrigger("NormalAttack");
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case TankState.Inactive:
                if (Vector3.Distance(transform.position, m_playerTransform.position) < m_detectionRadius)
                {
                    NormalAttack();
                    m_barkSource.PlayOneShot(m_aggroclips[Random.Range(0, m_aggroclips.Count)]);
                }
                break;
            case TankState.NormalAttackPhase:
                if (Vector3.Distance(transform.position, m_playerTransform.position) > 6.0f &&
                m_nav.IsWalkable(transform.position - Vector3.up * 0.5f, (((Vector2)transform.position - (Vector2)m_playerTransform.position) * Vector2.right).normalized))
                {
                    m_rb.velocity = ((Vector2)m_playerTransform.position - (Vector2)transform.position) * Vector2.right * m_speed;
                    m_anim.SetBool("IsMoving", true);
                }
                else
                {
                    m_anim.SetBool("IsMoving", false);
                }
                if ((m_normalAttackTimer -= Time.deltaTime) <= 0)
                {
                    MissileAttack();
                }
                if ((m_playerTransform.position - transform.position).x < 0.0f)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                }
                break;
            case TankState.MissileAttackPhase:
                if ((m_missileAttackTimer -= Time.deltaTime) <= 0)
                {
                    Reload();
                }
                break;
            case TankState.ReloadingPhase:
                if ((m_reloadTimer -= Time.deltaTime) <= 0)
                {
                    NormalAttack();
                }
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_detectionRadius);

        
    }
}
