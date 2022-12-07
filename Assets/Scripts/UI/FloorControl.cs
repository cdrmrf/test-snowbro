using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorControl : MonoBehaviour
{
    public bool hasPlayer = false;
    public bool hasEnemy = false;

    public bool HasPlayer()
    {
        return hasPlayer;
    }

    public bool HasEnemy()
    {
        return hasEnemy;
    }

    //  小怪被创建后还在飞行的过程中，这个时候还没有触发碰撞事件
    //  所以要手动更新标记
    public void CreateEnemy()
    {
        hasEnemy = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnColliderEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnColliderExit(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnColliderEnter(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnColliderExit(collision.collider);
    }

    private void OnColliderEnter(Collider2D collision)
    {
        if(collision.tag == TagManage.ENEMY || collision.tag == TagManage.SNOWBALL)
        {
            hasEnemy = true;
        }
        if(collision.tag == TagManage.PLAYER)
        {
            hasPlayer = true;
        }
    }

    private void OnColliderExit(Collider2D collision)
    {
        if (collision.tag == TagManage.ENEMY || collision.tag == TagManage.SNOWBALL)
        {
            hasEnemy = false;
        }
        if (collision.tag == TagManage.PLAYER)
        {
            hasPlayer = false;
        }
    }

}
