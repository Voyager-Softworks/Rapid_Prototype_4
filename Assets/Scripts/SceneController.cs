using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneController : MonoBehaviour
{
    private GameObject persistent;

    void Awake() {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        persistent = DontDestroy.instance;

        if (GetComponent<DontDestroy>() && !GetComponent<DontDestroy>().CheckValidInstance()) return;
    }

    public void LoadPrototype()
    {
        SceneManager.LoadScene("PROTOTYPE");
    }
    public void LoadMenu()
    {
        if (persistent != null) persistent.GetComponent<SaveSerialization>().SaveData();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    public void LoadLevel(LevelManager.LevelType _type){
        if (persistent != null) persistent.GetComponent<SaveSerialization>().SaveData();

        switch (_type)
        {
            case LevelManager.LevelType.HUB:
                LoadHub();
            break;

            case LevelManager.LevelType.WASTELAND:
                LoadWasteland();
            break;

            case LevelManager.LevelType.CAVES:
                LoadCaves();
            break;

            case LevelManager.LevelType.VOLCANO:
                LoadVolcano();
            break;

            case LevelManager.LevelType.VAULT:
                LoadValt();
            break;
        }
    }

    private void LoadHub()
    {
        SceneManager.LoadScene("Level_Hub");
    }

    private void LoadWasteland()
    {
        SceneManager.LoadScene("Level_Wasteland_VAR" + Random.Range(1, 3));
    }

    private void LoadCaves()
    {
        SceneManager.LoadScene("Level_Cave_VAR" + Random.Range(1, 3));
    }

    private void LoadVolcano()
    {
        SceneManager.LoadScene("Level_Volcano_VAR" + Random.Range(1, 3));
    }

    private void LoadValt()
    {
        SceneManager.LoadScene("Level_Vault_VAR" + Random.Range(1, 3));
    }

    private void LoadWin()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Win");
    }

    private void Exit()
    {
        if (persistent != null) persistent.GetComponent<SaveSerialization>().SaveData();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Application.Quit();
    }

    private void Update()
    {
        
    }
}