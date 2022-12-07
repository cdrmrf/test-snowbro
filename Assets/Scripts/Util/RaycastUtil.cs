using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RaycastUtil
{

    public static GameObject FindByTag(Vector2 position, float horizontal, string tag)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, horizontal >= 0 ? Vector2.right : Vector2.left, 0.05f);
        GameObject obj = null;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == tag)
            {
                obj = hit.collider.gameObject;
                break;
            }
        }
        return obj;
    }

    public static GameObject FindByTag(Vector2 position, Vector2 direction, float distance, string tag)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, distance);
        GameObject obj = null;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == tag)
            {
                obj = hit.collider.gameObject;
                break;
            }
        }
        return obj;
    }

    public static GameObject FindSnowBall(Vector2 position, float horizontal)
    {
        GameObject enemy = FindByTag(position, horizontal, TagManage.ENEMY);
        if(enemy == null)
        {
            return null;
        }
        //  找到了敌人，确认敌人的状态
        var enemyState = enemy.GetComponent<EnemyVariable>();
        bool foundSnowBall = enemyState.CanRoll();
        return foundSnowBall && enemy ? enemy : null;
    }

}
