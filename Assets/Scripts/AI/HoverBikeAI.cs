using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverBikeAI : MonoBehaviour
{
    public Navmesh2D navmesh;
    Animator anim;

    //Movement for hoverbike drive by shooting
    public float speed = 10f;
    public float turnSpeed = 5f;
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    public float hoverDistance = 10f;       
    private float currentSpeed;
    private float currentTurnSpeed;
    private float currentHoverForce;
    private float currentHoverHeight;
    private float currentHoverDistance;
    private Rigidbody2D rb;
    private Vector3 target;
    private Vector3 targetDir;
    
    public bool playerDetected = false;

    public float detectionRange = 10f;
    public float attackRange = 5f;

    public bool facingLeft = false;

    public Transform barrelPosition;
    public GameObject bullet;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        navmesh = FindObjectOfType<Navmesh2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get the target position by finding the player position
        target = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (Vector3.Distance(transform.position, target) < detectionRange)
        {
            playerDetected = true;
        }
        targetDir = (target - barrelPosition.position).normalized;
        if (playerDetected)
        {
            if (((Vector3.Distance(transform.position, target) < attackRange) && Vector3.Dot(barrelPosition.right, targetDir) > 0.0f))
            {
                //Shoot at the player
                anim.SetBool("isAttacking", true);
            }
            else
            {
                anim.SetBool("isAttacking", false);
            }
            rb.AddForce(transform.right * 5.0f, ForceMode2D.Force);
            //clamp velocity
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed);
            if (!navmesh.IsWalkable(transform.position, transform.right *2.0f) || 
            ((Vector3.Distance(transform.position, target) > attackRange) && Vector3.Dot(transform.right, targetDir) < 0.0f))
            
            {
                if (facingLeft)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    facingLeft = false;
                    anim.SetTrigger("Flip");

                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    facingLeft = true;
                    anim.SetTrigger("Flip");
                }
                
            }
        }
        
    }

    public void Fire()
    {
        Rigidbody2D b = Instantiate(bullet, barrelPosition.position, barrelPosition.rotation).GetComponent<Rigidbody2D>();
        b.velocity = (Vector3)rb.velocity + (transform.right * 20.0f);
    }


    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
            
        Gizmos.DrawLine(barrelPosition.position, barrelPosition.position + barrelPosition.right * 0.2f);
        Gizmos.DrawSphere(barrelPosition.position, 0.05f);
    }
}
