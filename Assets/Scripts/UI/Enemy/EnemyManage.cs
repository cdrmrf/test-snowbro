using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class EnemyManage : MonoBehaviour
{
    public GameObject enemy;
    private int counter = 1;

    //  发射敌人
    public void CreateEnemy(Transform bossTransform, GameObject targetFloor, UnityAction afterEnemyDie)
    {
        //  创建敌人，
        var e = GetEnemy();
        //  小怪出生的位置
        //  x坐标 是和敌人一样
        //  y坐标 比左侧的地板稍微高一点，要不然飞的时候会撞上地板边缘
        var epos = e.transform.position;
        epos.x = bossTransform.position.x;
        epos.y = targetFloor.transform.position.y + 0.1f;
        e.transform.position = epos;

        EnemyVariable enemyVars = e.GetComponent<EnemyVariable>();
        //  飞行掉落的坐标，小怪飞到这个 x坐标，就掉落开始移动
        Vector2 enemyFallPosition = Vector2.zero;
        enemyFallPosition.x = targetFloor.transform.position.x + Random.Range(-0.15f, 0.15f);
        enemyFallPosition.y = targetFloor.transform.position.y + 0.1f;
        enemyVars.bornPosition = enemyFallPosition;
        //  小怪死后的回调
        enemyVars.dieCallBack = () =>
        {
            Debug.Log($"{e.name} died");
            if (afterEnemyDie != null)
            {
                afterEnemyDie.Invoke();
            }
        };
        //  场上小怪数量 +1
        //  标记地板上有小怪了
        targetFloor.GetComponent<FloorControl>().CreateEnemy();
        e.SetActive(true);
    }

    //  从 enemyPool 中取一个 小怪，如果没有就创建一个
    private GameObject GetEnemy()
    {
        var e = Instantiate(enemy, transform.parent);
        e.name = $"Enemy_{counter++}";
        e.SetActive(false);
        return e;
    }

}
