using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManage : MonoBehaviour
{

    public static RewardManage instance;

    public List<GameObject> scoreRewardList;
    public List<GameObject> buffRewardList;

    private void Awake()
    {
        instance = this;
    }

    //  随机奖励
    public void RandomReward(Transform postion)
    {
        var rand = Random.Range(1, 100);
        //if (rand < 30)
        //{
        //    return;
        //}
        rand = Random.Range(1, 100);
        if (rand < 20)
        {
            RandomScoreReward(postion);
        }
        else
        {
            RandomBuffReward(postion);
        }
    }

    //  随机糖果奖励
    private void RandomScoreReward(Transform postion)
    {
        GameObject obj = scoreRewardList[Random.Range(0, scoreRewardList.Count)];
        var s = Instantiate(obj, transform.parent);
        var pos = s.transform.position;
        pos.x = postion.position.x;
        pos.y = postion.position.y - 0.03f;
        s.transform.position = pos;
    }

    //  随机 BUFF 奖励
    private void RandomBuffReward(Transform postion)
    {
        GameObject obj = buffRewardList[Random.Range(0, scoreRewardList.Count)];
        var s = Instantiate(obj, transform.parent);
        var pos = s.transform.position;
        pos.x = postion.position.x;
        pos.y = postion.position.y - 0.03f;
        s.transform.position = pos;
    }

}
