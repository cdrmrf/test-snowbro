using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///     物品的奖励积分，玩家吃到奖励后显示这个奖品的分数
public class PointControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * 0.5f * Time.deltaTime);
    }
}
