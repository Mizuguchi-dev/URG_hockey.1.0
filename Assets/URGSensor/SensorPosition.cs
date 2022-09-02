using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorPosition : MonoBehaviour
{
    public SensorController sc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float leftZ;
        float rightZ;
        leftZ = sc.leftPositionZ;
        rightZ = sc.rightPositionZ;
        Debug.Log(leftZ + " " + rightZ ); 
        /*mouse = Input.mousePosition;
        target = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 28));
        transform.LookAt(target);
        transform.position += transform.forward * speed;*/
    }
}
