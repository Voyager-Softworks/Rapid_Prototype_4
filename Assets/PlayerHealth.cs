using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public UnityEvent OnDeath;
    public UnityEvent OnDamage;

    public UnityEvent OnRespawn;

    public Transform m_respawnPos;

    public bool m_dead = false;
    public float m_deathTimer = 1.0f;
    public float m_playerHealth;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_dead && m_deathTimer > 0.0f) m_deathTimer -= Time.deltaTime;
        else if (m_dead)
        {
            m_dead = false;
            GetComponent<Animator>().SetBool("Dead", false);
            transform.position = m_respawnPos.position;
            OnRespawn.Invoke();

        }
    }
    public void Damage(float _dmg)
    {
        if (m_dead) return;
        m_playerHealth -= _dmg;
        OnDamage.Invoke();
        if (m_playerHealth <= 0.0f)
        {
            m_playerHealth = 0.0f;
            OnDeath.Invoke();
            m_dead = true;
            GetComponent<Animator>().SetBool("Dead", true);
            GetComponent<Animator>().speed = 1.0f;

        }
    }
}
