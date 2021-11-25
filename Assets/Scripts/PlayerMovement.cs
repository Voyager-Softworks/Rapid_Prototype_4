using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Events;



public class PlayerMovement : MonoBehaviour
{
    public bool m_dashEnabled = true, m_doubleJumpEnabled = true;

    FollowMouse m_mousescript;
    WalkSound m_walksoundmaker;
    SpriteRenderer m_renderer;
    Animator m_anim;

    public Animator m_dashAnim;
    Animator[] m_gunAnims;
    [Header("Controls")]
    public InputAction m_walkAction, m_jumpAction, m_airStrafeAction, m_dashAction, m_groundPoundAction;
    Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] float m_moveSpeed = 1.0f;
    [Header("Ground Speed")]
    [SerializeField] float m_maxGroundedVelocity = 1.0f;
    [Header("Jumping")]
    [SerializeField] float m_jumpForce = 1.0f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float m_airStrafeForce = 1.0f;
    [Header("Maneuvering Thrusters")]
    [SerializeField] float m_thrusterForce = 1.0f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float m_maneuveringThrusterStrength = 1.0f;
    
    public bool m_thrustersEngaged = false;
    public float m_thrusterDelay = 0.0f;
    float m_thrusterTimer;
    public float m_maxThrusterDuration = 0.0f;
    float m_thrusterDuration;

    [Header("Double Jump")]
    public int m_maxAirJumpCount = 0;
    int m_jumpcount = 0;
    
    [Header("Ground Pound")]
    public bool m_groundPoundEnabled = true;
    public float m_groundPoundForce = 1.0f;
    public float m_shockwaveDamage = 1.0f;
    public float m_shockwaveKnockback = 1.0f;
    public GameObject m_shockwavePrefab;

    [Header("Dash")]
    [SerializeField] float m_dashForce = 1.0f;
    Vector2 m_dashDirection;
    
    float m_timesincelastmovementkey = 0.0f;
    KeyCode m_lastmovementkey = KeyCode.None;

    Vector2 m_lastMovementDirection;
    [SerializeField] float m_dashDuration = 1.0f;
    float m_dashTimer = 0.0f;

    [SerializeField] float m_dashCooldown = 1.0f;
    float m_dashCooldownTimer;

    [Header("Landing")]
    [SerializeField] float m_landDuration = 1.0f;
    public float m_landingTimer = 0.0f;
    public UnityEvent m_OnLand, m_onDash;
   



    public bool m_grounded = false;
    public bool m_groundPounding = false;
    public Vector2 m_groundCheckOffset, m_groundCheckExtents;

    

   
   

    Vector2 m_oldVelocity;


    bool m_facingLeft = false;
    bool m_reversing = false;

    [Header("Special Effects")]
    public CinemachineVirtualCamera m_cam;
    public NoiseSettings m_noise;
    public Vector2 m_landingNoiseMagnitude;


    [Header("Audio")]
    public AudioSource m_landingSource;
    public AudioSource m_jumpSource;
    public AudioSource m_doubleJumpSource;
    public AudioSource m_thrusterSource;

    public AudioSource m_groundPoundSource;


    [Header("Particles")]
    public List<ParticleSystem> m_particles;

    private UpgradeManager upgradeManager = null;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_renderer = GetComponent<SpriteRenderer>();
        m_anim = GetComponent<Animator>();
        m_walksoundmaker = GetComponentInChildren<WalkSound>();
        m_mousescript = GetComponentInChildren<FollowMouse>();
        m_gunAnims = GetComponentsInChildren<Animator>();

        if (DontDestroy.instance) upgradeManager = DontDestroy.instance.GetComponent<UpgradeManager>();

        if (upgradeManager.ModuleUnlocked(UpgradeManager.ModuleType.DASH))
        {
            m_dashEnabled = true;
        }
        else
        {
            m_dashEnabled = false;
        }

        if (upgradeManager.ModuleUnlocked(UpgradeManager.ModuleType.SLAM))
        {
            m_groundPoundEnabled = true;
        }
        else
        {
            m_groundPoundEnabled = false;
        }
        m_shockwaveDamage = (upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.SLAM)+1) * 2.0f;
        m_shockwaveKnockback = (upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.SLAM)*2.5f) + 10.0f;
        m_dashDuration = 0.3f + (upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.DASH) * 0.1f);
        if (upgradeManager.ModuleUnlocked(UpgradeManager.ModuleType.JUMP))
        {
            m_doubleJumpEnabled = true;
        }
        else
        {
            m_doubleJumpEnabled = false;
        }

        
        //if (upgradeManager) upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.DASH);
    }

    public void StepNoise()
    {
        m_walksoundmaker.PlaySound();
    }
    public void StepParticle(int _index)
    {
        m_particles[_index].Play();
    }

    void Awake()
    {
        m_walkAction.Enable();
        m_jumpAction.Enable();
        m_airStrafeAction.Enable();
        m_dashAction.Enable();
        m_groundPoundAction.Enable();
        m_jumpAction.started += Jump;
        m_jumpAction.canceled += DisengageThrusters;
        m_jumpAction.performed += DisengageThrusters;
        m_dashAction.performed += Dash;
        
        m_walkAction.started += (_ctx) => { if (m_timesincelastmovementkey < 0.3f && m_lastMovementDirection == _ctx.ReadValue<Vector2>().normalized) { Dash(_ctx); } m_timesincelastmovementkey = 0; m_lastMovementDirection = _ctx.ReadValue<Vector2>().normalized; };
        m_groundPoundAction.started += (_ctx) => { if (m_timesincelastmovementkey < 0.3f && m_lastMovementDirection == _ctx.ReadValue<Vector2>().normalized) { GroundPound(_ctx); } m_timesincelastmovementkey = 0; m_lastMovementDirection = _ctx.ReadValue<Vector2>().normalized;};
    }


    void Dash(InputAction.CallbackContext _ctx)
    {
        if (m_dashTimer > 0.0f || m_dashCooldownTimer > 0.0f || !upgradeManager.ModuleUnlocked(UpgradeManager.ModuleType.DASH)) return;
        m_onDash.Invoke();
        
        
        m_dashDirection = m_walkAction.ReadValue<Vector2>();
        if ((m_dashDirection.x < 0.0f && m_facingLeft) || (m_dashDirection.x > 0.0f && !m_facingLeft))
        {
            m_dashAnim.SetTrigger("DashForward");
        }
        else
        {
            m_dashAnim.SetTrigger("DashBackward");
        }
        m_dashDuration = 0.3f + (upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.DASH) * 0.1f);
        m_dashTimer = m_dashDuration;

    }

    void GroundPound(InputAction.CallbackContext _ctx)
    {
        if(!m_grounded && !m_thrustersEngaged && upgradeManager.ModuleUnlocked(UpgradeManager.ModuleType.SLAM))
        {
            m_shockwaveDamage = (upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.SLAM)+1) * 2.0f;
            m_shockwaveKnockback = (upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.SLAM)*2.5f) + 10.0f;
            rb.velocity = new Vector2(0.0f, -m_groundPoundForce);
            m_groundPoundSource.Play();
            m_groundPounding = true;
        }
    }

    void Land()
    {

        m_anim.ResetTrigger("Jump");
        m_landingTimer = m_landDuration;
        m_noise.PositionNoise[0].X.Amplitude = m_landingNoiseMagnitude.x * (Mathf.Abs(m_oldVelocity.x) / 10.0f);
        m_noise.PositionNoise[0].Y.Amplitude = m_landingNoiseMagnitude.y * (Mathf.Abs(m_oldVelocity.y) / 10.0f);
        if(Gamepad.current != null) Gamepad.current.SetMotorSpeeds(Mathf.Clamp(m_oldVelocity.magnitude/20.0f, 0.0f, 1.0f), 0.5f);
        
        if (m_groundPounding)
        {
            Shockwave s = Instantiate(m_shockwavePrefab, transform.position, Quaternion.identity).GetComponent<Shockwave>();
            s.damage = m_shockwaveDamage;
            s.knockback = m_shockwaveKnockback;

            m_groundPounding = false;
        }
        else
        {
            m_landingSource.Play();
        }
        
        m_landingSource.Play();
        m_OnLand.Invoke();
        m_jumpcount = 0;
    }



    void Jump(InputAction.CallbackContext _ctx)
    {
        m_maxThrusterDuration = 0.3f + (upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.JETPACK) * 0.1f);
        m_maneuveringThrusterStrength = 0.2f + (upgradeManager.GetModuleLevel(UpgradeManager.ModuleType.AIRTRHUST) * 0.1f);
        if (m_grounded)
        {

            m_jumpSource.Play();
            rb.AddForce(new Vector2(0.0f, m_jumpForce), ForceMode2D.Force);
            m_anim.SetTrigger("Jump");
            m_thrustersEngaged = true;
            m_thrusterTimer = m_thrusterDelay;
            m_thrusterDuration = m_maxThrusterDuration;
            m_jumpcount++;
        }
        else if(m_jumpcount < m_maxAirJumpCount && upgradeManager.ModuleUnlocked(UpgradeManager.ModuleType.JUMP))
        {
            m_doubleJumpSource.Play();
            rb.velocity *= new Vector2(1.0f, 0.0f);
            rb.AddForce(new Vector2(0.0f, m_jumpForce ), ForceMode2D.Force);
            m_anim.SetTrigger("Jump");
            m_jumpcount++;
        }
        
    }

    void DisengageThrusters(InputAction.CallbackContext _ctx)
    {
        m_thrustersEngaged = false;
        m_thrusterSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        bool m_groundedOld = m_grounded;
        m_timesincelastmovementkey += Time.deltaTime;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            (Vector2)transform.position + m_groundCheckOffset, 
            m_groundCheckExtents, 
            0.0f, 
            Vector2.up, 
            1.0f, 
            LayerMask.GetMask("Ground")
            );
        m_grounded = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && Vector2.Dot(hit.normal, Vector2.up) >= 0.0f && hit.collider.gameObject.tag == "Ground")
            {
                m_grounded = true;
                break;
            }
        }
        m_anim.SetBool("Grounded", m_grounded);
        if (!m_groundedOld && m_grounded)
        {
            Land();
        }
        if (m_dashTimer > 0)
        {
            m_dashTimer -= Time.deltaTime;
            m_dashCooldownTimer = m_dashCooldown;
        }
        else
        {
            m_dashTimer = 0;
            rb.mass = 20000;
            if ((m_dashCooldownTimer -= Time.deltaTime) < 0.0f)
            {
                m_dashCooldownTimer = 0.0f;
            }

        }
        if (m_landingTimer > 0) { m_landingTimer -= Time.deltaTime; }
        else if (m_grounded)
        {

            
            m_landingTimer = 0.0f;
        }
        Vector2 vel = Vector2.zero;
        if (m_thrustersEngaged)
        {
            if (m_thrusterTimer <= 0.0f && !m_grounded)
            {
                if (m_thrusterDuration >= 0.0f)
                {
                    if (!m_thrusterSource.isPlaying) m_thrusterSource.Play();
                    vel += Vector2.up * m_thrusterForce;
                    m_thrusterDuration -= Time.deltaTime;
                }
                else
                {
                    m_thrusterSource.Stop();
                }
            }
            else
            {
                m_thrusterTimer -= Time.deltaTime;
            }
        }
        else
        {
            m_thrusterSource.Stop();
        }
       
        if (m_grounded && m_landingTimer == 0 && m_dashTimer <= 0.0f)
        {
            vel.x = m_walkAction.ReadValue<Vector2>().x;

        }
        else if (!m_grounded && m_thrustersEngaged && m_dashTimer <= 0.0f)
        {
            vel.x = m_airStrafeAction.ReadValue<Vector2>().x * (m_maneuveringThrusterStrength + m_airStrafeForce);

        }
        else if (!m_grounded && m_dashTimer <= 0.0f && Mathf.Abs(rb.velocity.y) > 0.5f)
        {
            vel.x = m_airStrafeAction.ReadValue<Vector2>().x * (m_airStrafeForce);
        }
        else if (m_dashTimer > 0.0f)
        {
            vel.x = m_dashDirection.x * m_dashForce;
        }

        vel = vel * m_moveSpeed;

        if (m_mousescript.mouse_pos.x < -50.0f)
        {
            m_facingLeft = true;
        }
        else if (m_mousescript.mouse_pos.x > 50.0f)
        {
            m_facingLeft = false;
        }
        //m_facingLeft = (m_mousescript.mouse_pos.x < 0.0f);
        m_reversing = ((vel.x < -0.01f && !m_facingLeft) || (vel.x > 0.01f && m_facingLeft));

        if (m_facingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

        m_anim.SetBool("Reversing", m_reversing);

        if (vel.magnitude < 0.1f && m_grounded && m_dashTimer <= 0.0f)
        {
            rb.velocity *= 0.9f;
            m_anim.SetBool("Walking", false);
            foreach (var anim in m_gunAnims)
            {
                if (anim != m_anim)
                {
                    //anim.SetBool("Walking", false);
                    

                }
            }
            
        }
        else if (m_grounded)
        {
            m_anim.SetBool("Walking", true);
            foreach (var anim in m_gunAnims)
            {
                if (anim != m_anim)
                {
                    //anim.SetBool("Walking", true);
                    
                }
            }
            
        }
        else
        {
            
            if (m_grounded)
            {
                foreach (var anim in m_gunAnims)
                {
                    if (anim != m_anim)
                    {
                        //anim.SetBool("Walking", true);
                        
                    }
                }
            }

        }

        rb.velocity += new Vector2(0.0f, vel.y * Time.deltaTime);
        rb.velocity = new Vector2(vel.x, rb.velocity.y);
        if (rb.velocity.magnitude > m_maxGroundedVelocity && m_grounded && vel.magnitude > 0.1f && m_dashTimer <= 0.0f)
        {
            rb.velocity = rb.velocity.normalized * m_maxGroundedVelocity;
        }
        m_oldVelocity = rb.velocity;
        m_anim.SetBool("Falling", rb.velocity.y < 0.0f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + (Vector3)m_groundCheckOffset, (Vector3)m_groundCheckExtents);
        if (rb) Gizmos.DrawRay(transform.position, rb.velocity);
    }
}
