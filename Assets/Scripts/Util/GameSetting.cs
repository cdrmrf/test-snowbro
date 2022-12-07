using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting
{

    public class Enemy
    {
        public const int MAX_ENEMY_COUNT = 4;

        public const float ENEMY_HP = 1.5f;

        public const int BOSS_HP = 200;

        private static float SNOWBALL_UNFREEE_TIME = 2f;

        public static float GetSnowBallUnFreezeTime(int hitCount)
        {
            return hitCount * SNOWBALL_UNFREEE_TIME;
        }

    }

    public class Player
    {

        //  跳跃CD
        public const float JUMP_CD = 0.25f;

        //  射击CD
        public const float FIRE_CD = 0.05f;

        //  无敌模式事件
        public const float TRANSPARENT_MODE_TIME = 3f;

        //  玩家生命
        public const int PLAYER_LIFE = 2;

        //  绿药水大脸无敌模式
        public const float BIG_FACE_MODE_TIME = 6f;
    }

    //  攻击力配置
    public class AttackPower
    {
        //  普通子弹攻击
        public const int BULLET_ATTACK = 10;

        //  吃了蓝药水的子弹攻击
        public const int BIG_BULLET_ATTACK = 10;

        //  雪球攻击
        public const int SNOW_BALL_ATTACK = 10;
    }

}
