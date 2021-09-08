using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;



public class PlayerMovement : MonoBehaviour
{
    [Header("Controls")]
    public InputAction m_walkAction, m_jumpAction, m_airStrafeAction;
    Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] float m_moveSpeed = 1.0f;
    [SerializeField] float m_jumpForce = 1.0f;
    [SerializeField] float m_landDuration = 1.0f;
    public float m_landingTimer = 0.0f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float m_maneuveringThrusterStrength = 1.0f;


    public AnimationCurve m_jumpForceCurve;

    public bool m_grounded = false;
    public bool m_charging = false;
    public float m_minChargeDuration = 0.0f;
    public float m_chargeTimer = 0.0f;

    
    bool m_facingLeft = false;
    
    [Header("Special Effects")]
    public CinemachineVirtualCamera m_cam;
    public NoiseSettings m_noise;
    public Vector2 m_landingNoiseMagnitude;
    

    [Header("Audio")]
    public AudioSource m_landingSource;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Awake()
    {
        m_walkAction.Enable();
        m_jumpAction.Enable();
        m_jumpAction.started += ChargeJump;
        m_jumpAction.canceled += Jump;
        m_chargeTimer = 0;
    }

    void Land()
    {
        m_airStrafeAction.Disable();
        
        m_landingTimer = m_landDuration;
        m_noise.PositionNoise[0].X.Amplitude = m_landingNoiseMagnitude.x;
        m_noise.PositionNoise[0].Y.Amplitude = m_landingNoiseMagnitude.y;
        
        
    }

    void ChargeJump(InputAction.CallbackContext _ctx)
    {
        if(!m_grounded) return;
        m_charging = true;
        m_walkAction.Disable();
    }

    void CancelJump(InputAction.CallbackContext _ctx)
    {
        m_walkAction.Enable();
        m_charging = false;
        m_chargeTimer = 0;
        
    }

    void Jump(InputAction.CallbackContext _ctx) 
    {
        m_charging = false;
        if (m_chargeTimer >= m_minChargeDuration)
        {
            m_airStrafeAction.Enable();
            m_walkAction.Disable();
            rb.AddForce(new Vector2(0.0f,m_jumpForceCurve.Evaluate(m_chargeTimer) * m_jumpForce), ForceMode2D.Impulse);
        }
        else
        {
            m_walkAction.Enable();
            m_airStrafeAction.Disable();
        }
        
        m_chargeTimer = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        bool m_groundedOld = m_grounded;
        m_grounded = Physics2D.BoxCast(transform.position + Vector3.down, new Vector3(1, 0.5f, 0), 0.0f, Vector2.up, 1.0f, LayerMask.GetMask("Ground"));
        if (!m_groundedOld && m_grounded)
        {
            Land();
        }
        if(m_landingTimer > 0){m_landingTimer -= Time.deltaTime;}
        else if (!m_walkAction.enabled && m_grounded)
        {
            m_walkAction.Enable();
            m_noise.PositionNoise[0].X.Amplitude = 0.0f;
            m_noise.PositionNoise[0].Y.Amplitude = 0.0f;
            m_landingTimer = 0.0f;
        }
        Vector2 vel = Vector2.zero;
        if (m_charging)
        {
            m_chargeTimer += Time.deltaTime;
        }
        if(m_walkAction.enabled)
        {
            vel += m_walkAction.ReadValue<Vector2>();
            if(!m_grounded) { m_walkAction.Disable(); m_airStrafeAction.Enable(); }
        }
        else if (m_airStrafeAction.enabled)
        {
            vel += m_airStrafeAction.ReadValue<Vector2>() * m_maneuveringThrusterStrength;
            if(m_grounded) Land();
        }

        vel = vel * m_moveSpeed;
        if(vel.x < -0.01f) m_facingLeft = true;
        if(vel.x > 0.01f) m_facingLeft = false;
        transform.localScale = new Vector3(m_facingLeft ? -1.0f : 1.0f, 1.0f, 1.0f);
        if (vel.magnitude < 0.1f && m_grounded)
        {
            rb.velocity *= 0.9f;
        }
        rb.velocity += vel * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + Vector3.down, new Vector3(1, 0.5f, 0));
    }
}
