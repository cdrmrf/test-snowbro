using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameInfoControl : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text life;
    public TMP_Text bossHP;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        score.text = string.Format("{0:D8}", Global.SCORE);
        life.text = Global.PLAYER_LIFE + "";
        bossHP.text = Global.BOSS_HP + "";
    }
}
