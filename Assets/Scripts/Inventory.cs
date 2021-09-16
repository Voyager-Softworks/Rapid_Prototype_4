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
        UpdateVisuals();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateVisuals()
    {
        m_text.text = "ORGANIC  x" + m_organicAmount +
                        "\nPOWER    x" + m_powerAmount +
                        "\nSCRAP    x" + m_scrapAmount;
    }

    public void AddResource(Resource.Type _type, float _amount = 1.0f)
    {
        switch (_type)
        {
            case Resource.Type.Organic:
                AddOrganic(_amount);
                break;

            case Resource.Type.Power:
                AddPower(_amount);
                break;

            case Resource.Type.Scrap:
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

    public bool DoesHave(Resource.Type _type, float _amount)
    {
        switch (_type)
        {
            case Resource.Type.Organic:
                if (m_organicAmount < _amount) return false;
                break;

            case Resource.Type.Power:
                if (m_powerAmount < _amount) return false;
                break;

            case Resource.Type.Scrap:
                if (m_scrapAmount < _amount) return false;
                break;

            default:
                return false;
        }

        return true;
    }

    public void Remove(Resource.Type _type, float _amount)
    {
        switch (_type)
        {
            case Resource.Type.Organic:
                m_organicAmount = m_organicAmount <= _amount ? 0 : m_organicAmount - _amount;
                break;

            case Resource.Type.Power:
                m_powerAmount = m_powerAmount <= _amount ? 0 : m_powerAmount - _amount;
                break;

            case Resource.Type.Scrap:
                m_scrapAmount = m_scrapAmount <= _amount ? 0 : m_scrapAmount - _amount;
                break;

            default:
                break;
        }

        UpdateVisuals();
    }

    public void Drop(Resource.Type _type, float _percentage)
    {
        int dropamount = 0;
        switch (_type)
        {
            case Resource.Type.Organic:
                dropamount = (int)(m_organicAmount * (_percentage / 100.0f));
                m_organicAmount = 0;
                break;

            case Resource.Type.Power:
                dropamount = (int)(m_powerAmount * (_percentage / 100.0f));
                m_powerAmount = 0;
                break;

            case Resource.Type.Scrap:
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
                case Resource.Type.Organic:
                    Instantiate(m_organicPrefab, gameObject.transform.position, Quaternion.identity);
                    break;
                case Resource.Type.Power:
                    Instantiate(m_powerPrefab, gameObject.transform.position, Quaternion.identity);
                    break;
                case Resource.Type.Scrap:
                    Instantiate(m_scrapPrefab, gameObject.transform.position, Quaternion.identity);
                    break;
                default:
                    break;
            }

        }
    }
}
