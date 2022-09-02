using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class PlayerMove : MonoBehaviour
{
    public SensorController sc;

    public bool left;
    public bool right;

    float leftZ = 0;
    float rightZ = 0;
 
    void Update ()
    {
        sc = GameObject.Find("Sensor").GetComponent<SensorController>();
        leftZ = sc.leftPositionZ;
        rightZ = sc.rightPositionZ;
        if(left){
            transform.position = new Vector3(-21, 0, (leftZ - 540) /60); 

//Debug.Log("LLL");
        }
        if(right){
            transform.position = new Vector3(21, 0, (rightZ - 540 ) /60);
//Debug.Log("RRR");
        }
        
//Debug.Log("leftZ="+leftZ +" rightZ="+rightZ);
    }
}
