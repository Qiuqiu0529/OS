using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ElevatorBaseState
{
    void StateChange();
    void LeaveState();
}

public class MovingState : ElevatorBaseState//
{
    float timer;
    Elevator elevator;
    public MovingState(Elevator ele)
    {
        elevator = ele;
        elevator.ShowUpDownPic();
        Debug.Log("MovingState" + elevator.dstCount.ToString());
    }

    public void StateChange()
    {
        if (elevator.dstCount == 0)//地点为空
        {
            elevator.SetElevatorState(new ReadyState(elevator));
            return;
        }
        if (elevator.levelNow == elevator.dstNow)
        {
            elevator.SetElevatorState(new WaitState(elevator));
            return;
        }

        if (timer >= elevator.waitBetweenLevel)//层级切换在这
        {
            elevator.LevelChange();
            if (elevator.levelNow == elevator.dstNow)
            {
                elevator.SetElevatorState(new WaitState(elevator));
                return;
            }
            else
            {
                timer = 0;
            }
        }
        timer += Time.fixedDeltaTime;
    }
    public void LeaveState()
    { }
}
public class ReadyState : ElevatorBaseState//
{
    Elevator elevator;
    public ReadyState(Elevator ele)
    {
        elevator = ele;
        elevator.dir = 0;
        elevator.ReadyPic();
        Debug.Log(" ReadyState");
    }
    public void StateChange()
    {
    }
    public void LeaveState()
    { }
}
public class WaitState : ElevatorBaseState//
{
    float timer;
    Elevator elevator;
    public WaitState(Elevator ele)
    {
        elevator = ele;
        timer = 0;
        
        elevator.WaitInLevel();
        Debug.Log("WaitState");
    }

    public void StateChange()
    {
        if (timer >= elevator.waitInLevel)
        {
            if (elevator.dstCount == 0)//地点为空
            {
                elevator.SetElevatorState(new ReadyState(elevator));
                return;
            }
           
            elevator.SetElevatorState(new MovingState(elevator));
            return;

        }
        if (elevator.closedoorEarly)
        {
            elevator.closedoorEarly = false;
            timer += 1.5f;//按一次加快1.5s
        }
        if (!elevator.opendoor)
        {
            timer += Time.fixedDeltaTime;
        }
    }
    public void LeaveState()
    { }
}
public class AlertState : ElevatorBaseState//
{
    float timer;
    Elevator elevator;
    public AlertState(Elevator ele)
    {
        elevator = ele;
        timer = 0;
        Debug.Log("AlertState");
    }
    public void StateChange()//alert时暂停运行
    {
        if (timer >= elevator.alertTime)
        {
            elevator.SetElevatorState(new MovingState(elevator));
            return;
        }
        timer += Time.fixedDeltaTime;
    }
    public void LeaveState()
    {
        elevator.ReleaseAlert();
    }
}