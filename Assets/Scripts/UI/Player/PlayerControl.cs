using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RewardControl;
using static AnimatorParamManage.Player;
using static AnimatorParamManage;
using UnityEngine.Playables;

public class PlayerControl : MonoBehaviour
{

    public BulletManage bulletManage;

    //  游戏边界，飞行的时候不要超出这个边界
    public Transform leftWall;
    public Transform rightWall;
    public Transform topWall;
    public Transform bottomWall;

    private PlayerBuffState playerBuffState;
    private PlayerAnimationControl animationControl;
    private Rigidbody2D rbody;

    private Vector2 defaultPosition;
    private bool isStart = false;
    private bool isOnFloor = true;

    //  碰到雪球时跟随雪球一起滚动
    private GameObject followSnowBall;

    private void Awake()
    {
        EventCenter.Instance.AddEventListener2(EventEnum.PLAYER_BORN, Born);
        EventCenter.Instance.AddEventListener2(EventEnum.PLAYER_DIE, Reset);
    }

    void Start()
    {
        playerBuffState = GetComponent<PlayerBuffState>();
        animationControl = GetComponent<PlayerAnimationControl>();
        rbody = GetComponent<Rigidbody2D>();
        defaultPosition = transform.position;
    }

    //  角色死亡，清空状态，设定倒计时重生
    private void Reset()
    {
        //  默认朝左边
        GetComponent<SpriteRenderer>().flipX = false;
        playerBuffState.Reset();
        transform.position = defaultPosition;
        GetComponent<Rigidbody2D>().gravityScale = 1f;
        //  2秒后播放复活的动画
        Invoke("PlayRebornAnimation", 2f);
    }

    private void PlayRebornAnimation()
    {
        animationControl.Born();
    }

    //  角色出生
    private void Born()
    {
        if (isStart)
        {
            return;
        }
        //  重置状态
        isStart = true;
        isOnFloor = true;
        //  调整动画
        animationControl.Idle();
        animationControl.StartTransparentMode();
    }

    // Update is called once per frame
    void Update()
    {
        //  游戏未开始
        if (!isStart)
        {
            return;
        }
        //  游戏结束了，删掉
        if(!Global.IsPlaying())
        {
            gameObject.SetActive(false);
            return;
        }

        //  跟随雪球移动
        if (followSnowBall != null)
        {
            MoveWithSnowBall();
            return;
        }

        //  角色移动
        var horizontal = Input.GetAxis(AxisConstant.Horizontal);
        var vertical = Input.GetAxis(AxisConstant.Vertical);
        //  无敌模式全屏飞行
        if(playerBuffState.isBigFaceMode)
        {
            float s = 1f;
            float curX = transform.position.x + (Vector2.right * s * Time.deltaTime * horizontal).x;
            float curY = transform.position.y + (Vector2.up * s * Time.deltaTime * vertical).y;
                
            if(horizontal != 0 && curX >= leftWall.position.x && curX <= rightWall.position.x)
            {
                transform.Translate(Vector2.right * s * Time.deltaTime * horizontal);
            }
            if(vertical != 0 && curY <= topWall.position.y && curY >= bottomWall.position.y)
            {
                transform.Translate(Vector2.up * s * Time.deltaTime * vertical);
            }
        } else if (horizontal == 0)
        {
            //  静静地站着
            animationControl.Idle();
        } else
        {
            //  跑起来
            animationControl.Run();
            //  移动
            float s = playerBuffState.enhanceBulletSpeed ? 0.5f : 0.4f;
            transform.Translate(Vector2.right * s * Time.deltaTime * horizontal);
            GetComponent<SpriteRenderer>().flipX = horizontal > 0;
        }

        //  飞行模式下只能移动，不能 射击/跳跃
        if (!playerBuffState.isBigFaceMode)
        {
            //  靠近敌人后展示推雪球的动画
            DisplayPushAnimation(horizontal);
            //  发射子弹
            if (Input.GetKeyDown(KeyCode.J))
            {
                this.Fire(horizontal);
            }
            //  跳跃
            if (Input.GetKeyDown(KeyCode.K) && isOnFloor)
            {
                animationControl.Jump();
                //  跳跃后脱离雪球
                followSnowBall = null;
            }
        }
    }

    //  碰撞到雪球之后和雪球一起滚动
    private void MoveWithSnowBall()
    {
        //  关闭跑步的动画
        animationControl.Idle();
        //  自己和雪球绑定在一起
        var snowBallPos = followSnowBall.transform.position;
        Vector2 newPos = Vector2.zero;
        newPos.x = snowBallPos.x;
        newPos.y = snowBallPos.y;
        transform.position = newPos;
    }

    /// <summary>
    /// 发射子弹，需要判断，如果身前是雪球，那就是推动雪球，否则就是发射子弹。
    /// </summary>
    private void Fire(float horizontal)
    {
        //  检查前方有没有雪球
        bool face2right = GetComponent<SpriteRenderer>().flipX;
        GameObject enemy = RaycastUtil.FindSnowBall(transform.position, face2right ? 1 : -1);
        Debug.Log($"Fire, enemy={enemy}, face2left={face2right}");
        //  滚雪球
        if(enemy != null)
        {
            //  确定滚动方向，如果 player 雪球在左边，雪球就往右边滚动
            bool rollToRight = GetComponent<SpriteRenderer>().flipX;
            var dir1 = rollToRight ? 1.5f : -1.5f;
            enemy.GetComponent<EnemyVariable>().Roll(dir1);
        } else
        {
            //  发射子弹，子弹有 CD时间，不一定发射成功
            bool res = bulletManage.Fire(playerBuffState.enhanceBulletSpeed, playerBuffState.enhanceBulletDistance, playerBuffState.enhanceBulletSize);
            animationControl.CancelAttack();
            if (res)
            {
                animationControl.Attack();
            }
        }
    }

    /// <summary>
    /// 靠近敌人后展示推雪球的动画
    /// 2D射线检测前方有没有敌人，如果有敌人检测敌人状态
    /// 如果敌人是雪球状态展示 Push 动画，如果不是恢复为 Idle 状态
    /// </summary>
    /// <param name="horizontal"></param>
    private void DisplayPushAnimation(float horizontal)
    {
        bool foundSnowBall = false;
        //  靠近雪球，并且在移动状态，展示 Push 动画
        if (horizontal != 0)
        {
            var s = RaycastUtil.FindSnowBall(transform.position, horizontal);
            //  寻找雪球
            foundSnowBall = s != null;
        }
        //  当前是静止状态，或者眼前没有雪球
        if (horizontal == 0 || !foundSnowBall)
        {
            animationControl.CancelPush();
        } else
        {
            //  有敌人 + 雪球状态，更新动画，展示 PUSH 动画
            animationControl.Push();
        }
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
        if (!isStart)
        {
            return;
        }
        string tag = collision.tag;
        if (tag == TagManage.FLOOR && rbody.velocity.y <= 0)
        {
            animationControl.CancelJump();
            isOnFloor = true;
        } else if(tag == TagManage.BOSS && !playerBuffState.isTransparentMode)
        {
            Die();
        }
        else if (tag == TagManage.ENEMY)
        {
            //  大脸模式，碰到敌人，敌人死掉
            var enemy = collision.GetComponent<EnemyVariable>();
            if(playerBuffState.isBigFaceMode)
            {
                enemy.Die(true);
            } else if (!playerBuffState.isTransparentMode && enemy.IsMoving())
            {
                //  不是无敌模式，敌人移动中，玩家死亡
                Die();
            }
        }
        else if (tag == TagManage.SNOWBALL)
        {
            //  碰到了滚动中的雪球，进入无敌状态，然后向上跳一下
            if (isOnFloor)
            {
                animationControl.Jump();
            }
            animationControl.StartTransparentMode();
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
        //  跳跃过程中无敌
        if (!isOnFloor)
        {
            return;
        }
        //  暂停游戏
        isStart = false;
        animationControl.Die();
        //  屏蔽碰撞体
        //var col = GetComponent<CapsuleCollider2D>();
        //col.enabled = false;
        //  切换到透明图层
        transform.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
        Global.PLAYER_LIFE--;
        GameOverCheck();
    }

    private void GameOverCheck()
    {
        //  玩家死亡，移除
        if (Global.PLAYER_LIFE <= 0)
        {
            Global.GAME_STATUS = Global.GameStatusEnum.GAME_OVER;
            EventCenter.Instance.EventTrigger2(EventEnum.GAME_STATUS_CHANGE);
            Destroy(gameObject);
        }
    }

}
