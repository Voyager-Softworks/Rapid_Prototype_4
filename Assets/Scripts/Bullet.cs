using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D m_rb;
    AudioSource m_as;

    [SerializeField] public float m_baseDamage = 2.0f;
    [SerializeField] public float m_damageMulti = 1.0f;
    [SerializeField] public float m_damageAdd = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_as = GetComponent<AudioSource>();
        m_rb = GetComponent<Rigidbody2D>();

        m_as.pitch = Random.Range(0.9f, 1.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_rb.velocity.magnitude <= 0.5f) Destroy(gameObject, 1.0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyTest _e = collision.transform.GetComponent<EnemyTest>();

        if (_e)
        {
            _e.TakeDamage((m_baseDamage * m_damageMulti) + m_damageAdd);
        }
    }
}
