using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InstructionMgr : MonoBehaviour
{
    int instructionID;//��ǰָ��id
    int insCount;//��ִ����
    [SerializeField] bool[] isPass = new bool[320];//����Ƿ��ֵ���,����Ϊ [SerializeField] ���ڹ۲�
    public Slider simSpeed,audioVolume;
    public AudioSource bgm;
    public Text announce;
    public Text nowID, lastID, preID;

    float waitTime = 1f;
    bool isSim;
    bool endSim;
    [SerializeField] bool dir;//false�ظ���ת��ǰ��ַ���֣�true��ת�����ַ����
    [SerializeField] bool nextMethod;//false��ʾ˳��ִ�У�trueǰ��ת

    public static InstructionMgr instance;// ����ģʽ���ڹ������
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
        }//�ÿ�
    }
    public void Clear()//��ԭ������Ϣ��
    {
        announce.text = "";
        nowID.text = "null";
        lastID.text = "null";
        preID.text = "null";
        isSim = false;
        StopCoroutine(Simulation());
        Init();
    }
    public void StartSim()//��ʼģ��
    {
        if (!isSim)
        {
            endSim = false;
            isSim = true;
            announce.text = "��ʼģ��";
            StartCoroutine(Simulation());//��ʼģ��
        }
        else
        {
            if (endSim)
            {
                Clear();
                MemoryMgr.instance.Clear();
                endSim = false;
                isSim = true;
                announce.text = "��ʼģ��";
                StartCoroutine(Simulation());//��ʼģ��
            }
            else
                announce.text = "�������ã�";//������ģ��ʱ�ظ���ʼ
        }
    }
    public void ChangeAudioVolume()//���û���������BGM����
    {
        bgm.volume = audioVolume.value;
    }
    public void ChangeWaitTime()//���û���������ģ���ٶ�
    {
        waitTime = simSpeed.value;
    }
    public void ChangeInsText()//����ִ��ָ������
    {
        preID.text = lastID.text;
        lastID.text = nowID.text;
        nowID.text = instructionID.ToString();
    }
    public int RandomID()//���ȫ�����
    {
        int next = Random.Range(0, 320);
        while (isPass[next])//���ѱ�ִ�У�˳��ִ�У�ֱ���ҵ�
        {
            next = (next + 1) % 320;
        }
        return next;
    }
    public int NextIns()//��ȡ��һ��Ӧִ��ָ��
    {
        int next = -1;
        if (!nextMethod)//˳��ִ��
        {
            next = (instructionID + 1) % 320;
            while (isPass[next])//���ѱ�ִ�У�˳��ִ�У�ֱ���ҵ�
            {
                next = (next + 1) % 320;
            }
            nextMethod = true;//�л�ģʽ
            return next;
        }
        //��˳��ʱ
        if (!dir)//��ǰ
        {
            if (instructionID - 1 > 0)
            {
                next = Random.Range(0, instructionID - 1);//����˳�������
                                                          //instruction�Լ�ǰһ������ִ����
                int times = 0;
                int length = instructionID - 1;
                while (isPass[next])
                {
                    times++;
                    next = (next + 1) % (length);//������˳��
                    if (times > length)//������ȫ�������������ѭ��
                    {
                        next = RandomID();
                        break;
                    }
                }
            }
            else//����Χ���ȫ�����ѡһ��
            {
                next = RandomID();
            }
            dir = true;
        }
        else//���
        {
            if (instructionID + 1 < 320)
            {
                next = Random.Range(instructionID + 1, 320);//����˳�������instruction��ִ��
                int times = 0;
                int length = 319 - instructionID;
                while (isPass[next])
                {
                    times++;
                    next = (next - instructionID) % length + instructionID + 1;//������˳��
                    if (times > length)//������ȫ�������������ѭ��
                    {
                        next = RandomID();
                        break;
                    }
                }
            }
            else//����Χ���ȫ�����ѡһ��
            {
                next = RandomID();
            }
            dir = false;
        }

        nextMethod = false;
        return next;
    }
    IEnumerator Simulation()//ģ�����
    {
        dir = false;
        nextMethod = false;
        instructionID = Random.Range(0, 320);//��ʼ���ѡ������ָ��
        while (isSim)
        {
            isPass[instructionID] = true;//�ѱ�ִ�б��
            ChangeInsText();//�޸�����
            MemoryMgr.instance.ChoosePage(instructionID);//�����ڴ����
            yield return new WaitForSeconds(waitTime);//�ȴ�
            insCount++;//��������
            if (insCount == 320)
            {
                MemoryMgr.instance.PrintFinal();//��ʾȱҳ��
                endSim = true;
                break;
            }
            instructionID = NextIns();//��ȡ��һ��ָ��
        }
        yield return 0;
    }
}
