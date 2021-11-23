using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Resources : MonoBehaviour
{
    public enum ResourceType
    {
        ICHOR,
        CRYSTAL,
        SCRAP,
        EXOTIC
    }

    [Header("Scene Instances")]
    //player GameObject
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject resourceText;

    [Serializable]
    public class PlayerResource {
        [SerializeField] public ResourceType type;
        [SerializeField] public int amount;
    }

    [Header("Player Resources")]
    //player resources
    [SerializeField] public PlayerResource[] playerResources = new PlayerResource[4]{
        new PlayerResource{type = ResourceType.ICHOR, amount = 0},
        new PlayerResource{type = ResourceType.CRYSTAL, amount = 0},
        new PlayerResource{type = ResourceType.SCRAP, amount = 0},
        new PlayerResource{type = ResourceType.EXOTIC, amount = 0}
    };

    [Header("Prefabs")]
    [SerializeField] public GameObject m_ICHORPrefab;
    [SerializeField] public GameObject m_CRYSTALPrefab;
    [SerializeField] public GameObject m_SCRAPPrefab;
    [SerializeField] public GameObject m_EXOTICPrefab;

    // Start is called before the first frame update
    void Awake() {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!GetComponent<DontDestroy>().CheckValidInstance()) return;
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
        if (!resourceText) resourceText = GameObject.FindObjectOfType<ResourceText>(true).gameObject;

        UpdateVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisuals();
    }

    public void AddResource(ResourceType type, int amount)
    {
        playerResources[(int)type].amount += amount;

        UpdateVisuals();
    }

    public void AddResources(List<PlayerResource> resources){
        foreach(PlayerResource resource in resources){
            AddResource(resource.type, resource.amount);
        }
    }

    public int GetResourceCount(ResourceType type)
    {
        return playerResources[(int)type].amount;
    }

    public GameObject GetResourcePrefab(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.ICHOR:
                return m_ICHORPrefab;
            case ResourceType.CRYSTAL:
                return m_CRYSTALPrefab;
            case ResourceType.SCRAP:
                return m_SCRAPPrefab;
            case ResourceType.EXOTIC:
                return m_EXOTICPrefab;
            default:
                return null;
        }
    }

    public bool TryConsumeResources(List<PlayerResource> resources)
    {
        //save the amount of resources we have before we try to consume them
        int[] before = new int[playerResources.Length];
        for (int i = 0; i < playerResources.Length; i++)
        {
            before[i] = playerResources[i].amount;
        }

        //try to consume the resources
        for (int i = 0; i < resources.Count; i++)
        {
            playerResources[(int)resources[i].type].amount -= resources[i].amount;

            if (playerResources[(int)resources[i].type].amount < 0)
            {
                //if we don't have enough resources, return false
                for (int j = 0; j < playerResources.Length; j++)
                {
                    playerResources[j].amount = before[j];
                }
                return false;
            }
        }

        UpdateVisuals();
        return true;
    }

    public void UpdateVisuals()
    {
        if (resourceText == null) return;

        TextMeshProUGUI[] texts = resourceText.GetComponentsInChildren<TextMeshProUGUI>();

        //update the texts to show the current resources
        for (int i = 0; i < playerResources.Length; i++)
        {
            if (texts.Length <= i) break;
            texts[i].text = playerResources[i].amount.ToString();
        }
    }
}