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
        m_text.text =     "ORGANIC  x" + m_organicAmount +
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
}
