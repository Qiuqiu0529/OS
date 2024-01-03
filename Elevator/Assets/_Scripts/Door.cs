using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    int currentLevel;
    public bool[] up = new bool[21];//21较方便
    public bool[] down = new bool[21];

    [SerializeField] int levelLimit;

    [Header("UI")]//UI修改
    [SerializeField] Text levelnow;
    [SerializeField] GameObject pressDown;
    [SerializeField] GameObject pressUp;

    [SerializeField] GameObject pressDownButton;
    [SerializeField] GameObject pressUpButton;


    public static Door instance;//单例模式便于管理
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        currentLevel = 1;
    }

    public void SwitchLevel(int level)//显示对应楼层门的信息，不论梯号，同享按键bool值
    {
        if (level >= 0 && level <= levelLimit)
        {
            currentLevel = level;
            levelnow.text = currentLevel.ToString();
            pressDown.SetActive(down[level]);
            pressUp.SetActive(up[level]);
            if (currentLevel == 1)
            {
                pressDownButton.SetActive(false);
            }
            else
            {
                pressDownButton.SetActive(true);
            }
            if (currentLevel == 20)
            {
                pressUpButton.SetActive(false);
            }
            else
            {
                pressUpButton.SetActive(true);
            }
        }
    }

    public void UpPress()//当前层按上
    {
        if (!up[currentLevel])//避免重复
        {
            up[currentLevel] = true;
            pressUp.SetActive(up[currentLevel]);
            Command command = new Command(currentLevel, true);//发送指令
            ElevatorMgr.instance.AddCommand(command);
        }
    }

    public void DownPress()//当前层按下
    {
        if (!down[currentLevel])//避免重复
        {
            down[currentLevel] = true;
            pressDown.SetActive(down[currentLevel]);
            Command command = new Command(currentLevel, false);//发送指令
            ElevatorMgr.instance.AddCommand(command);
        }
    }

    public void AchieveUp(int level)
    {
        up[level] = false;
        if (level == currentLevel)//若恰好为当前显示层，修改ui图片
        {
            pressUp.SetActive(false);
        }
    }

    public void AchieveDown(int level)
    {
        down[level] = false;
        if (level == currentLevel)//若恰好为当前显示层，修改ui图片
        {
            pressDown.SetActive(false);
        }
    }
}
