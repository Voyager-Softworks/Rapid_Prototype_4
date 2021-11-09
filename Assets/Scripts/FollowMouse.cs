using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    public static float AimConeAngle = 90.0f;
    public InputAction m_mousePos, m_aimStick;
    public Vector3 mouse_pos;

    public bool m_gamepadMode = false;
    [SerializeField] float m_rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        m_aimStick.Enable();
    }
 
    void Update()
    {
        
        if(Gamepad.current != null && m_gamepadMode)
        {
            mouse_pos = m_aimStick.ReadValue<Vector2>();
        }
        else
        {
            mouse_pos = Mouse.current.position.ReadValue();
            Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
        }
        
        
        float angle = Mathf.Atan2(mouse_pos.y, transform.parent.rotation.eulerAngles.y == 0 ? mouse_pos.x : -mouse_pos.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(0, 0, Mathf.Clamp(angle, 0.0f - (AimConeAngle/2.0f), 0.0f + (AimConeAngle/2.0f)))), m_rotateSpeed * Time.deltaTime);
    }
}
