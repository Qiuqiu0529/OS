using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InstructionMgr : MonoBehaviour
{
    int instructionID;//当前指令id
    int insCount;//已执行数
    [SerializeField] bool[] isPass = new bool[320];//标记是否轮到过,调整为 [SerializeField] 便于观察
    public Slider simSpeed,audioVolume;
    public AudioSource bgm;
    public Text announce;
    public Text nowID, lastID, preID;

    float waitTime = 1f;
    bool isSim;
    bool endSim;
    [SerializeField] bool dir;//false重复跳转到前地址部分，true跳转到后地址部分
    [SerializeField] bool nextMethod;//false表示顺序执行，true前后转

    public static InstructionMgr instance;// 单例模式便于管理调用
    void Start()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        Init();
    }
    public void Init()
    {
        instructionID = -1;
        insCount = 0;
        for (int i = 0; i < isPass.Length; ++i)
        {
            isPass[i] = false;
        }//置空
    }
    public void Clear()//复原文字信息等
    {
        announce.text = "";
        nowID.text = "null";
        lastID.text = "null";
        preID.text = "null";
        isSim = false;
        StopCoroutine(Simulation());
        Init();
    }
    public void StartSim()//开始模拟
    {
        if (!isSim)
        {
            endSim = false;
            isSim = true;
            announce.text = "开始模拟";
            StartCoroutine(Simulation());//开始模拟
        }
        else
        {
            if (endSim)
            {
                Clear();
                MemoryMgr.instance.Clear();
                endSim = false;
                isSim = true;
                announce.text = "开始模拟";
                StartCoroutine(Simulation());//开始模拟
            }
            else
                announce.text = "请先重置！";//不能在模拟时重复开始
        }
    }
    public void ChangeAudioVolume()//利用滑动条调整BGM音量
    {
        bgm.volume = audioVolume.value;
    }
    public void ChangeWaitTime()//利用滑动条调整模拟速度
    {
        waitTime = simSpeed.value;
    }
    public void ChangeInsText()//更改执行指令文字
    {
        preID.text = lastID.text;
        lastID.text = nowID.text;
        nowID.text = instructionID.ToString();
    }
    public int RandomID()//获得全局随机
    {
        int next = Random.Range(0, 320);
        while (isPass[next])//若已被执行，顺延执行，直到找到
        {
            next = (next + 1) % 320;
        }
        return next;
    }
    public int NextIns()//获取下一个应执行指令
    {
        int next = -1;
        if (!nextMethod)//顺序执行
        {
            next = (instructionID + 1) % 320;
            while (isPass[next])//若已被执行，顺延执行，直到找到
            {
                next = (next + 1) % 320;
            }
            nextMethod = true;//切换模式
            return next;
        }
        //非顺延时
        if (!dir)//向前
        {
            if (instructionID - 1 > 0)
            {
                next = Random.Range(0, instructionID - 1);//按照顺延情况，
                                                          //instruction以及前一个都被执行了
                int times = 0;
                int length = instructionID - 1;
                while (isPass[next])
                {
                    times++;
                    next = (next + 1) % (length);//区域内顺延
                    if (times > length)//已满，全局随机找数跳出循环
                    {
                        next = RandomID();
                        break;
                    }
                }
            }
            else//超范围随机全局随机选一个
            {
                next = RandomID();
            }
            dir = true;
        }
        else//向后
        {
            if (instructionID + 1 < 320)
            {
                next = Random.Range(instructionID + 1, 320);//按照顺延情况，instruction被执行
                int times = 0;
                int length = 319 - instructionID;
                while (isPass[next])
                {
                    times++;
                    next = (next - instructionID) % length + instructionID + 1;//区域内顺延
                    if (times > length)//已满，全局随机找数跳出循环
                    {
                        next = RandomID();
                        break;
                    }
                }
            }
            else//超范围随机全局随机选一个
            {
                next = RandomID();
            }
            dir = false;
        }

        nextMethod = false;
        return next;
    }
    IEnumerator Simulation()//模拟过程
    {
        dir = false;
        nextMethod = false;
        instructionID = Random.Range(0, 320);//初始随机选择中心指令
        while (isSim)
        {
            isPass[instructionID] = true;//已被执行标记
            ChangeInsText();//修改文字
            MemoryMgr.instance.ChoosePage(instructionID);//调用内存分配
            yield return new WaitForSeconds(waitTime);//等待
            insCount++;//计数增加
            if (insCount == 320)
            {
                MemoryMgr.instance.PrintFinal();//显示缺页率
                endSim = true;
                break;
            }
            instructionID = NextIns();//获取下一条指令
        }
        yield return 0;
    }
}
