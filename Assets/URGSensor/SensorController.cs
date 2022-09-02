using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
/// <summary>
/// お絵描き
/// センサーの位置データはスクリーン座標からワールド座標に変換して使う
/// 
/// DontDestroyOnLoadでシーン遷移後もTaskが使える
/// 上記関数が無くてもタスクは動き続けてるっぽいので
/// DontDestroy~をちゃんと使ってアプリが終わったらDestroyする
/// そうしないとアプリが終わってもタスクが終わらない
/// それとアプリが終わったらループも終わらせる処理を実装
/// 
/// </summary>

public class SensorController : MonoBehaviour
{
    private List<Vector3> pointObjects = new List<Vector3>();
    
    static Material lineMaterial;

    public URGSensor urgSensor;

    //private float leftPositionZ = 0;
    //private float rightPositionZ = 0;


    public float leftPositionZ = 0;
    public float rightPositionZ = 0;



    Color bgColor = Color.white;
    Color lineColor = Color.yellow;
    Color quadColor = Color.white;
    private float time;
    public float sensorRotation;
    public Vector2 sensorPosition;
    public float sensorScale = 1.0f;

    public bool isShowLine = true;
    public bool invert = true;
    public float pointThreshold = 50.0f;
    public float objectRadius = 50.0f;
    public string ipAdress;
    public Camera targetCamera;

    public bool editable;

    public bool DebugBool;

    public List<Vector2> objPoints = new List<Vector2>();
    private Task task;

    System.Object lockObj = new System.Object();

    private long[] distanceArray = new long[1081];
    private Vector3[] pointChain = new Vector3[1081];


    public bool isInit;

    private bool loop;
    private string filePath;

    private bool mousePressed;
    private Vector3 lastPos;

    private void Awake()
    {
        isInit = false;

        urgSensor = new URGSensor(ipAdress);
        task = null;

        /*sensorRotation = 0.0f;
        sensorPosition = new Vector2(0.0f, 0.0f);
        sensorScale = 1.0f;*/

        isInit = true;
        loop = true;

        filePath = Application.dataPath + this.name + ".txt";

        load();

        editable = false;


        //ディスプレイを初期化

    }
    void Start()
    {
        
        GL.Viewport(new UnityEngine.Rect(0, 0, Screen.width, Screen.height));

        time = Time.realtimeSinceStartup;



        task = new Task(() => {
            while (loop) {
                urgSensor.read_data();

                int count = 0;
                float rad = sensorRotation - (float)((45.0f / 180.0f) * Math.PI);
                distanceArray[0] = urgSensor.distances[0];
                long lastDistance = distanceArray[0];

                /////ループ開始
                lock (lockObj)
                {
                    //連続している点をみつける
                    List<Vector3> points = new List<Vector3>();
                    for (int i = 1; i < 1081; i++)
                    {
                        //データをコピーする　スレッドセーフ用　データはmm単位
                        distanceArray[i] = urgSensor.distances[i];
                        //反応がないレーザーははじく
                        //直前のレーザーと近いか判定
                        if (10 < distanceArray[i] && distanceArray[i] < 9800 && Mathf.Abs(lastDistance - distanceArray[i]) < pointThreshold) {
                            //リストに追加
                            
                            if(count == 0 && i > 0)
                            {
                                pointChain[count] = new Vector3(distanceArray[i - 1] * Mathf.Cos(rad - 0.00436f)
                                                                , distanceArray[i - 1] * Mathf.Sin(rad - 0.00436f)
                                                                , distanceArray[i - 1]);//x,y,length
                                count++;
                            }
                            
                            pointChain[count] = new Vector3(distanceArray[i] * Mathf.Cos(rad)
                                                            ,distanceArray[i] * Mathf.Sin(rad)
                                                            , distanceArray[i]);//x,y,length
                            count++;
                        }
                        else
                        {
                            if (2 < count)
                            {
                                //直径を計算　スケールを掛ける前のオリジナルの数値
                                float diameter = Vector2.Distance(new Vector2(pointChain[0].x, pointChain[0].y)
                                                                , new Vector2(pointChain[count - 1].x, pointChain[count - 1].y));

                                if (diameter < 500)
                                {
                                    //位置と長さの平均
                                    float sumx = 0;
                                    float sumy = 0;
                                    float suml = 0;

                                    for (int k = 0; k < count; k++)
                                    {
                                        sumx += pointChain[k].x;
                                        sumy += pointChain[k].y;
                                        suml += pointChain[k].z;
                                    }
                                    sumx /= count;
                                    sumy /= count;
                                    suml /= count;

                                    //位置のオフセットとスケールを計算してリストに追加
                                    points.Add(new Vector3((sumx * sensorScale * (invert ? -1.0f : 1.0f) + sensorPosition.x)
                                                                ,sumy * sensorScale + sensorPosition.y
                                                                , diameter * sensorScale));
                                }
                            }
                            count = 0;
                        }

                        lastDistance = distanceArray[i];
                        rad += 0.00436f;
                    }
                    //近い点群を個体として計算
                    pointObjects.Clear();

                    if (points.Count > 0)
                    {
                        Vector3 vsum = points[0];
                        Vector3 vsave = points[0];
                        int c = 1;

                        for (int i = 1; i < points.Count; i++)
                        {
                            Vector3 v = points[i];
                            if (Vector3.Distance(vsave, v) < objectRadius)
                            {
                                vsum += v;
                                c++;
                            }
                            else
                            {
                                Vector3 p = vsum / c;
                                if(20.0f < p.y && p.y < 1060.0f)pointObjects.Add(p);
                                vsum = v;
                                c = 1;
                                
                            }
                            vsave = v;

                        }
                        Vector3 pp = vsum / c;
                        if (20.0f < pp.y && pp.y < 1060.0f) pointObjects.Add(pp);
                    }
                    else
                    {   

                        Debug.Log("no data");
                    }
                }
            }
        });
        task.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            editable = !editable;
            isShowLine = editable;

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DebugBool = !DebugBool;

        }


        if (editable && isShowLine)
        {
            if (Input.GetMouseButtonDown(1))
            {
                mousePressed = true;
                lastPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(1))
            {
                mousePressed = false;
            }

            if (mousePressed)
            {
                Vector3 newPos = Input.mousePosition;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    sensorRotation += (newPos.x - lastPos.x) / 1000;
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    sensorScale += (newPos.x - lastPos.x) / 1000;
                }
                else
                {
                    Vector3 vec = newPos - lastPos;
                    sensorPosition += new Vector2(vec.x, vec.y);
                }
                lastPos = newPos;

            }
        }
    }

    void OnRenderObject()
    {
        
    float leftZ = 0;/////
    float rightZ = 0;/////

    int lefttCnt = 0;
    int rightCnt = 0;


        if (urgSensor == null) return;

        if (Camera.current == targetCamera)
        {
            if (isShowLine)
            {
                CreateLineMaterial();
                lineMaterial.SetPass(0);

                float rad = sensorRotation - (float)((45.0f / 180.0f) * Math.PI); ;

                lock (lockObj)
                {
                    GL.PushMatrix();
                    GL.LoadPixelMatrix();


                    for (int j = 0; j < pointObjects.Count; j++)
                    {
                        Vector3 v = pointObjects[j];
                        float s = v.z;
                        if (s > 100.0f) s = 200.0f;
                        else if (s < 50.0f) s = 50.0f;

                        GL.Begin(GL.QUADS);

                        GL.Color(quadColor);

                        Debug.Log("Y="+ v.y+"   X="+v.x );

                        if(v.y <= 2000 && v.x <= 1800 && v.y >= 0 && v.x >= 100 ){ //ここ変えたら検知範囲変更

                            if(DebugBool){
                                GL.Vertex3(-s / 5 + v.x, -s / 5 + v.y, 0); //四角描画
                                GL.Vertex3(s / 5 + v.x, -s / 5 + v.y, 0);
                                GL.Vertex3(s / 5 + v.x, s / 5 + v.y, 0);
                                GL.Vertex3(-s / 5 + v.x, s / 5 + v.y, 0);
                                //Debug.Log("Y="+ v.y );
                            }
                            

                            GL.End();

                                //Debug.Log("X="+v.x +" Y="+v.y+" Z="+v.z);
                        
                            if(v.x < 1000)//left
                            {
                                leftZ += v.y;
                                lefttCnt++;
                            }else if(v.x > 1000)//right
                            {
                                rightZ += v.y;
                                rightCnt++;
                            }
                        }


                    }
                    for (int i = 0; i < distanceArray.Count(); i += 1)
                    {
                        long d = distanceArray[i];

                        GL.Begin(GL.LINE_STRIP);

                        GL.Color(lineColor);
                        if(DebugBool){
                            GL.Vertex(new Vector3(sensorPosition.x, sensorPosition.y, 0)); //LINEの描写
                            GL.Vertex(new Vector3((float)Math.Cos(rad) * d * sensorScale * (invert ? -1.0f : 1.0f) + sensorPosition.x
                                            , (float)Math.Sin(rad) * d * sensorScale + sensorPosition.y
                                            , 0));

                        }

                        GL.End();

                        rad += 0.00436f;
                    }

                    GL.PopMatrix();
                
                PositionSet(leftZ / lefttCnt,rightZ / rightCnt); ///////////////

                }
            }
        }
    }

    public void getPointList(ref List<Vector3> _list)
    {
        lock (lockObj)
        {
            _list = new List<Vector3>(pointObjects);
        }
    }
 
    ~SensorController()
    {
        urgSensor.closeStream();
        urgSensor = null;
    }

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void save()
    {
        List<string> data = new List<string>();

        data.Add(sensorPosition.x.ToString());
        data.Add(sensorPosition.y.ToString());
        data.Add(sensorRotation.ToString());
        data.Add(sensorScale.ToString());
        data.Add(pointThreshold.ToString());
        data.Add(objectRadius.ToString());

        File.WriteAllLines(filePath, data);
    }

    public void load()
    {
        if (File.Exists(filePath))
        {
            string[] data = File.ReadAllLines(filePath);

            sensorPosition = new Vector2(float.Parse( data[0]),float.Parse( data[1]));
            sensorRotation = float.Parse(data[2]);
            sensorScale = float.Parse(data[3]);
            pointThreshold = float.Parse(data[4]);
            objectRadius = float.Parse(data[5]);

            if(sensorScale <= 0)
            {
                sensorScale = 1.0f;
            }
        }
    }


    private void OnApplicationQuit()
    {
        Debug.Log("Destroy");
        loop = false;
        Destroy(gameObject);
    }

    public void PositionSet(float leftZ,float rightZ)
    {
        this.leftPositionZ = leftZ;   //値入力用
        this.rightPositionZ = rightZ; //値入力用
//Debug.Log("thisX="+leftZ +" thisY="+rightZ);
//ebug.Log("thisX="+this.leftPositionZ +" thisY="+this.rightPositionZ);
    }



    /*public float leftPositionZ{
        get{ return this.bulletCount; }  //取得用
        private set{ this.bulletCount = value; }　//値入力用
    }*/

}


