using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Page : MonoBehaviour
{
    public Block[] blocks;
    public Text pageNum;//显示目前逻辑页
    [SerializeField]int pageID;
    public int leisureCount=0;//空闲次数
    public int startNum;//起始
    public void PageClear()//置空复原
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
    public void AddLeisureCount()//增加闲置次数
    {
        ++leisureCount;
    }
    public void RestLeisureCount()//清空闲置次数
    {
        leisureCount = 0;
    }
    public void ChangeText(int pos)//更改文字内容
    {
        int i = 0;
        startNum = pos/10;//修改起始
        int num = startNum * 10;//修改block数字
        pageNum.text = startNum.ToString();
        leisureCount = 0;
        foreach (var block in blocks)
        {
            block.ChangeText((num + i).ToString());
            ++i;
        }
    }

}
