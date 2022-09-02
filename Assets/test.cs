
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    bool isMouseLeftPressed;
    bool isMouseRightPressed;

    Vector3 nowMousePos;
    Vector3 lastMousePos;

    public SensorController sensor;
    

    // Start is called before the first frame update
    void Awake()
    {
        isMouseLeftPressed = false;
        isMouseRightPressed = false;

    }

    // Update is called once per frame
    void Update()
    {
        lastMousePos = nowMousePos ;
        nowMousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            isMouseLeftPressed = true;

            Debug.Log("Mouse Left Pressed");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(ray, out hit, 100.0f);

            if (hit.collider){
                Debug.Log("Hit!!");
                gameObject.GetComponent<Renderer>().material.color = Color.red;
            }

        }else if(Input.GetMouseButtonDown(1)){
            isMouseRightPressed = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseLeftPressed = false;

            Debug.Log("Mouse Left Released");

        }
        else if (Input.GetMouseButtonUp(1))
        {
            isMouseRightPressed = false;
        }



        if (isMouseLeftPressed)
        {
            Ray rayLast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitLast;

            Physics.Raycast(rayLast, out hitLast, 100.0f);

            Ray rayNow = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitNow;

            Physics.Raycast(rayNow, out hitNow, 100.0f);

            if (hitNow.collider && hitNow.collider.gameObject == hitLast.collider.gameObject)
            {
                Debug.Log("Drag!!");
            }
        }


        List<Vector3>vlist = new List<Vector3>();
        sensor.getPointList(ref vlist);

        foreach(Vector3 v in vlist){
            Ray rayLast = Camera.main.ScreenPointToRay(v);
            RaycastHit hitLast;

            Physics.Raycast(rayLast, out hitLast, 100.0f);
        }
    }
}
