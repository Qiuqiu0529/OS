using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorMgr : MonoBehaviour
{
    public static ElevatorMgr instance;//һ�����ݹ������ڵ���

    public Elevator[] elevators;

    [SerializeField] int elevatorCount = 5;

    public int elevatorNow;

   
    [Header("UI")]
    [SerializeField] Text elevatorID;//�ݺ�
    [SerializeField] Text elevatorlevelNow;//��ǰ���ݵ��ĵط�
    [SerializeField] Text insidelevelNow;//��ǰ���ݵ��ĵط�
    [SerializeField] Text podcastEleID;
    [SerializeField] Text podcastLevelID;
    [SerializeField] Slider timeBetL;
    [SerializeField] Slider timeInL;
    [SerializeField] Slider timeAlert;

    [SerializeField] GameObject[] press;//��ʾ¥�㰴ť
    [SerializeField] GameObject upPic;//��ʾ����
    [SerializeField] GameObject downPic;//��ʾ����
    [SerializeField] GameObject alertPic;//��ʾ����ͼƬ

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
    public void AddCommand(Command command)//ѡ����ʱ��̵�
    {
        int tempid = 1;
        float costTimeMin = 9999;
        for (int i = 1; i <= elevatorCount; ++i)//ѡ��
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
            elevatorlevelNow.text = elevators[elevatorNow - 1].levelNow.ToString();//��ʾ��
            insidelevelNow.text = elevators[elevatorNow - 1].levelNow.ToString();//��ʾ��
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

    public void InsideLevelButtonPress(int level)//���ڰ�ť����
    {
        if (elevators[elevatorNow - 1].isPress[level])
        {
            return;//��ǰ���Ѱ�
        }
        elevators[elevatorNow - 1].isPress[level] = true;
        elevators[elevatorNow - 1].Adddst(level);//����Ŀ��¥��
        press[level - 1].SetActive(true);
    }

    public void InsideLevelButtonRelease(int id, int level)//���ڰ�ť�ɿ�
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
            elevatorlevelNow.text = elevators[elevatorNow - 1].levelNow.ToString();//��ʾ��
            insidelevelNow.text = elevators[elevatorNow - 1].levelNow.ToString();//��ʾ��
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
