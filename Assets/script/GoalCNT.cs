using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalCNT  : MonoBehaviour {


    public int cnt = 0;

    public TextMeshProUGUI point;

    public GoalCNT gcnt;

    public int PointCount{
        get{ return this.cnt; }  //取得用
        set{ this.cnt = value; } //値入力用
    }

    public void PointUI(){
        string  s = cnt.ToString();
        point.text = s;
    }
    

    
}


