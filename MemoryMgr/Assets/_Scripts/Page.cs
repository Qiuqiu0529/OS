using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Page : MonoBehaviour
{
    public Block[] blocks;
    public Text pageNum;//��ʾĿǰ�߼�ҳ
    [SerializeField]int pageID;
    public int leisureCount=0;//���д���
    public int startNum;//��ʼ
    public void PageClear()//�ÿո�ԭ
    {
        string temp = "null";
        pageNum.text = "-1";
        startNum = -1;
        leisureCount = 0;
        foreach (var block in blocks)
        {
            block.ChangeText(temp);
        }
    }
    public void AddLeisureCount()//�������ô���
    {
        ++leisureCount;
    }
    public void RestLeisureCount()//������ô���
    {
        leisureCount = 0;
    }
    public void ChangeText(int pos)//������������
    {
        int i = 0;
        startNum = pos/10;//�޸���ʼ
        int num = startNum * 10;//�޸�block����
        pageNum.text = startNum.ToString();
        leisureCount = 0;
        foreach (var block in blocks)
        {
            block.ChangeText((num + i).ToString());
            ++i;
        }
    }

}
