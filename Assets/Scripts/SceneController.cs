using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
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

    public void LoadHub()
    {
        SceneManager.LoadScene("Level_Hub");
    }

    public void LoadWasteland()
    {
        SceneManager.LoadScene("Level_Wasteland");
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
}