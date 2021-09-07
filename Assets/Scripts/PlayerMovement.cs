using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float m_moveSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vel = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            vel += Vector2.up * 2.0f; 
        }
        if (Input.GetKey(KeyCode.S))
        {
            vel += Vector2.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vel += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            vel += Vector2.right;
        }

        vel = vel * m_moveSpeed;

        rb.velocity += vel * Time.deltaTime;
    }
}
