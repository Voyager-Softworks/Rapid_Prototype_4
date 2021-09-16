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
        [SerializeField] public UnityEvent OnUpgrade;
    }

    [SerializeField] GameObject m_player;
    [SerializeField] GameObject m_equipUI;
    [SerializeField] GameObject m_upgradeUI;
    [SerializeField] GameObject m_repairUI;
    [SerializeField] float m_upgradeRange;

    [SerializeField] List<UpgradeCost> m_upgrades;

    public UnityEvent Upgraded;
    public UnityEvent Failed;
    public UnityEvent HitMaxLevel;


    // Start is called before the first frame update
    void Start()
    {
        if(!m_player) m_player = GameObject.Find("Player");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_upgradeRange);
    }

    bool open = false;

    // Update is called once per frame
    void Update()
    {
        float dist = Vector2.Distance(m_player.transform.position, transform.position);

        if (dist < m_upgradeRange)
        {
            if (!open)
            {
                GameObject.Find("OpenSound").GetComponent<AudioSource>().Play();
            }
            open = true;

            m_equipUI.SetActive(true);
            m_upgradeUI.SetActive(true);
            m_repairUI.SetActive(true);

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryBuyUpgrade();
            }
        }
        else
        {
            open = false;

            if (m_equipUI.activeSelf) m_equipUI.SetActive(false);
            if (m_upgradeUI.activeSelf) m_upgradeUI.SetActive(false);
            if (m_repairUI.activeSelf) m_repairUI.SetActive(false);
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
        m_upgrades[(int)m_currentLevel].OnUpgrade.Invoke();
        m_currentLevel++;
        Upgraded.Invoke();
        if (m_currentLevel == m_upgrades.Count) HitMaxLevel.Invoke();
    }
}
