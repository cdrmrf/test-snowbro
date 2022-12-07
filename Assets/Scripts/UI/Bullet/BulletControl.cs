using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : ATK
{
    private Rigidbody2D rbody;
    //  飞行速度
    [HideInInspector]
    public float speed = 1.0f;
    //  销毁时间
    [HideInInspector]
    public float destroyTime = 2f;
    //  普通子弹还是加粗子弹
    public bool isBig = false;

    private float _timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        //  子弹的攻击力
        base.power = isBig ? GameSetting.AttackPower.BIG_BULLET_ATTACK : GameSetting.AttackPower.BULLET_ATTACK;
    }

    private void OnEnable()
    {
        rbody = GetComponent<Rigidbody2D>();
        base.power = isBig ? GameSetting.AttackPower.BIG_BULLET_ATTACK : GameSetting.AttackPower.BULLET_ATTACK;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //  游戏结束，删除自己
        if(!Global.IsPlaying())
        {
            Destroy(gameObject);
            return;
        }

        _timer += Time.deltaTime;
        bool flip = GetComponent<SpriteRenderer>().flipX;
        if(flip)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        } else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        //  飞行时间 = 消失时间 * 50%
        float flyingTime = destroyTime * 0.5f;
        //  恢复重力模式
        //  下降重力 0.15f
        if(_timer >= flyingTime && rbody.isKinematic)
        {
            rbody.isKinematic = false;
        }
        //  消灭子弹
        if (_timer >= destroyTime)
        {
            DestroyBullet();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != TagManage.REWARD)
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        //  隐藏子弹
        gameObject.SetActive(false);
    }

    public void ResetAndShow()
    {
        //  重置属性，显示子弹
        _timer = 0;
        rbody.isKinematic = true;
        gameObject.SetActive(true);
    }

}
