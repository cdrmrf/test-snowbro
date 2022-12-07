using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemyVariable : ATK
{
    //  出生的地点
    public Vector2 bornPosition;
    //  死亡后的回调
    public UnityAction dieCallBack;
    //  默认状态
    public EnemyStateEnum state = EnemyStateEnum.FLY;
    //  移动方向
    internal float dir = -0.4f;

    private void Start()
    {
        base.power = GameSetting.AttackPower.SNOW_BALL_ATTACK;
    }

    public void Die(bool reward)
    {
        Destroy(GetComponent<Collider2D>());
        Destroy(gameObject);
        if(reward)
        {
            RandUtil.IfTrue(() => RewardManage.instance.RandomReward(transform));
        }
        if (dieCallBack != null)
        {
            dieCallBack.Invoke();
        }
    }

    public bool IsMoving()
    {
        return state == EnemyStateEnum.MOVE || state == EnemyStateEnum.FLY;
    }

    public bool CanRoll()
    {
        return state == EnemyStateEnum.SNOWBALL;
    }

    public void Roll(float dir)
    {
        state = EnemyStateEnum.ROLL;
        transform.tag = TagManage.SNOWBALL;
        transform.gameObject.layer = LayerMask.NameToLayer("RollingSnowBall");
        this.dir = dir;
    }

    private void Update()
    {
        //  游戏结束
        if(!Global.IsPlaying())
        {
            Destroy(gameObject);
        }
    }

    //  移动状态 和 雪球状态下，碰到版边就消失
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == TagManage.ENEMY_DEAD_ZONE)
        {
            this.Die(false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.tag == TagManage.ENEMY_DEAD_ZONE)
        {
            this.Die(false);
        }
    }

}
