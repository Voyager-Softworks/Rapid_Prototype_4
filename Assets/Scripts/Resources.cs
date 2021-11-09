using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Resources : MonoBehaviour
{
    public enum ResourceType
    {
        ORGANIC,
        POWER,
        SCRAP
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
        new PlayerResource{type = ResourceType.ORGANIC, amount = 0},
        new PlayerResource{type = ResourceType.POWER, amount = 0},
        new PlayerResource{type = ResourceType.SCRAP, amount = 0}
    };

    [Header("Prefabs")]
    [SerializeField] public GameObject m_powerPrefab;
    [SerializeField] public GameObject m_organicPrefab;
    [SerializeField] public GameObject m_scrapPrefab;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}