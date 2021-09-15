using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public UnityEvent OnDeath;
    public float m_playerHealth;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Damage(float _dmg)
    {
        m_playerHealth -= _dmg;
        if (m_playerHealth <= 0.0f)
        {
            m_playerHealth = 0.0f;
            OnDeath.Invoke();
            GetComponent<Animator>().SetBool("Dead", true);
            GetComponent<Animator>().speed = 1.0f;

        }
    }
}
