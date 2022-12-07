using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemyRunLogic : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rbody;
    private EnemyVariable variable;
    private string curFloorName;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        variable = GetComponent<EnemyVariable>();
    }
    
    void FixedUpdate()
    {
        if(variable.state == EnemyStateEnum.FLY)
        {
            this.EnemyFly();
        } else if (variable.state == EnemyStateEnum.MOVE)
        { 
            this.EnemyRun();
        }
    }

    //  小怪出生后会飞行一段距离，到达指定地点后落地开始移动
    private void EnemyFly()
    {
        float curX = transform.position.x;
        if (curX > variable.bornPosition.x)
        {
            //  出生后默认是往左边飞的
            transform.Translate(Vector2.left * 0.05f);
        }
        else
        {
            //  达到指定地点，降落
            var pos = transform.position;
            pos.y = variable.bornPosition.y;
            transform.position = pos;
            //  飞行结束，开始移动
            variable.state = EnemyStateEnum.MOVE;
            animator.SetBool("IsRun", true);
            rbody.isKinematic = false;
        }
    }

    private void EnemyRun()
    {
        transform.Translate(Vector2.right * variable.dir * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsStateSupport())
        {
            return;
        }

        string tag = collision.collider.tag;
        //  撞到强以后改变方向
        if (tag == TagManage.WALL)
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            variable.dir *= -1;
        }
        else if (collision.collider.tag == TagManage.FLOOR)
        {
            //  落到地板上
            animator.ResetTrigger("IsJump");
            var n = collision.collider.name;
            if(curFloorName == null)
            {
                curFloorName = n;
            }
            else if(n != curFloorName)
            {
                //  落到下一层地板后调换移动方向
                //  这层地板是从左往右跑，下层地板就是从右往左跑
                GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
                variable.dir *= -1;
                curFloorName = n;
            }
        }
        else if (collision.collider.tag == TagManage.SNOWBALL)
        {
            //  当我撞到一个雪球
            variable.Die(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(!IsStateSupport())
        {
            return;
        }
        //  跳起来
        if (collision.collider.tag == TagManage.FLOOR)
        {
            animator.ResetTrigger("IsJump");
            animator.SetTrigger("IsJump");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == TagManage.BULLET)
        {
            //  跳跃过程中碰到子弹，重置动画
            animator.ResetTrigger("IsJump");
            //  恢复重力
            if(rbody.isKinematic)
            {
                rbody.isKinematic = false;
            }
        }
    }

    private bool IsStateSupport()
    {
        return variable.state == EnemyStateEnum.MOVE || variable.state == EnemyStateEnum.FLY;
    }

}
