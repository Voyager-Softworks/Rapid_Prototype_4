using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Player resources
    [SerializeField] public Dictionary<ResourceType, int> g_playerResources = new Dictionary<ResourceType, int>() {
        { ResourceType.Organic, 0 },
        { ResourceType.Power, 0 },
        { ResourceType.Scrap, 0 }
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