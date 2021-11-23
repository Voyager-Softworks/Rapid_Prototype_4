using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerHazard : MonoBehaviour
{
    Animator m_anim;
    public SpriteRenderer m_beamSprite;
    public ParticleSystem m_beamParticles;
    public AnimationCurve m_beamCurve;

    bool m_lazerOn = false;

    public float m_lazerDuration = 1.0f;
    public float m_lazerCooldown = 1.0f;
    public float m_lazerDPS = 1.0f;
    float m_lazerTimer = 0.0f;
    AudioSource m_lazerSource;


    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_beamSprite.enabled = false;
        m_lazerSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        m_lazerTimer += Time.deltaTime;
        if (m_lazerTimer >= m_lazerDuration && m_lazerOn)
        {
            m_lazerOn = false;
            m_anim.SetBool("LazerOn", false);
            m_lazerTimer = 0.0f;
            m_beamSprite.enabled = false;
            m_beamParticles.Stop();
            m_lazerSource.Stop();
        }
        else if (m_lazerTimer >= m_lazerCooldown && !m_lazerOn)
        {
            m_lazerOn = true;
            m_anim.SetBool("LazerOn", true);
            m_lazerTimer = 0.0f;
            m_beamSprite.enabled = true;
            m_beamParticles.Play();
            m_lazerSource.Play();
        }
        else if (m_lazerOn)
        {
            Vector3 cache = m_beamSprite.transform.localScale;
            m_beamSprite.gameObject.transform.localScale = new Vector3(m_beamCurve.Evaluate(m_lazerTimer / m_lazerDuration), cache.y, cache.z);
            
            RaycastHit2D r = Physics2D.BoxCast(m_beamSprite.transform.position, m_beamSprite.transform.localScale, 0.0f, Vector2.zero, 0.0f, LayerMask.GetMask("Player"));
            if(r && r.collider.gameObject.GetComponent<PlayerHealth>() != null)
            {
                r.collider.gameObject.GetComponent<PlayerHealth>().Damage(m_lazerDPS * Time.deltaTime * m_beamCurve.Evaluate(m_lazerTimer / m_lazerDuration));
            }
            

        }
        
    }
}
