using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Resource : MonoBehaviour
{

    [SerializeField] GameObject m_player;

    [SerializeField] Resources.ResourceType m_type;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_player) m_player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector2.Distance(m_player.transform.position, transform.position);
        if (dist < 3)
        {
            GetComponent<Rigidbody2D>().velocity += (Vector2)((m_player.transform.position - transform.position).normalized) * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == m_player.transform)
        {
            m_player.GetComponent<Inventory>().AddResource(m_type);
            Destroy(gameObject, 1.0f);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<AudioSource>().Play();
        }
    }

    public Resources.ResourceType GetResourceType()
    {
        return m_type;
    }
}
