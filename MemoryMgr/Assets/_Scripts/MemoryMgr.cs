using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryMgr : MonoBehaviour
{
    public static MemoryMgr instance;//����ģʽ���ڹ������
    public Page[] pages;
    public int LastPage;
    public int usedPageCount = 0;
    [SerializeField] bool algorithm;//false��FIFO��true��LRU
                                    //[SerializeField]����debug
    int lostPageCount;//ȱҳ��
    public Text announce;
    public Block lastBlock;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        lostPageCount = 0;
    }
    public void Clear()//��ԭ
    {
        LastPage = -1;
        usedPageCount = 0;
        lostPageCount = 0;
        lastBlock.ChangeColorToNormal();
        foreach (var page in pages)
        {
            page.PageClear();
        }
    }
    public void ChangeToFIFO()//�㷨����ΪFIFO
    {
        algorithm = false;
    }
    public void ChangeToLRU()//�㷨����ΪLRU
    {
        algorithm = true;
    }
    public void PrintFinal()//��ӡȱҳ��
    {
        double lostCount = lostPageCount * 100;
        announce.text = "ģ�����,ȱҳ�ʣ�" + (lostCount / 320).ToString() + "%";
    }
    public int ChangeFIFOLast()//���FIFOʱ���滻��ҳ
    {
        LastPage = (LastPage + 1) % 4;//ʹ��FIFOʱ�滻���޸ĸ��滻ҳ�ĺţ���˳�������滻
        return LastPage;
    }
    public int ChangeLRULast()//���LRUʱ���滻��ҳ
    {
        int id = 0;
        int maxLeisure = 0;
        for (int i = 0; i < pages.Length; ++i)//ʹ��LRUʱ���Ƚ����ô���
        {
            if (pages[i].leisureCount > maxLeisure)
            {
                id = i;
                maxLeisure = pages[i].leisureCount;//LRU�滻���е��Ǹ�
            }
        }
        return id;
    }
    public void ChoosePage(int pos)//ѡ��ҳ
    {
        lastBlock.ChangeColorToNormal();
        int pageID = InPage(pos);
        if (pageID != -1)
        {
            return;//�Ѵ��ڣ��������
        }
        ++lostPageCount;
        int changePage = 0;
        if (usedPageCount < 4)//���п��У�ֱ�ӷ���
        {
            ++usedPageCount;

            for (int i = 0; i < pages.Length; ++i)
            {
                if (pages[i].startNum == -1)
                {
                    changePage = i;
                    announce.text = "���ڿ���ҳ�����������" + changePage.ToString() + "ҳ";//��ӡ������Ϣ
                    break;
                }
            }
        }
        else
        {
            if (algorithm)//lru����
                changePage = ChangeLRULast();
            else//FIFO����
                changePage = ChangeFIFOLast();
            announce.text = "�滻�����" + changePage.ToString() + "ҳ����";//��ӡ������Ϣ

        }
        pages[changePage].ChangeText(pos);//�޸�ui����
        lastBlock = pages[changePage].blocks[pos % 10];//��ȡ��λ��
        lastBlock.ChangeColorToChoose();//�޸���ɫ
    }
    public int InPage(int pos)//�ж��Ƿ���������&�޸����ô���
    {
        int temp = -1;
        for (int i = 0; i < pages.Length; ++i)
        {
            if (pages[i].startNum > -1)
            {
                if (pages[i].startNum == pos / 10)//��ȡ��ʼλ��
                {
                    lastBlock = pages[i].blocks[pos % 10];//��ȡ��λ��
                    lastBlock.ChangeColorToChoose();
                    pages[i].RestLeisureCount();//��ʹ�ã�������ô���
                    announce.text = "ָ�������ڴ��У������ҳ";
                    temp = i;//�����ڴ���,��¼ҳ��
                }
                else
                {
                    pages[i].AddLeisureCount();//�ѱ�ռδʹ��ʱ���ô���+1
                }
            }
        }
        return temp;//�����ڴ���
    }

}
