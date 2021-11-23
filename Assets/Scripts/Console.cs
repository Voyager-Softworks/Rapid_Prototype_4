using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//if in editor mode
#if UNITY_EDITOR
using UnityEditor;
#endif

//class used to manage an interactable UI for shops and upgrades
public class Console : MonoBehaviour
{
    public string m_DisplayName = "Console";
    private GameObject m_interactCanvas = null;
    private GameObject player = null;
    public float m_interactDistance = 2.0f;
    public bool m_canInteract = false;
    public GameObject m_elementToOpen = null;
    public AudioClip m_openSound = null;
    public AudioClip m_closeSound = null;
    public bool m_isOpen = false;

    private void Start() {
        if (m_interactCanvas == null) {
            m_interactCanvas = GetComponentInChildren<InteractCanvas>().gameObject;
        }

        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void Update()
    {
        CheckDistance();

        //set active
        if (m_canInteract && !m_isOpen)
        {
            m_interactCanvas.SetActive(true);
        }
        else
        {
            m_interactCanvas.SetActive(false);
        }



        //if press E, open element
        if (m_canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) ToggleElement();
        }
        else
        {
            CloseElement();
        }
    }

    private void CheckDistance()
    {
        if (m_interactCanvas && player)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < m_interactDistance)
            {
                m_canInteract = true;
            }
            else
            {
                m_canInteract = false;
            }
        }
    }

    public void OpenElement() {
        if (m_elementToOpen && !m_isOpen) {
            GetComponent<AudioSource>().PlayOneShot(m_openSound);
            m_elementToOpen.SetActive(true);
            m_isOpen = true;
        }
    }

    public void CloseElement() {
        if (m_elementToOpen && m_isOpen) {
            GetComponent<AudioSource>().PlayOneShot(m_closeSound);
            m_elementToOpen.SetActive(false);
            m_isOpen = false;
        }
    }

    public void ToggleElement() {
        if (m_elementToOpen) {
            if (m_isOpen) {
                CloseElement();
            }
            else {
                OpenElement();
            }
        }
    }


    #region Editor Stuff
    public GameObject InteractCanvasPrefab;

    //if in editor mode, check if there is an InteractCanvas on this object
    #if UNITY_EDITOR
    [CustomEditor(typeof(Console))]
    public class ConsoleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Console console = (Console)target;
            if (GUILayout.Button("Add InteractCanvas"))
            {
                console.AddInteractCanvas();
            }

            //add audiosource button
            if (GUILayout.Button("Add AudioSource"))
            {
                console.AddAudioSource();
            }
        }
    }

    public void AddInteractCanvas()
    {
        //check if there is an InteractCanvas on this object
        if (GetComponentInChildren<InteractCanvas>() == null)
        {
            //if not, add one
            m_interactCanvas = Instantiate(InteractCanvasPrefab, transform);
            m_interactCanvas.name = "InteractCanvas";

            //set the text of the canvas to the display name
            m_interactCanvas.GetComponentInChildren<TextMeshProUGUI>().text = m_DisplayName + "\n[E]";

            //save the object
            EditorUtility.SetDirty(this);
        }
    }

    public void AddAudioSource()
    {
        //check if there is an AudioSource on this object
        if (GetComponent<AudioSource>() == null)
        {
            //if not, add one
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f;
            audioSource.minDistance = 1.0f;
            audioSource.maxDistance = 10.0f;

            //save the object
            EditorUtility.SetDirty(this);
        }
    }
    #endif
    #endregion
}