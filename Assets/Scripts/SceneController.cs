using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneController : MonoBehaviour
{
    void Awake() {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GetComponent<DontDestroy>() && !GetComponent<DontDestroy>().CheckValidInstance()) return;
    }

    public void LoadPrototype()
    {
        SceneManager.LoadScene("PROTOTYPE");
    }
    public void LoadMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    public void LoadLevel(LevelManager.LevelType _type){
        //HUB
        //WASTELAND
        //CAVES
        //VOLCANO
        //RUINS

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

    public void LoadHub()
    {
        SceneManager.LoadScene("Level_Hub");
    }

    public void LoadWasteland()
    {
        SceneManager.LoadScene("Level_Wasteland_VAR" + Random.Range(1, 3));
    }

    public void LoadCaves()
    {
        SceneManager.LoadScene("Level_Cave_VAR" + Random.Range(1, 3));
    }

    public void LoadVolcano()
    {
        SceneManager.LoadScene("Level_Volcano_VAR" + Random.Range(1, 3));
    }

    public void LoadValt()
    {
        SceneManager.LoadScene("Level_Vault_VAR" + Random.Range(1, 3));
    }

    public void LoadWin()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Win");
    }

    public void Exit()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Application.Quit();
    }

    public void Update()
    {
        
    }
}