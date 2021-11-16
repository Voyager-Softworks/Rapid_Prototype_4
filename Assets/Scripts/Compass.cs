using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public Transform m_needle, m_objToPointAt, m_playerTransform;
    public List<Image> m_images;
    public float m_DisplayDistance;
    // Start is called before the first frame update
    void Start()
    {
        if (!m_playerTransform)
        {
            m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_objToPointAt || !m_needle || !m_playerTransform)
        {
            return;
        }
        
        Vector3 perpendicular = m_playerTransform.position - m_objToPointAt.position;
        m_needle.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
        foreach (var img in m_images)
        {
            img.color = new Color(1, 1, 1, (m_playerTransform.position - m_objToPointAt.position).magnitude < m_DisplayDistance ? 0 : 1);
        }
    }
}
