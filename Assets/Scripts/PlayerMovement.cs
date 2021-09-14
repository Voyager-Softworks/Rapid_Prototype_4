using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Events;



public class PlayerMovement : MonoBehaviour
{
    FollowMouse m_mousescript;
    WalkSound m_walksoundmaker;
    SpriteRenderer m_renderer;
    Animator m_anim;
    Animator[] m_gunAnims;
    [Header("Controls")]
    public InputAction m_walkAction, m_jumpAction, m_airStrafeAction, m_dashAction;
    Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] float m_moveSpeed = 1.0f;
    [SerializeField] float m_maxGroundedVelocity = 1.0f;
    [SerializeField] float m_jumpForce = 1.0f;

    [SerializeField] float m_dashForce = 1.0f;
    float m_timesincelastmovementkey = 0.0f;
    [SerializeField] float m_dashDuration = 1.0f;
    float m_dashTimer = 0.0f;
    [SerializeField] float m_landDuration = 1.0f;
    public float m_landingTimer = 0.0f;
    public UnityEvent m_OnLand;
    [Range(0.0f, 1.0f)]
    [SerializeField] float m_maneuveringThrusterStrength = 1.0f;


    public AnimationCurve m_jumpForceCurve;

    public bool m_grounded = false;
    public Vector2 m_groundCheckOffset, m_groundCheckExtents;

    public bool m_charging = false;
    public float m_minChargeDuration = 0.0f;
    public float m_chargeTimer = 0.0f;

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
    public AudioSource m_chargeSource;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_renderer = GetComponent<SpriteRenderer>();
        m_anim = GetComponent<Animator>();
        m_walksoundmaker = GetComponentInChildren<WalkSound>();
        m_mousescript = GetComponentInChildren<FollowMouse>();
        m_gunAnims = GetComponentsInChildren<Animator>();
    }

    public void StepNoise()
    {
        m_walksoundmaker.PlaySound();
    }

    void Awake()
    {
        m_walkAction.Enable();
        m_jumpAction.Enable();
        m_airStrafeAction.Enable();
        m_dashAction.Enable();
        m_jumpAction.started += ChargeJump;
        m_jumpAction.canceled += Jump;
        m_jumpAction.performed += Jump;
        m_dashAction.performed += Dash;
        m_chargeTimer = 0;
        m_walkAction.started += (_ctx) => { if (m_timesincelastmovementkey < 0.3f) { Dash(_ctx); } m_timesincelastmovementkey = 0; };
    }


    void Dash(InputAction.CallbackContext _ctx)
    {
        if (m_dashTimer > 0.0f) return;
        rb.mass = 10000;
        rb.velocity += (m_walkAction.ReadValue<Vector2>() * m_dashForce) + (Vector2.up * 4.0f);
        m_dashTimer = m_dashDuration;

    }

    void Land()
    {

        m_anim.ResetTrigger("Jump");
        m_landingTimer = m_landDuration;
        m_noise.PositionNoise[0].X.Amplitude = m_landingNoiseMagnitude.x * (Mathf.Abs(m_oldVelocity.x) / 10.0f);
        m_noise.PositionNoise[0].Y.Amplitude = m_landingNoiseMagnitude.y * (Mathf.Abs(m_oldVelocity.y) / 10.0f);
        m_landingSource.Play();
        m_OnLand.Invoke();


    }

    void ChargeJump(InputAction.CallbackContext _ctx)
    {
        if (!m_grounded) return;
        m_anim.SetTrigger("Charge");
        m_anim.ResetTrigger("ChargeCancelled");
        m_charging = true;
        m_chargeSource.Play();

    }

    void CancelJump(InputAction.CallbackContext _ctx)
    {

        m_charging = false;
        m_chargeTimer = 0;

    }

    void Jump(InputAction.CallbackContext _ctx)
    {
        m_charging = false;
        if (m_chargeTimer >= m_minChargeDuration)
        {

            m_jumpSource.Play();
            rb.AddForce(new Vector2(0.0f, m_jumpForceCurve.Evaluate(m_chargeTimer) * m_jumpForce), ForceMode2D.Force);
            m_anim.SetTrigger("Jump");
        }
        else
        {

            m_anim.SetTrigger("ChargeCancelled");
            m_anim.ResetTrigger("Charge");
        }
        m_chargeSource.Stop();
        m_chargeTimer = 0;


    }

    // Update is called once per frame
    void Update()
    {
        bool m_groundedOld = m_grounded;
        m_timesincelastmovementkey += Time.deltaTime;
        m_grounded = Physics2D.BoxCast(transform.position + (Vector3)m_groundCheckOffset, (Vector3)m_groundCheckExtents, 0.0f, Vector2.up, 1.0f, LayerMask.GetMask("Ground"));
        m_anim.SetBool("Grounded", m_grounded);
        if (!m_groundedOld && m_grounded)
        {
            Land();
        }
        if (m_dashTimer > 0)
        {
            m_dashTimer -= Time.deltaTime;
        }
        else
        {
            m_dashTimer = 0;
            rb.mass = 20000;
        }
        if (m_landingTimer > 0) { m_landingTimer -= Time.deltaTime; }
        else if (m_grounded)
        {

            m_noise.PositionNoise[0].X.Amplitude = 0.0f;
            m_noise.PositionNoise[0].Y.Amplitude = 0.0f;
            m_landingTimer = 0.0f;
        }
        Vector2 vel = Vector2.zero;
        if (m_charging)
        {
            m_chargeTimer += Time.deltaTime;
        }
        if (m_grounded && !m_charging && m_landingTimer == 0)
        {
            vel += m_walkAction.ReadValue<Vector2>();

        }
        else if (!m_grounded)
        {
            vel += m_airStrafeAction.ReadValue<Vector2>() * m_maneuveringThrusterStrength;

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
                    anim.SetBool("Walking", false);
                    if (!anim.GetBool("Shooting") && !anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerGun_SHOOT")) anim.speed = Mathf.Clamp(rb.velocity.magnitude, 1.0f, float.PositiveInfinity);

                }
            }
            m_anim.speed = 1.0f;
        }
        else if (m_grounded)
        {
            m_anim.SetBool("Walking", true);
            foreach (var anim in m_gunAnims)
            {
                if (anim != m_anim)
                {
                    anim.SetBool("Walking", true);
                    if (!anim.GetBool("Shooting") && !anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerGun_SHOOT")) anim.speed = Mathf.Clamp(rb.velocity.magnitude, 1.0f, float.PositiveInfinity);
                }
            }
            m_anim.speed = rb.velocity.magnitude;
        }
        else
        {
            m_anim.speed = 1.0f;
            if (m_grounded)
            {
                foreach (var anim in m_gunAnims)
                {
                    if (anim != m_anim)
                    {
                        anim.SetBool("Walking", true);
                        if (!anim.GetBool("Shooting") && !anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerGun_SHOOT")) anim.speed = 1.0f;
                    }
                }
            }

        }

        rb.velocity += vel * Time.deltaTime;
        if (rb.velocity.magnitude > m_maxGroundedVelocity && m_grounded && !m_charging && vel.magnitude > 0.1f && m_dashTimer <= 0.0f)
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
