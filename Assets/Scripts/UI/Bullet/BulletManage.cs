using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManage : MonoBehaviour
{

    private int counter = 1;

    //  普通子弹
    public GameObject bullet;
    //  加粗子弹
    public GameObject boldBullet;
    //  子弹 cd
    private float cd = 0f;

    private List<GameObject> bulletPools = new List<GameObject>();
    private List<GameObject> boldBulletPools = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        cd += Time.deltaTime;
    }

    //  发射子弹
    public bool Fire(bool enhanceBulletSpeed, bool enhanceBulletDistance, bool enhanceBulletSize)
    {
        //  发射子弹
        if(cd < GameSetting.Player.FIRE_CD)
        {
            return false;
        }
        cd = 0f;
        //  确定角色的方向
        GameObject player = GameObject.FindWithTag("Player");
        bool isFlip = player.GetComponent<SpriteRenderer>().flipX;
        //  创建子弹
        var bullet = GetBullet(enhanceBulletSize);
        bullet.GetComponent<SpriteRenderer>().flipX = isFlip;
        //  子弹位置
        var pos = bullet.transform.localPosition;
        //  往前飞 / 往后飞
        if (isFlip)
        {
            pos.x = player.transform.position.x + 0.1f;
        } else
        {
            pos.x = player.transform.position.x - 0.1f;
        }
        pos.y = player.transform.position.y + 0.01f;
        bullet.transform.localPosition = pos;
        //  设置子弹的飞行速度和飞行时间
        var bulletControl = bullet.GetComponent<BulletControl>();
        bulletControl.speed = enhanceBulletSpeed ? 0.9f : 0.7f;
        bulletControl.destroyTime = enhanceBulletDistance ? 1.5f : 0.75f;
        bulletControl.ResetAndShow();
        return true;
    }

    private GameObject GetBullet(bool enhanceBulletSize)
    {
        List<GameObject> list = enhanceBulletSize ? boldBulletPools : bulletPools;
        foreach (GameObject obj in list)
        {
            //  未激活
            if (!obj.gameObject.activeSelf)
            {
                return obj;
            }
        }
        //  创建新子弹
        GameObject bul = null;
        if (enhanceBulletSize)
        {
            bul = Instantiate(this.boldBullet, transform.parent);
        } else
        {
            bul = Instantiate(this.bullet, transform.parent);
        }
        bul.name = $"bullet_{counter++}";
        list.Add(bul);
        return bul;
    }

}
