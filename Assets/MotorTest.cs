using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotorTest : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float hfSpeed = 0.0f;
    [Range(0.0f, 1.0f)]
    public float lfSpeed = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Gamepad.current.SetMotorSpeeds(lfSpeed, hfSpeed);
    }
}
