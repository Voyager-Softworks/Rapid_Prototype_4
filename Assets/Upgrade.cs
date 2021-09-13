using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Upgrade : MonoBehaviour
{

    [System.Serializable]
    public class ResourceCost
    {
        [SerializeField] public Resource.Type m_type;
        [SerializeField] public float m_amount;
    }

    [SerializeField] uint m_currentLevel = 0;

    [System.Serializable]
    public class UpgradeCost
    {
        [SerializeField] public ResourceCost[] m_costs;
    }

    [SerializeField] GameObject m_player;
    [SerializeField] float m_upgradeRange;

    [SerializeField] List<UpgradeCost> m_upgrades;

    public UnityEvent Upgraded;
    public UnityEvent Failed;
    public UnityEvent HitMaxLevel;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_upgradeRange);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && Vector2.Distance(m_player.transform.position, transform.position) < m_upgradeRange)
        {
            TryBuyUpgrade();
        }
    }

    private void TryBuyUpgrade()
    {
        if (m_upgrades.Count > m_currentLevel)
        {
            bool hasAll = true;
            foreach (ResourceCost _cost in m_upgrades[(int)m_currentLevel].m_costs)
            {
                if (!m_player.GetComponent<Inventory>().DoesHave(_cost.m_type, _cost.m_amount))
                {
                    hasAll = false;
                    break;
                }
            }

            if (hasAll)
            {
                foreach (ResourceCost _cost in m_upgrades[(int)m_currentLevel].m_costs)
                {
                    m_player.GetComponent<Inventory>().Remove(_cost.m_type, _cost.m_amount);
                }

                DoUpgrade();
            }
            else
            {
                Failed.Invoke();
            }
        }
    }

    private void DoUpgrade()
    {
        m_currentLevel++;
        Upgraded.Invoke();
        if (m_currentLevel == m_upgrades.Count) HitMaxLevel.Invoke();
    }
}
