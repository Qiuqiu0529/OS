using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class MainMenu : MonoBehaviour
{
    public void QuitGame()//�ر�exe
    {
        Application.Quit();
    }
    public void PauseMenu()//ʱ�����ٽ���0
    {
         Time.timeScale = 0f;
    }
    public void ResumeGame()//ʱ�����ٻָ�
    {
         Time.timeScale = 1f;
    }

}