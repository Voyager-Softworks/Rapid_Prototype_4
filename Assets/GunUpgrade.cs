using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class GunUpgrade : MonoBehaviour
{
    public UnityEvent Bought;
    public UnityEvent Failed;

    [SerializeField] public string m_message;
    
    [SerializeField] public GameObject m_player;

    [SerializeField] public List<Upgrade.ResourceCost> m_costs;

    Image m_image;
    Button m_button;
    TextMeshProUGUI m_text;

    void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");

        m_image = GetComponent<Image>();
        m_button = GetComponent<Button>();
        m_text = GetComponentInChildren<TextMeshProUGUI>();

        UpdateText();
    }

    private void UpdateText()
    {
        m_text.text = m_message;
        foreach (Upgrade.ResourceCost _cost in m_costs)
        {
            switch (_cost.m_type)
            {
                case Resource.Type.Organic:
                    m_text.text += ", " + _cost.m_amount + " Organic";
                    break;

                case Resource.Type.Power:
                    m_text.text += ", " + _cost.m_amount + " Power";
                    break;

                case Resource.Type.Scrap:
                    m_text.text += ", " + _cost.m_amount + " Scrap";
                    break;
            }
        }
    }

    public void TryUpgradeGun()
    {
        bool hasAll = true;

        foreach (Upgrade.ResourceCost _cost in m_costs)
        {
            if (!m_player.GetComponent<Inventory>().DoesHave(_cost.m_type, _cost.m_amount))
            {
                hasAll = false;
                break;
            }
        }

        if (hasAll)
        {
            foreach (Upgrade.ResourceCost _cost in m_costs)
            {
                m_player.GetComponent<Inventory>().Remove(_cost.m_type, _cost.m_amount);
            }

            Bought.Invoke();
            m_button.enabled = false;
            m_text.text = m_message + " BOUGHT";
        }
        else
        {
            Failed.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
