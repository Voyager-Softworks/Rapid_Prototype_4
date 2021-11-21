using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarVisual : MonoBehaviour
{
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

        float val = ph.m_playerHealth / 100.0f;

        transform.localScale = new Vector3(val * 1.0f, 0.3f, 1.0f);
    }
}
