using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static GameObject instance = null;

    void Awake()
    {
        CheckValidInstance();
    }

    public bool CheckValidInstance()
    {
        if (instance == null)
        {
            instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);   
        }
        else if (instance != this.gameObject)
        {
            Destroy(this.gameObject);
            return false;
        }

        return true;
    }
}
