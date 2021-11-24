using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static GameObject instance = null;

    public bool taggedForDelete = false;

    void Awake()
    {
        CheckValidInstance();
    }

    public bool CheckValidInstance()
    {
        if (instance == null && !taggedForDelete)
        {
            instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);   
            GetComponent<SaveSerialization>().LoadData();
        }
        else if (instance != this.gameObject)
        {
            Destroy(this.gameObject);
            return false;
        }

        return true;
    }
}
