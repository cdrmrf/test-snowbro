using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateEnum 
{

    FLY,
    //  移动状态
    MOVE,
    //  冷冻状态
    FREEZE,
    //  雪球状态
    SNOWBALL,
    //  撞到版边消失
    HIDE,
    //  撞到雪球消失
    DIE,
    ROLL

}
