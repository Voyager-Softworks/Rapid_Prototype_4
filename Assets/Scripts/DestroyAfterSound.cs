using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSound : MonoBehaviour
{
    AudioSource m_as;

    private void Start()
    {
        m_as = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_as.isPlaying) Destroy(gameObject);
    }
}
