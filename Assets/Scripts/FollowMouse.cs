using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    public InputAction m_mousePos;
    public Vector3 mouse_pos;

    [SerializeField] float m_rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }
 
    void Update()
    {
        

        mouse_pos = Mouse.current.position.ReadValue();
        Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        float angle = Mathf.Atan2(mouse_pos.y, transform.parent.rotation.eulerAngles.y == 0 ? mouse_pos.x : -mouse_pos.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(0, 0, Mathf.Clamp(angle, -25.0f, 25.0f))), m_rotateSpeed * Time.deltaTime);
    }
}
