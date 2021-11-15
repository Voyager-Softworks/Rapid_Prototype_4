using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// public class Upgrade : MonoBehaviour
// {

//     [System.Serializable]
//     public class ResourceCost
//     {
//         [SerializeField] public Resources.ResourceType m_type;
//         [SerializeField] public float m_amount;
//     }

//     [SerializeField] uint m_currentLevel = 0;

//     [System.Serializable]
//     public class UpgradeCost
//     {
//         [SerializeField] public ResourceCost[] m_costs;
//         [SerializeField] public UnityEvent OnUpgrade;
//     }

//     [SerializeField] GameObject m_player;
//     [SerializeField] GameObject m_equipUI;
//     [SerializeField] GameObject m_upgradeUI;
//     [SerializeField] GameObject m_repairUI;
//     [SerializeField] GameObject m_upgradeText;
//     [SerializeField] float m_upgradeRange;

//     [SerializeField] List<UpgradeCost> m_upgrades;

//     public UnityEvent Upgraded;
//     public UnityEvent Failed;
//     public UnityEvent HitMaxLevel;


//     // Start is called before the first frame update
//     void Start()
//     {
//         if(!m_player) m_player = GameObject.Find("Player");
//     }

//     private void OnDrawGizmos()
//     {
//         Gizmos.DrawWireSphere(transform.position, m_upgradeRange);
//     }

//     bool open = false;

//     // Update is called once per frame
//     void Update()
//     {
//         float dist = Vector2.Distance(m_player != null ? m_player.transform.position : Vector3.zero, transform.position);

//         if (dist < m_upgradeRange)
//         {
//             if (!open)
//             {
//                 GameObject.Find("OpenSound").GetComponent<AudioSource>().Play();
//                 UpdateText();
//             }
//             open = true;

//             m_equipUI.SetActive(true);
//             m_upgradeUI.SetActive(true);
//             m_repairUI.SetActive(true);
//             m_upgradeText.SetActive(true);

//             if (Keyboard.current.eKey.wasPressedThisFrame)
//             {
//                 TryBuyUpgrade();
//             }
//         }
//         else
//         {
//             open = false;

//             if (m_equipUI.activeSelf) m_equipUI.SetActive(false);
//             if (m_upgradeUI.activeSelf) m_upgradeUI.SetActive(false);
//             if (m_repairUI.activeSelf) m_repairUI.SetActive(false);
//             if (m_upgradeText.activeSelf) m_upgradeText.SetActive(false);
//         }
//     }

//     private void UpdateText()
//     {
//         if (m_upgrades.Count <= m_currentLevel) return;

//         string costString = "";

//         foreach (ResourceCost _cost in m_upgrades[(int)m_currentLevel].m_costs)
//         {
//             switch (_cost.m_type)
//             {
//                 case Resources.ResourceType.ORGANIC:
//                     costString += _cost.m_amount + " Organic ";
//                     break;
//                 case Resources.ResourceType.POWER:
//                     costString += _cost.m_amount + " Power ";
//                     break;
//                 case Resources.ResourceType.SCRAP:
//                     costString += _cost.m_amount + " Scrap ";
//                     break;
//             }
//         }

//         m_upgradeText.GetComponent<TextMeshProUGUI>().text = "[E] To Upgrade\n(" + costString + ")";
//     }

//     private void TryBuyUpgrade()
//     {
//         if (m_upgrades.Count > m_currentLevel)
//         {
//             bool hasAll = true;
//             foreach (ResourceCost _cost in m_upgrades[(int)m_currentLevel].m_costs)
//             {
//                 if (!m_player.GetComponent<Inventory>().DoesHave(_cost.m_type, _cost.m_amount))
//                 {
//                     hasAll = false;
//                     break;
//                 }
//             }

//             if (hasAll)
//             {
//                 foreach (ResourceCost _cost in m_upgrades[(int)m_currentLevel].m_costs)
//                 {
//                     m_player.GetComponent<Inventory>().Remove(_cost.m_type, _cost.m_amount);
//                 }

//                 DoUpgrade();
//             }
//             else
//             {
//                 Failed.Invoke();
//             }
//         }
//     }

//     private void DoUpgrade()
//     {
//         m_upgrades[(int)m_currentLevel].OnUpgrade.Invoke();
//         m_currentLevel++;
//         Upgraded.Invoke();
//         if (m_currentLevel == m_upgrades.Count) HitMaxLevel.Invoke();
//         UpdateText();
//     }
// }
