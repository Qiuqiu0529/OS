using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorMgr : MonoBehaviour
{
    public static ElevatorMgr instance;//一个电梯管理用于调度

    public Elevator[] elevators;

    [SerializeField] int elevatorCount = 5;

    public int elevatorNow;

   
    [Header("UI")]
    [SerializeField] Text elevatorID;//梯号
    [SerializeField] Text elevatorlevelNow;//当前电梯到的地方
    [SerializeField] Text insidelevelNow;//当前电梯到的地方
    [SerializeField] Text podcastEleID;
    [SerializeField] Text podcastLevelID;
    [SerializeField] Slider timeBetL;
    [SerializeField] Slider timeInL;
    [SerializeField] Slider timeAlert;

    [SerializeField] GameObject[] press;//显示楼层按钮
    [SerializeField] GameObject upPic;//显示上行
    [SerializeField] GameObject downPic;//显示下行
    [SerializeField] GameObject alertPic;//显示警告图片

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    #region Timechange
    public void ChangeTimeBetL()
    {
        foreach (var temp in elevators)
        {
            temp.waitBetweenLevel = timeBetL.value;
        }
    }
    public void ChangeTimeInL()
    {
        foreach (var temp in elevators)
        {
            temp.waitInLevel = timeInL.value;
        }
    }
    public void ChangeTimeAlert()
    {
        foreach (var temp in elevators)
        {
            temp.alertTime = timeAlert.value;
        }
    }
    #endregion
    public void AddCommand(Command command)//选择用时最短的
    {
        int tempid = 1;
        float costTimeMin = 9999;
        for (int i = 1; i <= elevatorCount; ++i)//选择
        {
            float tempTime = elevators[i - 1].CostTime(command);
            if (tempTime < costTimeMin)
            {
                tempid = i;
                costTimeMin = tempTime;
            }
        }
        elevators[tempid - 1].Adddst(command.level);
    }

    #region UIPic
    public void SwitchElevatorID(int id)//1-5
    {
        if (id >= 1 && id <= elevatorCount)
        {
            elevatorNow = id;
            elevatorID.text = id.ToString();
            elevatorlevelNow.text = elevators[elevatorNow - 1].levelNow.ToString();//显示层
            insidelevelNow.text = elevators[elevatorNow - 1].levelNow.ToString();//显示层
            for (int i = 0; i < 20; ++i)
            {
                press[i].SetActive(elevators[elevatorNow - 1].isPress[i + 1]);
            }
            alertPic.SetActive(false);
            if (elevators[elevatorNow - 1].dir == 1)
            {
                ShowUpPic(elevatorNow);
            }
            else if (elevators[elevatorNow - 1].dir == -1)
            {
                ShowDownPic(elevatorNow);
            }
            if (elevators[elevatorNow - 1].state.GetType() == typeof(ReadyState))
            {
                ShowReadyPic(elevatorNow);
            }
            if (elevators[elevatorNow - 1].state.GetType() == typeof(AlertState))
            {
                ShowAlertPic(elevatorNow);
            }
        }
    }

    public void InsideLevelButtonPress(int level)//梯内按钮按下
    {
        if (elevators[elevatorNow - 1].isPress[level])
        {
            return;//当前层已按
        }
        elevators[elevatorNow - 1].isPress[level] = true;
        elevators[elevatorNow - 1].Adddst(level);//新增目标楼层
        press[level - 1].SetActive(true);
    }

    public void InsideLevelButtonRelease(int id, int level)//梯内按钮松开
    {
        elevators[id - 1].isPress[level] = false;
        if (id == elevatorNow)
        {
            Debug.Log(level);
            press[level - 1].SetActive(false);
        }
    }

    public void ShowLevel(int id)
    {
        if (id == elevatorNow)
        {
            elevatorlevelNow.text = elevators[elevatorNow - 1].levelNow.ToString();//显示层
            insidelevelNow.text = elevators[elevatorNow - 1].levelNow.ToString();//显示层
        }
    }

    public void ShowReadyPic(int id)
    {
        if (id == elevatorNow)
        {
            upPic.SetActive(false);
            downPic.SetActive(false);
        }
    }

    public void ShowUpPic(int id)
    {
        if (id == elevatorNow)
        {
            upPic.SetActive(true);
            downPic.SetActive(false);

        }
    }

    public void ShowDownPic(int id)
    {
        if (id == elevatorNow)
        {
            upPic.SetActive(false);
            downPic.SetActive(true);
        }
    }

    public void ShowAlertPic(int id)
    {
        if (id == elevatorNow)
        {
            alertPic.SetActive(true);
            upPic.SetActive(false);
            downPic.SetActive(false);
        }
    }
    #endregion
    public void PressAlert()
    {
        elevators[elevatorNow - 1].Alert();
        ShowAlertPic(elevatorNow);
    }

    public void ReleaseAlert(int id)
    {
        if (id == elevatorNow)
        {
            alertPic.SetActive(false);
        }
    }

    public void OpenDoor()
    {
        elevators[elevatorNow - 1].OpenDoor();
    }

    public void CloseDoor()
    {
        elevators[elevatorNow - 1].CloseDoor();
    }

    public void PodCast(int eleID, int levelID)
    {
        podcastEleID.text = (eleID).ToString();
        podcastLevelID.text = levelID.ToString();
    }

}
