using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Railgun : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject m_player;

    [SerializeField] GameObject m_beam;

    [SerializeField] Transform m_shootPos;
    Vector3 m_restPos;

    Animator m_anim;

    [Header("Bullet")]
    [SerializeField] float m_Range = 20.0f;
    [SerializeField] float m_Damage = 150.0f;
    [SerializeField] EnemyTest.DamageType m_damageType;

    [Header("Particles")]
    [SerializeField] GameObject m_chargePartPrefab;
    [SerializeField] GameObject m_shootPartPrefab;
    [SerializeField] GameObject m_hitPartPrefab;

    [Header("Sounds")]
    [SerializeField] AudioSource m_as;
    [SerializeField] AudioClip m_shootSound;
    [SerializeField] AudioClip m_chargeSound;



    [Header("Stats")]
    [SerializeField] float m_currentCharge = 0;
    [SerializeField] float m_desiredCharge = 100.0f;
    [SerializeField] float m_chargePerSecond = 75.0f;
    [SerializeField] float m_cooldownWait = 2.0f;
    float m_lastShotTime = 0;

    [Header("Feel")]
    [SerializeField] float m_recoilMulti = 5.0f;
    [SerializeField] [Range(0.0f, 1.0f)] float m_returnRate = 0.75f;
    [SerializeField] bool m_leftClick = true;

    public UnityEvent OnShoot;

    Vector3 vel = Vector3.zero;

    GameObject _chargePart;

    void Start()
    {
        m_as = GetComponent<AudioSource>();
        m_restPos = transform.localPosition;
        m_anim = GetComponent<Animator>();
    }

    void Update()
    {
        bool noUIcontrolsInUse = !EventSystem.current.IsPointerOverGameObject();

        m_anim.ResetTrigger("Fire");
        if (noUIcontrolsInUse && (Time.time - m_lastShotTime >= m_cooldownWait && (m_leftClick && Mouse.current.leftButton.isPressed) || (!m_leftClick && Mouse.current.rightButton.isPressed)))
        {

            m_as.clip = m_chargeSound;
            m_as.loop = true;
            if (!m_as.isPlaying) m_as.Play();

            if (!_chargePart) _chargePart = Instantiate(m_chargePartPrefab, m_shootPos.position, transform.rotation, transform);
            m_currentCharge += m_chargePerSecond * Time.deltaTime;
            if (m_currentCharge >= m_desiredCharge) m_currentCharge = m_desiredCharge;
        }
        else
        {
            if (_chargePart) Destroy(_chargePart);
            if (m_as.clip == m_chargeSound) m_as.Stop();

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

        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        vel *= 0.95f;
        transform.position += vel * Time.deltaTime;

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_restPos, m_returnRate);
    }

    void Shoot()
    {
        OnShoot.Invoke();

        m_anim.SetTrigger("Fire");

        m_beam.transform.parent = m_shootPos.transform;
        m_beam.transform.localPosition = Vector3.zero;
        m_beam.transform.localRotation = Quaternion.identity;

        GameObject _shootPart = Instantiate(m_shootPartPrefab, m_shootPos.position, transform.rotation, null);
        Destroy(_shootPart, 3.0f);

        m_as.Stop();
        m_as.clip = m_shootSound;
        m_as.loop = false;
        if (!m_as.isPlaying) m_as.Play();

        vel -= transform.right * m_recoilMulti;

        m_lastShotTime = Time.time;

        RaycastHit2D[] hits = Physics2D.RaycastAll(m_shootPos.position, transform.right);

        System.Array.Sort(hits, (a, b) => { return a.distance.CompareTo(b.distance); });

        float hitDist = 20;

        foreach (RaycastHit2D _hit in hits)
        {
            if (_hit && _hit.transform)
            {

                GameObject _hitPart = Instantiate(m_hitPartPrefab, _hit.point, Quaternion.identity, null);
                Destroy(_hitPart, 3.0f);

                EnemyTest _e = _hit.transform.GetComponent<EnemyTest>();
                if (_e)
                {
                    _e.TakeDamage(m_Damage, m_damageType);
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

    public void UpdateVisuals()
    {
        m_beam.transform.localScale = Vector3.Lerp(m_beam.transform.localScale, new Vector3(m_beam.transform.localScale.x, 0, 1), 5.0f * Time.deltaTime);

        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.color = new Color(1, (Time.time - m_lastShotTime) / m_cooldownWait, (Time.time - m_lastShotTime) / m_cooldownWait);
    }

    public void AddDamage(float _amount)
    {
        m_Damage += _amount;
    }

    public void IncreaseChargeRate(float _amount)
    {
        m_chargePerSecond += _amount;
    }
}
