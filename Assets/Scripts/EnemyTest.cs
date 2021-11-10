using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public enum DamageType
    {
        Minigun,
        Railgun,
        Universal
    }

    public enum EnemyType
    {
        NONE,
        ALIEN_SMALL,
        ALIEN_LARGE,
        ALIENT_FLYING,
        MECH_SMALL,
        MECH_MEDIUM,
        MECH_LARGE,
    }

    [System.Serializable]
    public class Drop
    {
        public GameObject m_prefab;
        public float m_amount;
    }

    public EnemyType m_enemyType = EnemyType.NONE;

    [SerializeField] float m_health = 100.0f;
    float startHealth = 0;
    bool m_alive = true;

    public bool m_elite = false;
    [SerializeField] GameObject m_deathPart;

    [SerializeField] DamageType m_weakTo;

    [SerializeField] List<Drop> m_drops;

    // Start is called before the first frame update
    void Start()
    {
        startHealth = m_health;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -10)
        {
            Revive();
        }
    }

    public void TakeDamage(float _amount, DamageType _type)
    {
        if (m_weakTo == _type)
        {
            _amount *= 1.5f;
        }
        else if (_type == DamageType.Universal)
        {
            _amount *= 1.0f;
        }
        else
        {
            _amount *= 0.3f;
        }

        m_health -= _amount;

        if (m_alive && m_health <= 0)
        {
            Die();
        }

        UpdateVisuals();
    }

    private void Revive()
    {
        m_alive = true;
        m_health = 100;
        transform.position = new Vector3(4, 5, 0);
        transform.rotation = Quaternion.identity;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        GetComponent<Rigidbody2D>().mass *= 2.0f;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        UpdateVisuals();
    }

    public void Die()
    {
        GameObject persistent = GameObject.Find("Persistent");
        if (persistent){
            PlayerStats stats = persistent.GetComponent<PlayerStats>();
            if (stats)
            {
                stats.AddKill(gameObject, false);
            }
        }

        m_alive = false;
        m_health = 0;
        GetComponent<Rigidbody2D>().freezeRotation = false;
        GetComponent<Rigidbody2D>().AddTorque(1);
        Destroy(gameObject, 1.0f);

        Destroy(GetComponent<NavAgent>());

        foreach (Drop _drop in m_drops)
        {
            for (int i = 0; i < _drop.m_amount; i++)
            {
                GameObject newDrop = Instantiate(_drop.m_prefab, transform.position + transform.up, Quaternion.identity, null);
            }
        }
    }

    private void OnDestroy()
    {
        if (!this.gameObject.scene.isLoaded) return;

        Destroy(Instantiate(m_deathPart, transform.position, Quaternion.identity, null), 4.0f);
    }

    public void UpdateVisuals()
    {
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.color = new Color(1, m_health / startHealth, m_health / startHealth);
    }
}
