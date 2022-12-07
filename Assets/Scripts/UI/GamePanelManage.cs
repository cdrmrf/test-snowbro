using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanelManage : MonoBehaviour
{
    public GameObject win;
    public GameObject gameOver;

    private void Awake()
    {
        EventCenter.Instance.AddEventListener2(EventEnum.GAME_STATUS_CHANGE, OnStatusChange);
    }

    private void OnStatusChange()
    {
        if(Global.GAME_STATUS == Global.GameStatusEnum.WIN)
        {
            win.SetActive(true);
        } else if(Global.GAME_STATUS == Global.GameStatusEnum.GAME_OVER)
        {
            gameOver.SetActive(true);
        }
    }

}
