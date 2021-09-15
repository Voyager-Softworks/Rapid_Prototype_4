using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [System.Serializable]
    public class Drop
    {
        public GameObject m_prefab;
        public float m_amount;
    }

    [SerializeField] float m_health = 100.0f;
    bool m_alive = true;


    [SerializeField] List<Drop> m_drops;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -10)
        {
            Revive();
        }
    }

    public void TakeDamage(float _amount)
    {
        m_health -= _amount;

        if (m_alive && m_health <= 0)
        {
            Die();
        }

        UpdateVisuals();
    }

    private void Revive()
    {
        m_alive = true;
        m_health = 100;
        transform.position = new Vector3(4, 5, 0);
        transform.rotation = Quaternion.identity;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        GetComponent<Rigidbody2D>().mass *= 2.0f;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        UpdateVisuals();
    }

    public void Die()
    {
        m_alive = false;
        m_health = 0;
        GetComponent<Rigidbody2D>().freezeRotation = false;
        GetComponent<Rigidbody2D>().AddTorque(1);
        Destroy(gameObject, 1.0f);

        Destroy(GetComponent<NavAgent>());

        foreach (Drop _drop in m_drops)
        {
            for (int i = 0; i < _drop.m_amount; i++)
            {
                GameObject newDrop = Instantiate(_drop.m_prefab, transform.position + transform.up, Quaternion.identity, null);
            }
        }
    }

    private void OnDestroy()
    {
        
    }

    public void UpdateVisuals()
    {
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.color = new Color(1, m_health / 100, m_health / 100);
    }
}
