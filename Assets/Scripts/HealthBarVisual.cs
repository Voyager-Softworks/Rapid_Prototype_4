using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarVisual : MonoBehaviour
{
    public enum HealthBarType
    {
        HEALTH,
        SHIELD
    }

    public HealthBarType m_Type;
    public Image m_HealthBar;
    [SerializeField] GameObject m_player;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerHealth ph = m_player.GetComponent<PlayerHealth>();

        float val = 0.0f;
        switch (m_Type)
        {
            case HealthBarType.HEALTH:
                val = ph.m_playerHealth / 150.0f;
                break;
            case HealthBarType.SHIELD:
                val = ph.m_playerShield / 75.0f;
                break;
        }

        
        m_HealthBar.fillAmount = val;
    }
}
