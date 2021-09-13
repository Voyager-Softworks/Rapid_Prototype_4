using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] float m_health = 100.0f;
    bool m_healthy = true;

    [SerializeField] List<float> m_spawnResourceAtHealth = new List<float>{ 0.0f };

    [SerializeField] GameObject m_resourcePrefab;

    public void TakeDamage(float _amount)
    {
        if (!m_healthy) return;

        m_health -= _amount;

        CheckSpawns();

        if (m_health <= 0)
        {
            m_healthy = false;
            m_health = 0;

            Break();
        }
    }

    void CheckSpawns()
    {
        for (int i = m_spawnResourceAtHealth.Count - 1; i >= 0; i--){
            if (m_health <= m_spawnResourceAtHealth[i])
            {
                GameObject _resource = Instantiate(m_resourcePrefab, transform.position + transform.up, Quaternion.identity, null);

                m_spawnResourceAtHealth.RemoveAt(i);
            }
        }
    }

    void Break()
    {
        Destroy(gameObject);
    }
}
