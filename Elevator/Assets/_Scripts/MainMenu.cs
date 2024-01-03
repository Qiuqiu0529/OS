using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class MainMenu : MonoBehaviour
{
    public void QuitGame()//关闭exe
    {
        Application.Quit();
    }
    public void PauseMenu()//时间流速降至0
    {
         Time.timeScale = 0f;
    }
    public void ResumeGame()//时间流速恢复
    {
         Time.timeScale = 1f;
    }

}