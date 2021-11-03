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

    //Player resources
    public Dictionary<ResourceType, int> g_playerResources = new Dictionary<ResourceType, int>() {
        { ResourceType.Organic, 0 },
        { ResourceType.Power, 0 },
        { ResourceType.Scrap, 0 }
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
