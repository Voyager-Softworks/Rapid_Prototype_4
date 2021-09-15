using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.CompareTag("Player"))
        {
            other.collider.gameObject.GetComponent<PlayerHealth>().Damage(1.0f);
        }
        if (GetComponent<SFX_Effect>())
        {
            GetComponent<SFX_Effect>().Play();
        }
    }
}
