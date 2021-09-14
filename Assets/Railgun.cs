using UnityEngine;
using UnityEngine.InputSystem;

public class Railgun : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject m_player;

    [SerializeField] GameObject m_beam;

    [SerializeField] Transform m_shootPos;
    Vector3 m_restPos;

    Animator m_anim;

    AudioSource AS;

    [Header("Bullet")]
    [SerializeField] float m_Range = 20.0f;
    [SerializeField] float m_Damage = 150.0f;

    [Header("Stats")]
    [SerializeField] float m_currentCharge = 0;
    [SerializeField] float m_desiredCharge = 100.0f;
    [SerializeField] float m_chargePerSecond = 75.0f;
    float m_lastShotTime = 0;

    [Header("Feel")]
    [SerializeField] float m_recoilMulti = 5.0f;
    [SerializeField] [Range(0.0f, 1.0f)] float m_returnRate = 0.75f;
    [SerializeField] bool m_leftClick = true;

    Vector3 vel = Vector3.zero;

    void Start()
    {
        AS = GetComponent<AudioSource>();
        m_restPos = transform.localPosition;
        m_anim = GetComponent<Animator>();
    }

    void Update()
    {
        m_anim.ResetTrigger("Fire");
        if ((m_leftClick && Mouse.current.leftButton.isPressed) || (!m_leftClick && Mouse.current.rightButton.isPressed))
        {
            m_currentCharge += m_chargePerSecond * Time.deltaTime;
            if (m_currentCharge >= m_desiredCharge) m_currentCharge = m_desiredCharge;
        }
        else
        {
            if (m_currentCharge >= m_desiredCharge)
            {
                Shoot();
                m_currentCharge = 0;
            }
            else
            {
                m_currentCharge = m_currentCharge * Time.deltaTime * 5;
            }
            m_anim.SetBool("Shooting", false);
        }

        m_beam.transform.localScale = Vector3.Lerp(m_beam.transform.localScale, new Vector3(m_beam.transform.localScale.x, 0, 1), 5.0f * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        vel *= 0.95f;
        transform.position += vel * Time.deltaTime;

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_restPos, m_returnRate);
    }

    void Shoot()
    {
        AS.Play();

        m_anim.SetTrigger("Fire");

        m_beam.transform.parent = m_shootPos.transform;
        m_beam.transform.localPosition = Vector3.zero;
        m_beam.transform.localRotation = Quaternion.identity;

        vel -= transform.right * m_recoilMulti;

        RaycastHit2D[] hits = Physics2D.RaycastAll(m_shootPos.position, transform.right);

        System.Array.Sort(hits, (a, b) => { return a.distance.CompareTo(b.distance); });

        float hitDist = 20;

        foreach (RaycastHit2D _hit in hits)
        {
            if (_hit && _hit.transform)
            {

                EnemyTest _e = _hit.transform.GetComponent<EnemyTest>();
                if (_e)
                {
                    _e.TakeDamage(m_Damage);
                }

                ResourceNode _rn = _hit.transform.GetComponent<ResourceNode>();
                if (_rn)
                {
                    _rn.TakeDamage(50.0f);
                }

                if (_hit.transform.gameObject.layer == 6)
                {
                    hitDist = _hit.distance;
                    break;
                }
            }
        }


        m_beam.transform.localScale = new Vector3(hitDist, 0.05f, 1.0f);
        m_beam.transform.localPosition = new Vector3(hitDist / 2.0f, 0, 0);

        m_beam.transform.parent = null;
    }
}
