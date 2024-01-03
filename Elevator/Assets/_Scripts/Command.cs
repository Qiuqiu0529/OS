using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command 
{
    public int level;
    public bool upOrdown;
    public float startTime;
    public Command(int x,bool upOrdown)
    {
        this.level = x;
        this.upOrdown = upOrdown;
        startTime = Time.time;
    }
}
