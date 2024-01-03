using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryMgr : MonoBehaviour
{
    public static MemoryMgr instance;//单例模式便于管理调用
    public Page[] pages;
    public int LastPage;
    public int usedPageCount = 0;
    [SerializeField] bool algorithm;//false表FIFO，true表LRU
                                    //[SerializeField]便于debug
    int lostPageCount;//缺页数
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
    public void Clear()//复原
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
    public void ChangeToFIFO()//算法更改为FIFO
    {
        algorithm = false;
    }
    public void ChangeToLRU()//算法更改为LRU
    {
        algorithm = true;
    }
    public void PrintFinal()//打印缺页率
    {
        double lostCount = lostPageCount * 100;
        announce.text = "模拟结束,缺页率：" + (lostCount / 320).ToString() + "%";
    }
    public int ChangeFIFOLast()//获得FIFO时需替换的页
    {
        LastPage = (LastPage + 1) % 4;//使用FIFO时替换，修改该替换页的号，按顺序轮流替换
        return LastPage;
    }
    public int ChangeLRULast()//获得LRU时需替换的页
    {
        int id = 0;
        int maxLeisure = 0;
        for (int i = 0; i < pages.Length; ++i)//使用LRU时，比较闲置次数
        {
            if (pages[i].leisureCount > maxLeisure)
            {
                id = i;
                maxLeisure = pages[i].leisureCount;//LRU替换最闲的那个
            }
        }
        return id;
    }
    public void ChoosePage(int pos)//选择页
    {
        lastBlock.ChangeColorToNormal();
        int pageID = InPage(pos);
        if (pageID != -1)
        {
            return;//已存在，无需调度
        }
        ++lostPageCount;
        int changePage = 0;
        if (usedPageCount < 4)//尚有空闲，直接分配
        {
            ++usedPageCount;

            for (int i = 0; i < pages.Length; ++i)
            {
                if (pages[i].startNum == -1)
                {
                    changePage = i;
                    announce.text = "存在空闲页，调用物理第" + changePage.ToString() + "页";//打印调用信息
                    break;
                }
            }
        }
        else
        {
            if (algorithm)//lru调度
                changePage = ChangeLRULast();
            else//FIFO调度
                changePage = ChangeFIFOLast();
            announce.text = "替换物理第" + changePage.ToString() + "页内容";//打印调用信息

        }
        pages[changePage].ChangeText(pos);//修改ui文字
        lastBlock = pages[changePage].blocks[pos % 10];//提取个位数
        lastBlock.ChangeColorToChoose();//修改颜色
    }
    public int InPage(int pos)//判断是否已在其中&修改闲置次数
    {
        int temp = -1;
        for (int i = 0; i < pages.Length; ++i)
        {
            if (pages[i].startNum > -1)
            {
                if (pages[i].startNum == pos / 10)//提取起始位置
                {
                    lastBlock = pages[i].blocks[pos % 10];//提取个位数
                    lastBlock.ChangeColorToChoose();
                    pages[i].RestLeisureCount();//被使用，清空闲置次数
                    announce.text = "指令已在内存中，无需调页";
                    temp = i;//已在内存中,记录页号
                }
                else
                {
                    pages[i].AddLeisureCount();//已被占未使用时闲置次数+1
                }
            }
        }
        return temp;//不在内存中
    }

}
