using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    public UnityEvent OnDeath;
    public UnityEvent OnDamage;
    public UnityEvent OnShieldDamage;
    public UnityEvent OnShieldPop;

    public UnityEvent OnRespawn;

    public Transform m_respawnPos;

    public bool m_dead = false;
    public float m_deathTimer = 2.0f;
    public float m_playerHealth;
    public float m_playerShield;
    public Color m_shieldDamageColor;
    public Color m_damageColor;
    public Color m_shieldPopColor;
    public float m_shieldRegenRate = 1.0f;

    public float m_shieldRegenDelay = 1.0f;
    float m_shieldRegenTimer = 0.0f;
    // Start is called before the first frame update

    private GameObject persistent = null;
    void Start()
    {
        if (!persistent)
        {
            persistent = DontDestroy.instance;
        }

        m_deathTimer = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_dead && m_playerHealth <= 0.0f)
        {
            Die();
        }

        if(m_shieldRegenTimer > 0.0f)
        {
            m_shieldRegenTimer -= Time.deltaTime;
        }
        else
        {
            m_playerShield = Mathf.Min(m_playerShield + m_shieldRegenRate * Time.deltaTime, 100.0f);
        }

        if (m_dead)
        {
            m_deathTimer -= Time.deltaTime;

            if (m_deathTimer <= 0.0f)
            {
                persistent.GetComponent<SceneController>().LoadLevel(LevelManager.LevelType.HUB);
            }
        }
    }

    private void Die()
    {
        m_playerHealth = 0.0f;
        OnDeath.Invoke();
        m_dead = true;
        GetComponent<Animator>().SetBool("Dead", true);
        GetComponent<Animator>().speed = 1.0f;

        m_deathTimer = 2.0f;

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        //loop through all of the weapons and disable them
        foreach (PlayerWeapons.WeaponEquip weapon in GetComponent<PlayerWeapons>().leftWeapons)
        {
            weapon.weapon.SetActive(false);
        }
        foreach (PlayerWeapons.WeaponEquip weapon in GetComponent<PlayerWeapons>().rightWeapons)
        {
            weapon.weapon.SetActive(false);
        }

        Light2D[] light = GetComponentsInChildren<Light2D>(true);
        foreach (Light2D l in light)
        {
            l.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void TryRepair()
    {
        bool hasAll = true;

        //TODO repair code
        // if (!GetComponent<Inventory>().DoesHave(Resources.ResourceType.SCRAP, 1))
        // {
        //     hasAll = false;
        // }

        // if (hasAll)
        // {
        //     GetComponent<Inventory>().Remove(Resources.ResourceType.SCRAP, 1);

        //     m_playerHealth += 25.0f;
        // }

        m_playerHealth = Mathf.Clamp(m_playerHealth, 0, 100.0f);
    }

    //Coroutine that changes the color of the player sprite then returns it to normal
    public IEnumerator ChangeColor(Color color)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    public void Damage(float _dmg)
    {
        if (m_dead) return;
        if (m_playerShield > 0.0f)
        {
            m_playerShield -= _dmg;
            if(m_playerShield <= 0.0f)
            {
                OnShieldPop.Invoke();
                StartCoroutine(ChangeColor(m_shieldPopColor));
            }
            else
            {
                OnShieldDamage.Invoke();
                StartCoroutine(ChangeColor(m_shieldDamageColor));
            }
            m_playerShield = Mathf.Clamp(m_playerShield, 0.0f, 75.0f);
            m_shieldRegenTimer = m_shieldRegenDelay;
            
        }
        else
        {
            m_playerHealth -= _dmg;
            m_shieldRegenTimer = m_shieldRegenDelay;
            m_playerHealth = Mathf.Clamp(m_playerHealth, 0.0f, 150.0f);
            StartCoroutine(ChangeColor(m_damageColor));
            OnDamage.Invoke();
        }
    }
}
