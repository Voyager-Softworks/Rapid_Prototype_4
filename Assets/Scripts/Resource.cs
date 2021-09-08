using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Resource : MonoBehaviour
{
    public InputAction m_pickupAction;

    public enum Type
    {
        Organic,
        Power,
        Scrap
    }

    [SerializeField] GameObject m_player;
    [SerializeField] Canvas m_canvas;
    [SerializeField] TextMeshProUGUI m_pickupText;

    [SerializeField] Type m_type;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");
        m_pickupText.fontSize = 1;
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.transform.rotation = Quaternion.identity;
        m_canvas.transform.position = transform.position + Vector3.up;

        if (Vector2.Distance(m_player.transform.position, transform.position) < 3)
        {
            m_pickupText.enabled = true;

            if (Keyboard.current.eKey.isPressed)
            {
                m_player.GetComponent<Inventory>().AddResource(m_type);
                Destroy(gameObject);
            }
        }
        else
        {
            m_pickupText.enabled = false;
        }
    }

    public Type GetResourceType()
    {
        return m_type;
    }
}
