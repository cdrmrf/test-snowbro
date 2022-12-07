using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static RewardControl;

//  玩家 Buff 状态
//  只处理奖励碰撞事件
public class PlayerBuffState : MonoBehaviour
{

    //  红药水
    internal bool enhanceBulletSpeed = false;
    //  黄药水
    internal bool enhanceBulletDistance = false;
    //  蓝药水
    internal bool enhanceBulletSize = false;
    //  透明模式，无敌模式
    internal bool isTransparentMode = false;
    //  绿药水，大脸无敌模式
    internal bool isBigFaceMode = false;

    internal void Reset()
    {
        enhanceBulletSpeed = false;
        enhanceBulletDistance = false;
        enhanceBulletSize = false;
        isTransparentMode = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollision(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision(collision.collider);
    }

    private void OnCollision(Collider2D collision)
    {
        string tag = collision.tag;
        //  碰到了奖励
        if (tag == TagManage.REWARD)
        {
            var reward = collision.GetComponent<RewardControl>();
            OnCollisionReward(reward);
            Destroy(collision.gameObject);
        }
    }

    //  碰到了奖励
    private void OnCollisionReward(RewardControl reward)
    {
        var rType = reward.type;
        if (rType == RewardControl.RewardType.SCORE)
        {
            //  积分奖励
            reward.AddScore();
        }
        else if (rType == RewardControl.RewardType.BUFF)
        {
            //  BUFF 奖励
            BuffType buffType = reward.buffType;
            OnCollisionBuff(buffType, reward);
        }
    }

    //  BUFF 叠加，如果这项 buff 已经有了，就加积分。
    private void OnCollisionBuff(BuffType buffType, RewardControl reward)
    {
        switch (buffType)
        {
            case BuffType.RED:
                if (enhanceBulletSpeed)
                {
                    reward.AddScore();
                }
                else
                {
                    enhanceBulletSpeed = true;
                }
                break;
            case BuffType.BLUE:
                if (enhanceBulletSize)
                {
                    reward.AddScore();
                }
                else
                {
                    enhanceBulletSize = true;
                }
                break;
            case BuffType.YELLOW:
                if (enhanceBulletDistance)
                {
                    reward.AddScore();
                }
                else
                {
                    enhanceBulletDistance = true;
                }
                break;
            case BuffType.GREEN:
                //  绿药水，大脸模式，改为无敌标签
                isBigFaceMode = true;
                gameObject.tag = TagManage.FLY_PLAYER;
                //  整个人变大一点
                GetComponent<Transform>().localScale = new Vector3(0.15f, 0.15f, 0.15f);
                break;
            default:
                break;
        }
    }

}
