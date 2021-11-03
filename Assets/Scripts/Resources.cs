using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Resources : MonoBehaviour
{
    public enum ResourceType
    {
        Organic,
        Power,
        Scrap
    }

    [Header("Scene Instances")]
    //player GameObject
    [SerializeField] public GameObject player;

    [Serializable]
    public struct PlayerResource {
        [SerializeField] public ResourceType type;
        [SerializeField] public int amount;
    }

    [Header("Player Resources")]
    //player resources
    [SerializeField] public PlayerResource[] playerResources = new PlayerResource[3]{
        new PlayerResource{type = ResourceType.Organic, amount = 0},
        new PlayerResource{type = ResourceType.Power, amount = 0},
        new PlayerResource{type = ResourceType.Scrap, amount = 0}
    };

    [Header("Prefabs")]
    [SerializeField] public GameObject m_powerPrefab;
    [SerializeField] public GameObject m_organicPrefab;
    [SerializeField] public GameObject m_scrapPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}