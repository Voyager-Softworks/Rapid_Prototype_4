using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscapeMenu : MonoBehaviour
{
    public GameObject escapeMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            escapeMenu.SetActive(!escapeMenu.activeSelf);
            //Time.timeScale = (escapeMenu.activeSelf) ? 0 : 1;
        }
    }
}
