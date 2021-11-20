using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] float m_organicAmount = 0;
    [SerializeField] float m_powerAmount = 0;
    [SerializeField] float m_scrapAmount = 0;

    [SerializeField] Text m_text;

    [Header("Prefabs")]
    [SerializeField] GameObject m_powerPrefab;
    [SerializeField] GameObject m_organicPrefab;
    [SerializeField] GameObject m_scrapPrefab;

    private void Start()
    {
        //UpdateVisuals();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateVisuals()
    {
        return;
    }

    public void AddResource(Resources.ResourceType _type, float _amount = 1.0f)
    {
        switch (_type)
        {
            case Resources.ResourceType.ORGANIC:
                AddOrganic(_amount);
                break;

            case Resources.ResourceType.POWER:
                AddPower(_amount);
                break;

            case Resources.ResourceType.SCRAP:
                AddScrap(_amount);
                break;

            default:
                break;
        }
    }

    public void AddOrganic(float _amount = 1.0f)
    {
        m_organicAmount += _amount;
        UpdateVisuals();
    }

    public void AddPower(float _amount = 1.0f)
    {
        m_powerAmount += _amount;
        UpdateVisuals();
    }

    public void AddScrap(float _amount = 1.0f)
    {
        m_scrapAmount += _amount;
        UpdateVisuals();
    }

    public bool DoesHave(Resources.ResourceType _type, float _amount)
    {
        switch (_type)
        {
            case Resources.ResourceType.ORGANIC:
                if (m_organicAmount < _amount) return false;
                break;

            case Resources.ResourceType.POWER:
                if (m_powerAmount < _amount) return false;
                break;

            case Resources.ResourceType.SCRAP:
                if (m_scrapAmount < _amount) return false;
                break;

            default:
                return false;
        }

        return true;
    }

    public void Remove(Resources.ResourceType _type, float _amount)
    {
        switch (_type)
        {
            case Resources.ResourceType.ORGANIC:
                m_organicAmount = m_organicAmount <= _amount ? 0 : m_organicAmount - _amount;
                break;

            case Resources.ResourceType.POWER:
                m_powerAmount = m_powerAmount <= _amount ? 0 : m_powerAmount - _amount;
                break;

            case Resources.ResourceType.SCRAP:
                m_scrapAmount = m_scrapAmount <= _amount ? 0 : m_scrapAmount - _amount;
                break;

            default:
                break;
        }

        UpdateVisuals();
    }

    public void Drop(Resources.ResourceType _type, float _percentage)
    {
        int dropamount = 0;
        switch (_type)
        {
            case Resources.ResourceType.ORGANIC:
                dropamount = (int)(m_organicAmount * (_percentage / 100.0f));
                m_organicAmount = 0;
                break;

            case Resources.ResourceType.POWER:
                dropamount = (int)(m_powerAmount * (_percentage / 100.0f));
                m_powerAmount = 0;
                break;

            case Resources.ResourceType.SCRAP:
                dropamount = (int)(m_scrapAmount * (_percentage / 100.0f));
                m_scrapAmount = 0;
                break;

            default:
                break;
        }

        UpdateVisuals();
        for (int i = 0; i <= dropamount; i++)
        {
            switch (_type)
            {
                case Resources.ResourceType.ORGANIC:
                    Instantiate(m_organicPrefab, gameObject.transform.position, Quaternion.identity);
                    break;
                case Resources.ResourceType.POWER:
                    Instantiate(m_powerPrefab, gameObject.transform.position, Quaternion.identity);
                    break;
                case Resources.ResourceType.SCRAP:
                    Instantiate(m_scrapPrefab, gameObject.transform.position, Quaternion.identity);
                    break;
                default:
                    break;
            }

        }
    }
}
