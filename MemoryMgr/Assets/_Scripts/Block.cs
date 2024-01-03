using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public Text message;
    public Image image;
    public void ChangeColorToChoose()//改变颜色,表示正在执行
    {
        image.color = Global.choose;
    }
    public void ChangeColorToNormal()//回归原状
    {
        image.color = Global.normal;
    }
    public void ChangeText(string content)//改变文字
    {
        message.text = content;
    }

}
