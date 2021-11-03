using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float m_Damage = 1.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.CompareTag("Player"))
        {
            other.collider.gameObject.GetComponent<PlayerHealth>().Damage(m_Damage);
        }
        if (GetComponent<SFX_Effect>())
        {
            GetComponent<SFX_Effect>().Play();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().Damage(m_Damage);
        }
        if (GetComponent<SFX_Effect>())
        {
            GetComponent<SFX_Effect>().Play();
        }
    }
}
