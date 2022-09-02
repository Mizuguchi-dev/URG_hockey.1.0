using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitToriga : MonoBehaviour
{
  // 当たった時に呼ばれる関数
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit"); // ログを表示する
        Debug.Log(collision.gameObject.name);

        if(collision.gameObject.name == "GoalR")
        {
            Debug.Log("GOAL");
        }
    }
}

