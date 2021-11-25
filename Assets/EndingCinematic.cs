using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCinematic : MonoBehaviour
{
    public Canvas UIcanvas;

    public void EnableUI()
    {
        UIcanvas.enabled = true;
        Cursor.visible = true;
    }
    public void DisableUI()
    {
        UIcanvas.enabled = false;
        Cursor.visible = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
