using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    AudioSource source;
    public List<AudioClip> clipList;
    public SFX_Effect sfx;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlaySound()
    {
        source.clip = clipList[Random.Range(0, clipList.Count)];
        source.Play();
        if (sfx != null)
        {
            sfx.Play();
        }
        
    }
}
