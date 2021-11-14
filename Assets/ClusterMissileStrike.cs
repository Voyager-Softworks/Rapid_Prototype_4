using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterMissileStrike : MonoBehaviour
{
    public int numOfMissiles = 10;
    public GameObject missilePrefab;
    public float delay = 0.5f;

    public float startHeight = 0.0f;
    public float horizontalPositionJitter = 0.0f;

    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((timer -= Time.deltaTime) <= 0)
        {
            timer = delay;
            numOfMissiles--;
            Rigidbody2D rb = Instantiate(missilePrefab, new Vector3(transform.position.x + Random.Range(-horizontalPositionJitter, horizontalPositionJitter), transform.position.y + startHeight, transform.position.z), Quaternion.identity).GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(0, -10);
        }
        if (numOfMissiles <= 0)
        {
            Destroy(gameObject);
        }
        
        
    }
}
