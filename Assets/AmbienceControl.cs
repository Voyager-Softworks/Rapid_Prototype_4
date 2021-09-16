using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceControl : MonoBehaviour
{
    [SerializeField] float m_threshold = 6.0f;
    [SerializeField] AudioClip m_upperSound;
    [SerializeField] AudioClip m_lowerSound;
    [SerializeField] AudioSource m_as;

    // Start is called before the first frame update
    void Start()
    {
        m_as = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= m_threshold)
        {
            if (m_as.clip == m_upperSound && m_as.volume > 0.0f)
            {
                m_as.volume -= Time.deltaTime;
            }
            else if (m_as.clip == m_upperSound && m_as.volume <= 0.0f)
            {
                m_as.clip = m_lowerSound;
                m_as.Play();
            }
            else if (m_as.clip == m_lowerSound && m_as.volume < 1.0f)
            {
                m_as.volume += Time.deltaTime;
            }
        }
        else
        {
            if (m_as.clip == m_lowerSound && m_as.volume > 0.0f)
            {
                m_as.volume -= Time.deltaTime;
            }
            else if (m_as.clip == m_lowerSound && m_as.volume <= 0.0f)
            {
                m_as.clip = m_upperSound;
                m_as.Play();
            }
            else if (m_as.clip == m_upperSound && m_as.volume < 1.0f)
            {
                m_as.volume += Time.deltaTime;
            }
        }

    }
}
