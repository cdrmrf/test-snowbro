using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//  玩家动画控制
public class PlayerAnimationControl : MonoBehaviour
{

    private const float IDLE_STATE = 0;
    private const float RUN_STATE = 1;
    private const float SPEED_UP_STATE = 2;

    private Animator animator;
    private Rigidbody2D rbody;
    private SpriteRenderer spriteRenderer;
    private PlayerBuffState playerBuffState;

    //  无敌模式配置，持续时间，变身颜色
    private float transparentModeTime = 0f;
    private int transparentModeColorIdx = 0;
    private List<Color> transparentModeColors = new List<Color>();

    //  绿药水大脸模式 timer
    private float _BFtimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerBuffState = GetComponent<PlayerBuffState>();
        animator = GetComponent<Animator>();

        transparentModeColors.Add(Color.cyan);
        transparentModeColors.Add(Color.red);
        transparentModeColors.Add(Color.yellow);
        transparentModeColors.Add(Color.white);
    }

    internal void Born()
    {
        animator.SetBool("IsBorn", true);
    }

    internal void Idle()
    {
        var speed = playerBuffState.enhanceBulletSpeed;
        animator.SetFloat("MoveState", speed ? SPEED_UP_STATE : IDLE_STATE);
    }

    internal void Run()
    {
        var speed = playerBuffState.enhanceBulletSpeed;
        animator.SetFloat("MoveState", speed ? SPEED_UP_STATE : RUN_STATE);
    }

    internal void Push()
    {
        animator.SetBool("IsPush", true);
    }

    internal void CancelPush()
    {
        animator.SetBool("IsPush", false);
    }

    internal void Die()
    {
        animator.SetTrigger("DieTrigger");
    }

    internal void Attack()
    {
        animator.SetTrigger("AttackTrigger");
    }

    internal void CancelAttack()
    {
        animator.ResetTrigger("AttackTrigger");
    }

    internal void Jump()
    {
        this.CancelAttack();
        rbody.AddForce(Vector2.up * 140);
        animator.SetTrigger("JumpTrigger");
    }

    internal void CancelJump()
    {
        animator.ResetTrigger("JumpTrigger");
    }

    internal void StartTransparentMode()
    {
        //  进入无敌时间
        playerBuffState.isTransparentMode = true;
        transparentModeTime = GameSetting.Player.TRANSPARENT_MODE_TIME;
        transparentModeColorIdx = 0;
        transform.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
    }

    private void FixedUpdate()
    {
        //  吃了绿药水，大脸模式
        if(playerBuffState.isBigFaceMode && !rbody.isKinematic)
        {
            StartBigFaceMode();
        }
        //  关闭无敌模式
        this.StopTransparentMode();
        //  关闭大脸模式
        this.StopBigFaceMode();
    }

    //  更新无敌模式，无敌模式有效的情况下，变换身上的颜色
    //  时间一到，就变回普通样子
    private float tempTimeCounter1 = 0.15f;
    private void StopTransparentMode()
    {
        if(!playerBuffState.isTransparentMode)
        {
            return;
        }
        //  无敌模式结束
        if (transparentModeTime <= 0 && transparentModeColorIdx == transparentModeColors.Count)
        {
            if (playerBuffState.isTransparentMode)
            {
                playerBuffState.isTransparentMode = false;
                transform.gameObject.layer = LayerMask.NameToLayer("UI");
            }
            return;
        }
        //  0.15秒变一次颜色
        tempTimeCounter1 -= Time.deltaTime;
        transparentModeTime -= Time.deltaTime;
        if (tempTimeCounter1 > 0)
        {
            return;
        }

        tempTimeCounter1 = 0.15f;
        spriteRenderer.color = transparentModeColors[transparentModeColorIdx++];
        if (transparentModeTime > 0 && transparentModeColorIdx == transparentModeColors.Count)
        {
            transparentModeColorIdx = 0;
        }
    }

    //  吃了绿药水，进入无敌状态
    //  大脸模式有两个动画，先播放第一个动画，0.3秒后切换到满屏幕飞的动画
    //  关闭也是这样两步
    internal void StartBigFaceMode()
    {
        rbody.isKinematic = true;
        animator.SetFloat("BigFaceState", 1);
        Invoke("StartBigFaceStep2", 0.5f);
        //  大脸模式和无敌模式有冲突，强制改为 UI 图层
        transform.gameObject.layer = LayerMask.NameToLayer("UI");
    }

    internal void StartBigFaceStep2()
    {
        animator.SetFloat("BigFaceState", 2);
        _BFtimer = GameSetting.Player.BIG_FACE_MODE_TIME;
    }

    private void StopBigFaceMode()
    {
        if (!playerBuffState.isBigFaceMode || _BFtimer == 0f)
        {
            return;
        }
        _BFtimer -= Time.deltaTime;
        //  模式结束
        if(_BFtimer <= 0)
        {
            //  关闭动画
            animator.SetFloat("BigFaceState", 1);
            Invoke("StopBigFaceStep2", 0.5f);
        }
    }

    private void StopBigFaceStep2()
    {
        //  关闭动画
        animator.SetFloat("BigFaceState", 0);
        //  切换闲置动画
        Idle();
        //  更新状态，标签，缩放
        playerBuffState.isBigFaceMode = false;
        rbody.isKinematic = false;
        gameObject.tag = TagManage.PLAYER;
        GetComponent<Transform>().localScale = new Vector3(0.08f, 0.08f, 0.08f);
    }

}
