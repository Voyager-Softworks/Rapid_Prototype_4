using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUpgrades : MonoBehaviour
{
    void Awake() {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!GetComponent<DontDestroy>().CheckValidInstance()) return;
    }
}
