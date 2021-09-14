using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D m_rb;

    [SerializeField] public float m_baseDamage = 2.0f;
    [SerializeField] public float m_damageMulti = 1.0f;
    [SerializeField] public float m_damageAdd = 0.0f;

    bool m_active = true;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_rb.velocity.magnitude <= 0.5f) Destroy(gameObject, 1.0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!m_active) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.0f);

        foreach (Collider2D _hit in hits)
        {
            EnemyTest _e = _hit.transform.GetComponent<EnemyTest>();
            if (_e)
            {
                _e.TakeDamage((m_baseDamage * m_damageMulti) + m_damageAdd);
            }

            ResourceNode _rn = _hit.transform.GetComponent<ResourceNode>();
            if (_rn)
            {
                _rn.TakeDamage(10.0f);
            }
        }

        Destroy(gameObject, 0.01f);
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<TrailRenderer>().enabled = false;
        m_active = false;
    }
}
