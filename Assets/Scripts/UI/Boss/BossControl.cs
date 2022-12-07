using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// 跳跃规则：
/// 随机几次小跳，一次向上大跳
/// 大跳之后，随机几次小跳，向下大跳
/// 
/// 从上往下跳，小兵发射在上两层地板上
/// 从下往上跳，小兵发射在下两层地板上
///
/// 创建小怪的规则：
/// 每次跳起时创建小怪，用 Boss 的 y 坐标和左侧地板的 y 坐标对比
/// 如果 Boss 此时的位置高于地板，那块地板上没有小怪，并且当前活着的小怪数量 < 4
/// 那就可以创建
///
/// 小怪出生的位置是在 Boss 的身体中间，然后快速发射到左侧地板上面
/// 小怪到达地板之后，开始移动
/// </summary>
public class BossControl : MonoBehaviour
{
    private int curEnemyCount = 0;
    private bool isOnFloor = true;
    private float countdown = 0f;

    //  敌人
    public EnemyManage enemyManage;
    public GameObject[] leftFloors;

    private Animator animator;
    private Rigidbody2D rbody;

    //  下一个动作
    private Action curAction;
    //  脚下踩着的地板
    private GameObject floor;
    //  boss 当前的位置
    private Vector2 bossPostion = Vector2.down;

    void Start()
    {
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
        curAction = BossAction.GetNextAction();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Global.IsPlaying())
        {
            return;
        }
        if(Global.BOSS_HP <= 0)
        {
            return;
        }

        //  每次跳起时发射小怪
        if (!isOnFloor)
        {
            //  随机，50% 几率发射小怪
            //RandUtil.IfTrue(this.Fire);
            this.Fire();
            return;
        }
        //  每次动作的间隔
        countdown -= Time.deltaTime;
        if(countdown > 0)
        {
            return;
        }
        //  BOSS 的下一个动作
        float number = curAction.number;
        var action = curAction.action;
        switch (action)
        {
            case ActionEnum.JUMP_LOW:
                //  小跳
                number -= 1;
                rbody.AddForce(Vector2.up * 120);
                countdown = 0.3f;
                animator.SetBool("IsJump", true);
                break;
            case ActionEnum.JUMP_HIGH:
                //  大跳
                number -= 1;
                //  大跳分为向上跳和向下跳
                if (bossPostion == Vector2.down)
                {
                    rbody.AddForce(Vector2.up * 250);
                    bossPostion = Vector2.up;
                } else
                {
                    //  向下跳的时候先把脚下的地板屏蔽了，因为这个地板只有 BOSS 能踩到，所以直接屏蔽
                    if(floor.GetComponent<PlatformEffector2D>() != null)
                    {
                        floor.gameObject.SetActive(false);
                    }
                    rbody.AddForce(Vector2.up * 50);
                    bossPostion = Vector2.down;
                }
                animator.SetBool("IsJump", true);
                countdown = 0.6f;
                break;
        }
        curAction.number = number;
        //  下一个动作
        if (number <= 0)
        {
            curAction = BossAction.GetNextAction();
            action = curAction.action;
            number = curAction.number;
        }
    }

    //  发射敌人
    private void Fire()
    {
        if (curEnemyCount >= GameSetting.Enemy.MAX_ENEMY_COUNT)
        {
            return;
        }
        //  小怪诞生在哪块地板上面
        GameObject nextBornFloor = null;
        foreach (GameObject newPlace in leftFloors)
        {
            FloorControl floor = newPlace.GetComponent<FloorControl>();
            if(floor == null)
            {
                continue;
            }
            //  地板的高度，boss 的高度超过地板时触发
            float floorPositionY = newPlace.transform.position.y;
            if (transform.position.y <= floorPositionY)
            {
                continue;
            }
            //  地板上必须没有敌人
            if(floor.HasEnemy())
            {
                break;
            }
            //  减少难度，有玩家就跳过这块地板
            if(floor.HasPlayer())
            {
                break;
            }
            nextBornFloor = newPlace;
            break;
        }
        if(nextBornFloor == null)
        {
            return;
        }
        curEnemyCount++;
        //  创建敌人
        UnityAction afterEnemyDie = () => this.curEnemyCount--;
        enemyManage.CreateEnemy(transform, nextBornFloor, afterEnemyDie);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //  被攻击了
        var atk = collider.gameObject.GetComponent<ATK>();
        if(atk != null)
        {
            if(Global.BOSS_HP > atk.power)
            {
                Global.BOSS_HP -= atk.power;
            } else
            {
                Global.BOSS_HP = 0;
                Die();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == TagManage.FLOOR)
        {
            isOnFloor = true;
            animator.SetBool("IsJump", false);
            if (floor != null && !floor.activeSelf) {
                floor.SetActive(true);
            }
            floor = collision.collider.gameObject;
        } else
        {
            OnTriggerEnter2D(collision.collider);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == TagManage.FLOOR)
        {
            isOnFloor = false;
        }
    }

    private void Die()
    {
        if(!Global.IsPlaying())
        {
            return;
        }
        //  死亡动画
        animator.SetTrigger("DieTrigger");
        //  更新游戏状态
        Global.GAME_STATUS = Global.GameStatusEnum.WIN;
        EventCenter.Instance.EventTrigger2(EventEnum.GAME_STATUS_CHANGE);
    }

}
