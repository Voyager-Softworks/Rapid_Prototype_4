using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        BANDIT_GRUNT,
        BANDIT_BIKE,
        BANDIT_TANK,
        BANDIT_TURRET,
        ALIEN_DRONE,
        ALIEN_MECH,
        MUTANT_CRAWLER,
        MUTANT_ABOMINATION,
        MUTANT_TURRET
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

    public GameObject m_healthBar;
    public GameObject m_healthBarPREFAB;

    // Start is called before the first frame update
    void Start()
    {
        startHealth = m_health;
    }

    // Update is called once per frame
    void Update()
    {
        //get the sprite
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        //get the size of the sprite
        Vector2 size = sprite.bounds.size;

        //get the position of the sprite
        Vector3 pos = transform.position;

        if (m_healthBar == null)
        {
            m_healthBar = Instantiate(m_healthBarPREFAB, pos, Quaternion.identity, transform);
        }

        //set thje position of the health bar
        m_healthBar.transform.position = new Vector3(pos.x, pos.y + size.y / 2, 0);

        //set the scale to match the size of the sprite
        m_healthBar.transform.localScale = new Vector3(size.x / transform.localScale.x, 1 / transform.localScale.y, 1);

        //set the health bar to the correct size
        m_healthBar.GetComponentsInChildren<RectTransform>()[2].sizeDelta = new Vector2(m_health / startHealth, m_healthBar.GetComponentsInChildren<RectTransform>()[1].sizeDelta.y);

        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend.color != Color.white)
        {
            rend.color += new Color(Time.deltaTime,Time.deltaTime,Time.deltaTime) * 5.0f;

            //if too white, set to white
            if (rend.color.g > 1.0f)
            {
                rend.color = Color.white;
            }
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

        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.color = new Color(1, 0, 0);

        if (m_alive && m_health <= 0)
        {
            Die();
        }
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
        Destroy(gameObject, 0.0f);

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
}
