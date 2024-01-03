using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    int currentLevel;
    public bool[] up = new bool[21];//21�Ϸ���
    public bool[] down = new bool[21];

    [SerializeField] int levelLimit;

    [Header("UI")]//UI�޸�
    [SerializeField] Text levelnow;
    [SerializeField] GameObject pressDown;
    [SerializeField] GameObject pressUp;

    [SerializeField] GameObject pressDownButton;
    [SerializeField] GameObject pressUpButton;


    public static Door instance;//����ģʽ���ڹ���
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        currentLevel = 1;
    }

    public void SwitchLevel(int level)//��ʾ��Ӧ¥���ŵ���Ϣ�������ݺţ�ͬ����boolֵ
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

    public void UpPress()//��ǰ�㰴��
    {
        if (!up[currentLevel])//�����ظ�
        {
            up[currentLevel] = true;
            pressUp.SetActive(up[currentLevel]);
            Command command = new Command(currentLevel, true);//����ָ��
            ElevatorMgr.instance.AddCommand(command);
        }
    }

    public void DownPress()//��ǰ�㰴��
    {
        if (!down[currentLevel])//�����ظ�
        {
            down[currentLevel] = true;
            pressDown.SetActive(down[currentLevel]);
            Command command = new Command(currentLevel, false);//����ָ��
            ElevatorMgr.instance.AddCommand(command);
        }
    }

    public void AchieveUp(int level)
    {
        up[level] = false;
        if (level == currentLevel)//��ǡ��Ϊ��ǰ��ʾ�㣬�޸�uiͼƬ
        {
            pressUp.SetActive(false);
        }
    }

    public void AchieveDown(int level)
    {
        down[level] = false;
        if (level == currentLevel)//��ǡ��Ϊ��ǰ��ʾ�㣬�޸�uiͼƬ
        {
            pressDown.SetActive(false);
        }
    }
}
