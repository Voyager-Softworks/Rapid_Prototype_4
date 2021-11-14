using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemyPrefabs;

    [Serializable]
    public class KillTracker{
        [SerializeField] public EnemyTest.EnemyType type;
        [SerializeField] public bool elite;
        [SerializeField] public int killsCount;
    }

    [SerializeField] public List<KillTracker> killList = new List<KillTracker>(){};

    public int total_kills = 0;
    public int total_deaths = 0;

    void Awake() {
        if (!GetComponent<DontDestroy>().CheckValidInstance()) return;
        SceneManager.sceneLoaded += SceneLoaded;

        //for each enemy type, add a killtracker to the kill list
        foreach (EnemyTest.EnemyType type in Enum.GetValues(typeof(EnemyTest.EnemyType))) {
            killList.Add(new KillTracker(){type = type, elite = false, killsCount = 0});
            killList.Add(new KillTracker(){type = type, elite = true, killsCount = 0});
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    public void AddKill(GameObject enemy, bool elite) {
        EnemyTest enemyTest = enemy.GetComponent<EnemyTest>();
        if (enemyTest == null) {
            Debug.LogError("EnemyTest component not found on " + enemy.name);
            return;
        }

        foreach (KillTracker kill in killList) {
            if (kill.type == enemy.GetComponent<EnemyTest>().m_enemyType && kill.elite == elite) {
                kill.killsCount++;
                total_kills++;
                return;
            }
        }
    }

    public int GetKills(EnemyTest.EnemyType type, bool elite) {
        foreach (KillTracker kill in killList) {
            if (kill.type == type && kill.elite == elite) {
                return kill.killsCount;
            }
        }
        return 0;
    }
}
