using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandUtil
{
    public static bool RandBool()
    {
        return Random.Range(0, 1000) > 500;
    }

    //  随机，如果是 True，执行 CallBack
    public static void IfTrue(UnityAction callBack)
    {
        if(RandBool())
        {
            callBack.Invoke();
        }
    }

}
