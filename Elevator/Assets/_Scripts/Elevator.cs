using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour//һ�������൱��һ���߳�
{
    [SerializeField] int ID;//ID��1-5
    public float waitBetweenLevel;
    public float waitInLevel;
    public float alertTime;

    public int levelNow;//1-20
    public bool[] isPress = new bool[21];

    public bool stop;
    public bool opendoor;
    public bool closedoorEarly;

    public List<int> dsts;//�żӵ��ݰ�ť�赽��ص�
    public int dstCount;
    public int pointdst;
    public int dstNow;
    public int dir;
    public ElevatorBaseState state;
    [SerializeField] string statement;

    private void Start()
    {
        levelNow = 1;
        dstNow = 1;
        pointdst = 0;
        dir = 0;
        state = new ReadyState(this);
        statement = state.GetType().ToString();
    }

    public void SetElevatorState(ElevatorBaseState newState)
    {
        state.LeaveState();
        state = newState;
        statement = state.GetType().ToString();//�۲���
    }

    public void FixedUpdate()//�̶�֡�ʵ���
    {
        if (stop)
            return;
        state.StateChange();
    }

    #region Interrupt 
    //���濪����
    public void ReadyPic()
    {
        ElevatorMgr.instance.ShowReadyPic(ID);
    }
    public void Alert()
    {
        if (state.GetType() != typeof(AlertState))
        {
            SetElevatorState(new AlertState(this));
        }
    }
    public void ReleaseAlert()
    {
        ElevatorMgr.instance.ReleaseAlert(ID);
    }

    public void OpenDoor()
    {
        if (state.GetType() == typeof(WaitState))//��¥���ͣ��ʱ���ܿ�����
        {
            StopCoroutine(DoorOpen());
            opendoor = true;
            StartCoroutine(DoorOpen());
        }
    }
    IEnumerator DoorOpen()
    {
        yield return new WaitForSeconds(3f);
        opendoor = false;
    }
    public void CloseDoor()
    {
        if (state.GetType() == typeof(WaitState))//��¥���ͣ��ʱ���ܿ�����
        {
            closedoorEarly = true;
            StartCoroutine(DoorClose());
        }
    }
    IEnumerator DoorClose()
    {
        yield return new WaitForSeconds(1f);
        closedoorEarly = false;
    }
    #endregion

    #region choose
    public void WaitInLevel()//����,�޸ķ����
    {
        ElevatorMgr.instance.PodCast(ID, levelNow);
        ElevatorMgr.instance.InsideLevelButtonRelease(ID, levelNow);//�����ڰ�ť��ʾ
        pointdst = FindIndex(dstNow);
        Debug.Log(pointdst);
        if (pointdst != -1)
            dsts.RemoveAt(pointdst);//��ɾһ��,֮���п����ж����ɾ��

        dstCount = dsts.Count;
        if (dstCount == 0)//���һ���ص㣬����������ȡ��
        {
            if (Door.instance.up[levelNow])
                Door.instance.AchieveUp(levelNow);
            else if (Door.instance.down[levelNow])
                Door.instance.AchieveDown(levelNow);
            return;
        }
        else
        {
            if (dir == 1)
            {
                if (Door.instance.up[levelNow])
                {
                    Door.instance.AchieveUp(levelNow);
                    pointdst = FindIndex(levelNow);
                    if (pointdst != -1)
                    {
                        dsts.RemoveAt(pointdst);
                    }
                }
                pointdst = FirstLarge(levelNow);
                if (pointdst != -1)
                {
                    dstNow = dsts[pointdst];
                }
                else
                {
                    dir = -1;
                    pointdst = FirstLess(levelNow);
                    if (pointdst != -1)
                    {
                        dstNow = dsts[pointdst];
                    }
                    else
                    {
                        dstNow = levelNow;
                    }
                }
            }
            else if (dir == -1)
            {
                if (Door.instance.down[levelNow])
                {
                    Door.instance.AchieveDown(levelNow);
                    pointdst = FindIndex(levelNow);
                    if (pointdst != -1)
                    {
                        dsts.RemoveAt(pointdst);
                    }
                }
                pointdst = FirstLess(levelNow);
                if (pointdst != -1)
                {
                    dstNow = dsts[pointdst];
                }
                else
                {
                    dir = 1;
                    pointdst = FirstLarge(levelNow);
                    if (pointdst != -1)
                    {
                        dstNow = dsts[pointdst];
                    }
                    else
                    {
                        dstNow = levelNow;
                    }
                }
            }
        }
        ShowUpDownPic();

    }

    public int FirstLarge(int level)
    {
        int i = -1;
        for (i = 0; i < dsts.Count; i++)
        {
            if (dsts[i] > level)
                return i;
        }
        return -1;
    }
    public int FirstLess(int level)
    {
        int i = -1;
        for (i = dsts.Count - 1; i >= 0; i--)
        {
            if (dsts[i] < level)
                return i;
        }
        return -1;
    }

    public void ShowUpDownPic()
    {
        if (dstNow > levelNow)
        {
            ElevatorMgr.instance.ShowUpPic(ID);
        }
        else if (dstNow < levelNow)
        {
            ElevatorMgr.instance.ShowDownPic(ID);
        }
    }
    public void Adddst(int level)//����Ŀ��¥��
    {
        dsts.Add(level);
        dstCount = dsts.Count;
        dsts.Sort();
        if (state.GetType() == typeof(ReadyState))
        {
            dstNow = dsts[0];
            pointdst = 0;
            dstCount = dsts.Count;
            dir = (dstNow >= levelNow) ? 1 : -1;//������
            ShowUpDownPic();
            state = new MovingState(this);
            return;
        }

        if (dir == 1)//���ܻ��޸ĵ�ǰĿ��
        {
            if (level <= dstNow && level > levelNow)
            {
                if (Door.instance.up[level] || (isPress[level]))
                    dstNow = level;
            }
        }
        else if (dir == -1)
        {
            if (level < levelNow && level >= dstNow)
            {
                if (Door.instance.down[level] || (isPress[level]))
                    dstNow = level;
                //����һ�»���Դ�ڵ����ڲ�
            }
        }
    }

    public int FindIndex(int level)
    {
        int i = -1;
        for (i = 0; i < dsts.Count; i++)
        {
            if (dsts[i] == level)
                return i;
        }
        return -1;
    }

    public void LevelChange()//¥��仯
    {
        levelNow += dir;
        ElevatorMgr.instance.ShowLevel(ID);
    }

    public float CostTime(Command command)
    {
        float time = 0;
        if (state.GetType() == typeof(ReadyState) || dstCount == 0)//Ŀ�ĵ�Ϊ��ʱ
        {
            time = waitBetweenLevel * Mathf.Abs(command.level - levelNow);//ֱ�ӷ��ز㼶���ֵ
        }
        else
        {
            if (state.GetType() == typeof(AlertState))
                time += alertTime;
            time += waitInLevel * (dsts.Count + 1);
            //����һЩ��Զ��(�෴����)¥��ĸ���
            int top = dsts[dstCount - 1];
            int bottom = dsts[0];
            if (dir == 1)
            {
                if (command.level >= levelNow)//�����Ϸ�
                {
                    if (command.upOrdown || command.level >= dsts[dstCount - 1])
                    {
                        time += waitBetweenLevel * (command.level - levelNow);//Ŀǰ¥��Ŀ��¥
                    }
                    else
                    {
                        time += waitBetweenLevel *
                            (dsts[dstCount - 1] - levelNow + dsts[dstCount - 1] - command.level);//�����۷�
                    }
                }
                else
                {
                    if (command.upOrdown)
                    {
                        if (command.level >= dsts[0])
                        {
                            time += waitBetweenLevel * (dsts[dstCount - 1] - levelNow +
                               dsts[dstCount - 1] - dsts[0] + command.level - dsts[0]);//Ŀǰ¥��Ŀ��¥
                        }
                        else
                        {
                            time += waitBetweenLevel * (dsts[dstCount - 1] - levelNow +
                               dsts[dstCount - 1] - command.level);//Ŀǰ¥��Ŀ��¥
                        }
                    }
                    else
                    {
                        time += waitBetweenLevel * (dsts[dstCount - 1] - levelNow +
                               dsts[dstCount - 1] - command.level);//Ŀǰ¥��Ŀ��¥
                    }
                }
            }
            else if (dir == -1)
            {
                if (command.level >= levelNow)//�����Ϸ�
                {
                    if (command.upOrdown || command.level >= dsts[dstCount - 1])
                    {
                        time += waitBetweenLevel * (levelNow - dsts[0] + command.level - dsts[0]);//Ŀǰ¥��Ŀ��¥
                    }
                    else
                    {
                        time += waitBetweenLevel * (levelNow - dsts[0] + dsts[dstCount - 1] - dsts[0]
                            + dsts[dstCount - 1] - command.level);
                    }
                }
                else
                {
                    if (command.upOrdown)
                    {
                        if (command.level >= dsts[0])
                        {
                            time += waitBetweenLevel * (levelNow - dsts[0] + command.level - dsts[0]);//Ŀǰ¥��Ŀ��¥
                        }
                        else
                        {
                            time += waitBetweenLevel * (levelNow - command.level);//Ŀǰ¥��Ŀ��¥
                        }
                    }
                    else
                    {
                        time += waitBetweenLevel * (levelNow - command.level);//Ŀǰ¥��Ŀ��¥
                    }
                }
            }

        }
        return time;
    }
    #endregion
}
