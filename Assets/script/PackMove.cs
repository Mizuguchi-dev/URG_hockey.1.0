using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackMove  : MonoBehaviour {
    private float speed = 2.5f;//弾速度
    private float moveX = 0f;
    private float moveY = 0f;
    private float moveZ = 0f;
    private Vector3 lastVelocity;//速度ベクトル
    private Rigidbody rb;//Rigidbody用
    private int time = 0;


    //public GoalCNT gcnt;
    public GoalCNT gcntL;
    public GoalCNT gcntR;

    void Start () {
        rb = GetComponent<Rigidbody>();
        SetUp();
        gcntL = GameObject.Find("GoalL").GetComponent<GoalCNT>();
        gcntR = GameObject.Find("GoalR").GetComponent<GoalCNT>();
    }

    void SetUp(){
        //this.speed = 2.5f;
        transform.position = new Vector3(0.0f,0.2f,0.0f);
        moveX = Random.Range(-12.0f, 12.0f) * speed;
        if(moveX <= 9.0f &&  moveX >= -9.0f){
            moveX = 9.0f;
            if(Random.Range(0,2) == 1){
                moveX = moveX * -1.0f;
            }
        }
        Debug.Log(moveX);
        moveZ = Random.Range(7.0f, 8.0f) * speed;
        rb.velocity = new Vector3(moveX, 0.2f, moveZ);//初期ベクトル

    }
	
	void Update () {
        /*time++;
        if (time > 1500)//1500フレーム
        {
            this.speed += 0.2f;
        }*/
    }

    void FixedUpdate()
    {
        this.lastVelocity = this.rb.velocity;//Rigidbodyを使用した移動用

    }


    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.name == "wall1")//壁と当たった時
        {
            Vector3 refrectVec = Vector3.Reflect(this.lastVelocity, coll.contacts[0].normal);//反射ベクトル計算
            this.rb.velocity = refrectVec;
        }
        if (coll.gameObject.name == "wall2")//壁と当たった時
        {
            Vector3 refrectVec = Vector3.Reflect(this.lastVelocity, coll.contacts[0].normal);//反射ベクトル計算
            this.rb.velocity = refrectVec;
        }
        if (coll.gameObject.tag == "Player")//プレイヤーと当たった時
        {
            Vector3 refrectVec = Vector3.Reflect(this.lastVelocity, coll.contacts[0].normal);//反射ベクトル計算
            this.rb.velocity = refrectVec;
        }
        if (coll.gameObject.name == "GoalL")//ゴールと当たった時
        {
            gcntL.PointCount = gcntL.PointCount + 1;
            gcntL.PointUI();
            Debug.Log(gcntL.PointCount);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Reset();
            //Invoke("Reset", 4);
            //Destroy(gameObject);
        }
        if (coll.gameObject.name == "GoalR")//ゴールと当たった時
        {
            gcntR.PointCount = gcntR.PointCount + 1;
            gcntR.PointUI();
            Debug.Log(gcntR.PointCount);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Reset();
            //Invoke("Reset", 4);
            //Destroy(gameObject);
        }
        /*if (coll.gameObject.tag == "tagggg")//破壊出来る壁に当たった時
        {
            Destroy(coll.gameObject);//壁削除
            Vector3 refrectVec = Vector3.Reflect(this.lastVelocity, coll.contacts[0].normal);//反射ベクトル計算
            this.rb.velocity = refrectVec;
        }*/
    }

    void Reset(){
        SetUp();
    }
}


/*public class PackMove : MonoBehaviour
{
    public string objName;
 
    void Update()
    {
        float dx = Input.GetAxis("Horizontal") * Time.deltaTime * 6;
        float dz = Input.GetAxis("Vertical") * Time.deltaTime * 6;
        transform.position = new Vector3
        (
            transform.position.x + dx, 0.5, transform.position.z + dz
        );
    }
 
     void OnCollisionEnter(Collision collision){
        objName = collision.gameObject.name;
        Debug.Log(objName);
    }
}*/