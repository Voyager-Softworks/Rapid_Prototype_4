using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float m_Damage = 1.0f;
    public float m_timeBeforeAutoDestroy = 5.0f;
    public bool m_spawnExplosion = true;
    public GameObject m_ExplosionPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_timeBeforeAutoDestroy -= Time.deltaTime;
        if (m_timeBeforeAutoDestroy <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyTest>().TakeDamage(m_Damage, EnemyTest.DamageType.Universal);
        }
        if(m_spawnExplosion)
        {
        //Instantiate an explosion using the normal of the collision
        Vector3 explosionPos = transform.position;
        Quaternion explosionRot = Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal);
        Instantiate(m_ExplosionPrefab, explosionPos, explosionRot);
        Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyTest>().TakeDamage(m_Damage, EnemyTest.DamageType.Universal);
        }
        if (m_spawnExplosion)
        {
         //Instantiate an explosion using the normal of the collision
        Vector3 explosionPos = transform.position;
        Quaternion explosionRot = Quaternion.FromToRotation(Vector3.up, other.transform.position - transform.position);
        Instantiate(m_ExplosionPrefab, explosionPos, explosionRot);
        Destroy(gameObject);
        }
    }
}
