using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionEnum
{
    JUMP_LOW,
    JUMP_HIGH,
}

public struct Action
{
    public ActionEnum action;

    public float number;

    public Action(ActionEnum a, float b)
    {
        action = a;
        number = b;
    }
}

public class BossAction
{

    private static Action curAction = GetNextAction();

    //  BOSS 的下一个动作，大跳只有 1次，小跳是随机次数
    public static Action GetNextAction()
    {
        switch (curAction.action)
        {
            case ActionEnum.JUMP_LOW:
                curAction = new Action(ActionEnum.JUMP_HIGH, 1);
                break;
            case ActionEnum.JUMP_HIGH:
                curAction = new Action(ActionEnum.JUMP_LOW, UnityEngine.Random.Range(0, 4));
                break;
            default:
                curAction = new Action(ActionEnum.JUMP_LOW, UnityEngine.Random.Range(0, 4));
                break;
        }
        return curAction;
    }

}
