using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("Interact")]
    private GameObject m_interactCanvas = null;
    private GameObject player = null;
    public float m_interactDistance = 2.0f;
    private bool m_canInteract = false;
    public bool open = false;

    // Start is called before the first frame update
    void Start()
    {
        m_interactCanvas = GetComponentInChildren<InteractCanvas>().gameObject;

        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();

        //set active
        if (m_canInteract && !open)
        {
            m_interactCanvas.SetActive(true);
        }
        else
        {
            m_interactCanvas.SetActive(false);
        }

        if (m_canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) open = true;
        }
        
        if (open)
        {
            GetComponentInChildren<Animator>().SetBool("Open", true);
            //check if the animation is done
            if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Open") && GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                //disable collider in children
                if (GetComponentInChildren<Collider2D>() && GetComponentInChildren<Collider2D>().enabled) GetComponentInChildren<Collider2D>().enabled = false;
            }
        }
    }

    private void CheckDistance()
    {
        if (m_interactCanvas && player)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < m_interactDistance)
            {
                m_canInteract = true;
            }
            else
            {
                m_canInteract = false;
            }
        }
    }
}
