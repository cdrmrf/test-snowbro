using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    public enum GameStatusEnum
    {
        PLAYING,
        WIN,
        GAME_OVER
    }

    public static GameStatusEnum GAME_STATUS = GameStatusEnum.PLAYING;

    public static int BOSS_HP = GameSetting.Enemy.BOSS_HP;

    public static int PLAYER_LIFE = GameSetting.Player.PLAYER_LIFE;

    public static int SCORE = 0;

    public static bool IsPlaying()
    {
        return GAME_STATUS == GameStatusEnum.PLAYING;
    }


}