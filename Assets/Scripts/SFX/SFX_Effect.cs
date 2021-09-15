using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SFX_Effect : MonoBehaviour
{
    public enum SFXtype
    {
        DESTROY_OBJ,
        WAIT,
        PLAY_SOUND,
        PLAY_PARTICLE,
        STOP_PARTICLE,
        SET_SCREEN_SHAKE,
    }
    [System.Serializable]
    public class Effect
    {
        public SFXtype m_type;
        public float m_waitDuration;
        public AudioClip m_clip;
        public AudioSource m_source;
        public ParticleSystem m_particle;
        public Vector2 m_screenShakeAmplitude;
    }
    public NoiseSettings m_noiseSettingsData;
    public bool m_playOnAwake = false;
    public bool m_loop = false;
    bool m_playing = false;
    float m_delayTimer = 0.0f;

    int m_currentEffectIndex = 0;

    [HideInInspector]
    public List<Effect> m_effectsList;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (m_playOnAwake) m_playing = true;
    }

    public void Play()
    {
        m_playing = true;
    }
    public void Pause()
    {
        m_playing = false;
    }
    public void Stop()
    {
        m_playing = false;
        m_currentEffectIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_delayTimer > 0.0f)
        {
            m_delayTimer -= Time.deltaTime;
        }
        else
        {
            m_delayTimer = 0.0f;
        }
        if (m_playing && m_delayTimer == 0.0f && m_effectsList.Count > 0)
        {
            if (m_currentEffectIndex == m_effectsList.Count)
            {
                m_currentEffectIndex = 0;
                if (!m_loop)
                {
                    m_playing = false;
                    return;
                }
            }
            Effect currEffect = m_effectsList[m_currentEffectIndex];
            switch (currEffect.m_type)
            {
                case SFXtype.DESTROY_OBJ:
                    Destroy(this.gameObject);
                    break;
                case SFXtype.PLAY_PARTICLE:
                    currEffect.m_particle.Play();
                    break;
                case SFXtype.STOP_PARTICLE:
                    currEffect.m_particle.Stop();
                    break;
                case SFXtype.PLAY_SOUND:
                    currEffect.m_source.clip = currEffect.m_clip;
                    currEffect.m_source.Play();
                    break;
                case SFXtype.SET_SCREEN_SHAKE:
                    m_noiseSettingsData.PositionNoise[0].X.Amplitude = currEffect.m_screenShakeAmplitude.x;
                    m_noiseSettingsData.PositionNoise[0].Y.Amplitude = currEffect.m_screenShakeAmplitude.y;
                    break;
                case SFXtype.WAIT:
                    m_delayTimer = currEffect.m_waitDuration;
                    break;
                default:
                    break;
            }
            m_currentEffectIndex++;


        }
    }
}