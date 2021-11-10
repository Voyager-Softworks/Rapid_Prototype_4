using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSalvo : MonoBehaviour
{
    public float salvoSpeed = 10f;
    public float verticalDistance = 10f;

    public GameObject clusterMissilePrefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * salvoSpeed * Time.deltaTime;
        if (transform.position.y > verticalDistance)
        {
            //Get the location of the player then spawn a cluster missile prefab at their position
            Vector3 playerLocation = GameObject.FindGameObjectWithTag("Player").transform.position;
            Instantiate(clusterMissilePrefab, playerLocation, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
