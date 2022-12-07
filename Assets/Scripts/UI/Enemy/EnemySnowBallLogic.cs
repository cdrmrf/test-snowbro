using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemySnowBallLogic : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rbody;
    private EnemyVariable variable;
    private GameObject player;

    private float HP = GameSetting.Enemy.ENEMY_HP;
    //  被子弹击中的次数
    private int hitCount = 1;
    //  化冻时间
    private float freezeTime = 0f;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        variable = GetComponent<EnemyVariable>();
    }

    void FixedUpdate()
    {
        if(!IsStateSupport())
        {
            return;
        }
        //  雪球解冻
        if(variable.state == EnemyStateEnum.FREEZE || variable.state == EnemyStateEnum.SNOWBALL)
        {
            this.UnFreeze();
        }
        //  雪球移动
        if(variable.state == EnemyStateEnum.ROLL)
        {
            this.SnowBallGoGoGo();
        }
    }

    //  雪球化冻
    private void UnFreeze()
    {
        if(variable.state != EnemyStateEnum.FREEZE && variable.state != EnemyStateEnum.SNOWBALL)
        {
            return;
        }
        //  按照时间解冻
        freezeTime -= Time.deltaTime;
        if(freezeTime > 0)
        {
            return;
        }
        variable.state = EnemyStateEnum.FREEZE;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        hitCount = hitCount - 1 <= 0 ? 0 : hitCount - 1;
        freezeTime = GameSetting.Enemy.GetSnowBallUnFreezeTime(hitCount);
        animator.SetFloat("FreezeState", hitCount);
        //  完全解冻
        if(hitCount <= 0)
        {
            variable.state = EnemyStateEnum.MOVE;
        }
    }

    //  雪球不断滚动
    private void SnowBallGoGoGo()
    {
        //  检查动画是否正确
        if(!animator.GetBool("IsSnowBall"))
        {
            animator.SetBool("IsSnowBall", true);
        }
        //  滚动
        transform.Translate(Vector2.right * variable.dir * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //  飞行或者运动过程中碰到子弹，变成 初级雪球
        if (collision.tag == TagManage.BULLET && variable.IsMoving())
        {
            //  展现为一个半冻住的雪球
            hitCount = 1;
            HP = GameSetting.Enemy.ENEMY_HP * hitCount;
            freezeTime = GameSetting.Enemy.GetSnowBallUnFreezeTime(hitCount);
            animator.SetFloat("FreezeState", hitCount);
            variable.state = EnemyStateEnum.FREEZE;
        }
        //  冰冻状态下的碰撞事件
        else if (variable.state == EnemyStateEnum.FREEZE || variable.state == EnemyStateEnum.SNOWBALL)
        {
            this.OnCollisionWhenFreeze(collision);
        }
        //  变成雪球后的滚动事件
        else if (variable.state == EnemyStateEnum.ROLL)
        {
            this.OnCollisitionWhenRoll(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (variable.state == EnemyStateEnum.ROLL)
        {
            this.OnCollisitionWhenRoll(collision.collider);
        } else if (collision.collider.tag == TagManage.ENEMY_DEAD_ZONE)
        {
            variable.Die(false);
        } else if (collision.collider.tag == TagManage.SNOWBALL && variable.state == EnemyStateEnum.SNOWBALL)
        {
            Global.SCORE += 200;
            //  碰到了雪球，自己也是雪球，滚动起来
            var vars = collision.gameObject.GetComponent<EnemyVariable>();
            variable.Roll(vars.dir);
            vars.dir *= -1;
        }
    }

    //  冷冻状态下的碰撞事件
    private void OnCollisionWhenFreeze(Collider2D collision)
    {
        //  碰到子弹后变成雪球状态
        if (collision.tag == TagManage.BULLET)
        {
            Global.SCORE += 20;
            //  跳跃过程中碰到子弹，重置动画
            animator.ResetTrigger("IsJump");
            //  雪球只有三种状态
            if (hitCount >= 3)
            {
                gameObject.layer = LayerMask.NameToLayer("SnowBall");
                freezeTime = GameSetting.Enemy.GetSnowBallUnFreezeTime(hitCount);
                return;
            }
            //  子弹的大小
            var bulletControl = collision.GetComponent<BulletControl>();
            bool isBigBullet = bulletControl.isBig;
            if (isBigBullet)
            {
                HP -= 1f;
            }
            else
            {
                HP -= 0.9f;
            }
            //  雪球状态变化
            if (HP <= 0)
            {
                hitCount += 1;
                animator.SetFloat("FreezeState", hitCount);
                HP = GameSetting.Enemy.ENEMY_HP;
            }
            freezeTime = GameSetting.Enemy.GetSnowBallUnFreezeTime(hitCount);
            if (hitCount >= 3)
            {
                variable.state = EnemyStateEnum.SNOWBALL;
                //  从敌人图层切换为雪球图层，
                //  如果一个敌人这个时候是雪球状态，玩家靠近后会显示推动的动画，但是无法推动雪球，
                //  因为玩家图层碰不到敌人图层，所以要切换到玩家能碰到的图层
                gameObject.layer = LayerMask.NameToLayer("SnowBall");
            }
        }
        else if (collision.tag == TagManage.SNOWBALL && variable.state == EnemyStateEnum.FREEZE)
        {
            Global.SCORE += 200;
            //  碰到了雪球，自己当前没有完全变成雪球，自己 死
            variable.Die(true);
        }
        else if (collision.tag == TagManage.SNOWBALL && variable.state == EnemyStateEnum.SNOWBALL)
        {
            Global.SCORE += 200;
            //  碰到了雪球，自己也是雪球，滚动起来
            var vars = collision.gameObject.GetComponent<EnemyVariable>();
            variable.Roll(vars.dir);
            vars.dir *= -1;
        } else if(collision.tag == TagManage.PLAYER && variable.state == EnemyStateEnum.SNOWBALL)
        {
            //  玩家撞击雪球，检测玩家位置，如果玩家在雪球下方，玩家和雪球一起跳起来
            bool findPlayer = RaycastUtil.FindByTag(transform.position, Vector2.down, 2f, TagManage.PLAYER);
            if (findPlayer)
            {
                //  获取玩家的方向
                bool face2Right = player.GetComponent<SpriteRenderer>().flipX;
                //  雪球向上跳动
                rbody.AddForce(Vector2.up * 120);
                var p = transform.position;
                p.x += face2Right ? 0.1f : -0.1f;
                transform.position = p;
                //  玩家继续向上跳动，
                //  玩家一开始向上跳动时因为碰到了雪球，跳跃行为被终止
                //  这里手动让玩家继续跳一次
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 50);
            }
        }
    }

    //  雪球滚动中的碰撞事件
    //  玩家，敌人的碰撞事件代码写在了它们自己的代码里
    private void OnCollisitionWhenRoll(Collider2D collision)
    {
        string tag = collision.tag;
        //  撞到墙以后改变方向
        if (tag == TagManage.WALL)
        {
            variable.dir *= -1;
        }
        else if (tag == TagManage.BOSS)
        {
            Global.SCORE += 200;
            variable.Die(false);
        }
    }

    //  雪球被玩家顶起
    public void SnowBallJump()
    {
        if(variable.state != EnemyStateEnum.SNOWBALL)
        {
            return;
        }
        rbody.AddForce(Vector2.up * 140);
    }

    private bool IsStateSupport()
    {
        return
            variable.state == EnemyStateEnum.FLY ||
            variable.state == EnemyStateEnum.MOVE ||
            variable.state == EnemyStateEnum.FREEZE ||
            variable.state == EnemyStateEnum.SNOWBALL ||
            variable.state == EnemyStateEnum.ROLL;
    }

}
