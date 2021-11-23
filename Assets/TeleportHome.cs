using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportHome : MonoBehaviour
{
    private Persistent persistent;
    public float time = 1.0f;
    private float timeCopy = 0;

    private void Start() {
        timeCopy = time;
        
        if (!persistent) {
            persistent = GameObject.FindObjectOfType<Persistent>();
        }
    }
    private void Update() {
        if (Keyboard.current.rKey.isPressed) {
            time -= Time.deltaTime;
        }
        else{
            time = timeCopy;
        }

        if (time <= 0) {
            time = timeCopy;
            persistent.GetComponent<SceneController>().LoadHub();
        }
    }
}
