using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public Text message;
    public Image image;
    public void ChangeColorToChoose()//�ı���ɫ,��ʾ����ִ��
    {
        image.color = Global.choose;
    }
    public void ChangeColorToNormal()//�ع�ԭ״
    {
        image.color = Global.normal;
    }
    public void ChangeText(string content)//�ı�����
    {
        message.text = content;
    }

}
