using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] float m_health = 100.0f;
    bool m_alive = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    public void TakeDamage(float _amount)
    {
        m_health -= _amount;

        if (m_alive && m_health <= 0)
        {
            m_alive = false;
            m_health = 0;
            GetComponent<Rigidbody2D>().freezeRotation = false;
            GetComponent<Rigidbody2D>().mass *= 0.5f;
        }

        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.color = new Color(1, m_health / 100, m_health / 100);
    }
}
