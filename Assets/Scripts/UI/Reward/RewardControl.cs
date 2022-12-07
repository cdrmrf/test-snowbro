using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardControl : MonoBehaviour
{
    public enum RewardType
    {
        SCORE,
        BUFF
    }

    public enum BuffType
    {
        NONE,
        RED,
        BLUE,
        YELLOW,
        GREEN
    }

    public RewardType type;

    public BuffType buffType = BuffType.NONE;

    public int score;
    public GameObject scoreIconPrefab;

    private void Start()
    {
        //  5秒不吃就消失
        Destroy(gameObject, 5f);
    }

    public void AddScore()
    {
        Global.SCORE += this.score;
        if (score > 0 && scoreIconPrefab != null)
        {
            var text = Instantiate(scoreIconPrefab, transform.parent);
            text.transform.position = transform.position;
        }
    }

}
