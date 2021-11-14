using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public Vector2 boundingBox;


    // Start is called before the first frame update
    void Start()
    {
        //Get all enemies in the area
        Collider2D[] enemies = Physics2D.OverlapBoxAll(transform.position, boundingBox, 0);
        //Check for Enemy Test component
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.GetComponent<EnemyTest>())
            {
                //Damage the enemy
                enemy.GetComponent<EnemyTest>().TakeDamage(10, EnemyTest.DamageType.Universal);
                //If the enemy has a rigidbody, knock it back away from the shockwave proportional to the distance from the shockwave
                if (enemy.GetComponent<Rigidbody2D>())
                {
                    //Get the direction away from the shockwave
                    Vector2 direction = enemy.transform.position - transform.position;
                    direction += Vector2.up;
                    //Add force to the enemy
                    enemy.GetComponent<Rigidbody2D>().AddForce(direction.normalized * (10.0f / direction.magnitude), ForceMode2D.Impulse);
                }
            }
        }
    }


    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, boundingBox);
    }
}
