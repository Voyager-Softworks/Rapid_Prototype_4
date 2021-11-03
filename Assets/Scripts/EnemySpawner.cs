using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject m_player;
    [SerializeField] GameObject m_town;

    [SerializeField] GameObject m_feralMechPrefab;
    GameObject m_feralMechInstance;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Vector2.Distance(m_player.transform.position, m_town.transform.position) <= 4.0f)
        {
            TrySpawnMech();
        }
    }

    private void TrySpawnMech()
    {
        if (!m_feralMechInstance) m_feralMechInstance = Instantiate(m_feralMechPrefab, transform.position, Quaternion.identity, null);
    }
}
