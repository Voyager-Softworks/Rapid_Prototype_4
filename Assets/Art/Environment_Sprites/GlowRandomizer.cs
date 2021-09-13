using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowRandomizer : MonoBehaviour
{
    private float r = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        r = Random.Range(0.0f, 100.0f);
       
        GetComponent<Renderer>().material = new Material(GetComponent<Renderer>().material); 
        GetComponent<Renderer>().material.SetFloat("_RandomNum", r);
    }

}
