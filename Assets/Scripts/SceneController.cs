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

    public void LoadLevel(BountyManager.LevelType _type){
        //HUB
        //WASTELAND
        //CAVES
        //VOLCANO
        //RUINS

        switch (_type)
        {
            case BountyManager.LevelType.HUB:
                LoadHub();
            break;

            case BountyManager.LevelType.WASTELAND:
                LoadWasteland();
            break;

            case BountyManager.LevelType.CAVES:
                LoadCaves();
            break;

            case BountyManager.LevelType.VOLCANO:
                LoadVolcano();
            break;

            case BountyManager.LevelType.RUINS:
                LoadRuins();
            break;
        }
    }

    public void LoadHub()
    {
        SceneManager.LoadScene("Level_Hub");
    }

    public void LoadWasteland()
    {
        SceneManager.LoadScene("Level_Wasteland_VAR1");
    }

    public void LoadCaves()
    {
        SceneManager.LoadScene("Level_Caves");
    }

    public void LoadVolcano()
    {
        SceneManager.LoadScene("Level_Volcano");
    }

    public void LoadRuins()
    {
        SceneManager.LoadScene("Level_Ruins");
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
        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            LoadMenu();
        }

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            LoadLevel(BountyManager.LevelType.HUB);
        }

        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            LoadLevel(BountyManager.LevelType.WASTELAND);
        }

    }
}