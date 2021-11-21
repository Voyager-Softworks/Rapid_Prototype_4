using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Harvest : MonoBehaviour
{
    [Header("Interact")]
    private GameObject m_interactCanvas = null;
    private GameObject player = null;
    public float m_interactDistance = 2.0f;
    private bool m_canInteract = false;

    [Header("Harvest")]
    private Resources resourceManager = null;
    public List<Resources.PlayerResource> drops = new List<Resources.PlayerResource>();
    public bool m_isHarvested = false;

    [Header("Stages")]
    public GameObject fullStage = null;
    public GameObject emptyStage = null;

    // Start is called before the first frame update
    void Start()
    {
        m_interactCanvas = GetComponentInChildren<InteractCanvas>().gameObject;

        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (resourceManager == null) {
            resourceManager = GameObject.FindObjectOfType<Resources>();
        }

        UpdateVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();

        //set active
        if (m_canInteract && !m_isHarvested)
        {
            m_interactCanvas.SetActive(true);
        }
        else
        {
            m_interactCanvas.SetActive(false);
        }

        if (m_canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) DoHarvest();
        }
    }

    private void DoHarvest()
    {
        if (m_isHarvested) return;

        m_isHarvested = true;

        //spawn drops
        foreach (Resources.PlayerResource drop in drops)
        {
            GameObject dropObj = resourceManager.GetResourcePrefab(drop.type);
            for (int i = 0; i < drop.amount; i++)
            {
                GameObject dropInstance = Instantiate(dropObj, transform.position, Quaternion.identity, null);
                //add random force
                dropInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)) * 10.0f, ForceMode2D.Impulse);
            }
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (!m_isHarvested)
        {
            fullStage.SetActive(true);
            emptyStage.SetActive(false);
        }
        else
        {
            fullStage.SetActive(false);
            emptyStage.SetActive(true);
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
