using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float tickRate = 1.0f;
    public float timeToNextTick = 0.0f;

    public float tickDamage = 1.0f;
    public float slowedMaxVelocity = 0.5f;
    public PlayerHealth playerHealth;
    public Rigidbody2D currentRigidbody;

    void Start()
    {
        timeToNextTick = tickRate;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        timeToNextTick -= Time.deltaTime;
        if (currentRigidbody != null)
        {
            if (timeToNextTick <= 0)
            {
                timeToNextTick = tickRate;
                playerHealth.Damage(tickDamage);
            }
            if(currentRigidbody.velocity.y < slowedMaxVelocity)
            {
                currentRigidbody.velocity = new Vector2(currentRigidbody.velocity.x, slowedMaxVelocity);
            }
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            currentRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        currentRigidbody = null;
    }

    

    
}
