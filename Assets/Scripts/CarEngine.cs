using Assets.Scripts;
using Assets.Scripts.Dijkstra;
using RoadArchitect;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System;
using UnityEngine.UIElements;

//https://blog.csdn.net/qq_68117303/article/details/133011345
//https://docs.unity3d.com/cn/current/Manual/class-WheelCollider.html
public class CarEngine : MonoBehaviour
{
    //汽车的最大转向角度
    public float maxSteerAngle = 90;

    //获取路径
    private List<Transform> nodes;
    //路径点索引值
    private int currentIndex = 0;


    ////车轮碰撞器
    //public WheelCollider LF;  //左前轮
    //public WheelCollider RF;  //右前轮

    ////轮胎碰撞器
    //public WheelCollider LB; //左后
    //public WheelCollider RB; //右后
    GameObject carHead;

    private float targetSteerAngle; //汽车轮胎实际的转角

    //车轮动力部分
    public float maxMotorTorque = 600f; //车轮最大动力   
    public float maxSpeed = 60f; //最大车速
    //public float currentSpeed;   //汽车当前车速

    //汽车稳定性的提升
    private Rigidbody rb;
    public UnityEngine.Vector3 centerOfMass = new UnityEngine.Vector3(0, -1, 0);

    //汽车动力
    public float maxBrakeTorque = 600f;  //刹车的制动力

    //刹车部分的设置
    public bool isBraking;
    public Texture2D textureNormal;     //不刹车时车灯的贴图
    public Texture2D textureBreaking;   //刹车时车灯的贴图
    public Renderer carRender;

    RouteNet routeNet;


    public String[] Plan(Coord startPosition, Coord endPosition)
    {
        routeNet = HarborApp.graph.RouteNet.Clone();
        Node startNode = routeNet.InsertNearstNode("startNodeId", startPosition);
        Node endNode = routeNet.InsertNearstNode("endNodeId", endPosition);
        RoutePlanner planner = new RoutePlanner();
        RoutePlanResult routePlanResult = planner.Plan(routeNet.NodeList, startNode.ID, endNode.ID, null);
        return routePlanResult.ResultNodes;
    }

    // Start is called before the first frame update
    void Start()
    {
        //获取要装卸的集装箱
        GameObject container = GameObject.Find("Port-Container_SHIP1/Port-container_38");
        //集装箱位置
        Transform containerPos = container.transform;
        //获取集卡的位置
        GameObject truck = GameObject.Find("HG0702");
        Transform truckPos = truck.transform;

        String[] resultNodes = HarborApp.Plan(new Coord(truckPos.position.x, truckPos.position.y, truckPos.position.z), new Coord(containerPos.position.x, containerPos.position.y, containerPos.position.z));
        //routeNet.m_nodeList.Clear();
        Debug.Log(resultNodes);

        //Transform[] pathTransfroms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < resultNodes.Length; i++)
        {
            Coord coord = routeNet.GetNodeById(resultNodes[i]).Coord;
            Transform newTransform = new GameObject().transform;
            newTransform.position = new UnityEngine.Vector3(coord.X, coord.Y, coord.Z);
            nodes.Add(transform);
        }
        rb = GetComponent<Rigidbody>();
        //改变车辆的重心,汽车稳定性的提升
        rb.centerOfMass = centerOfMass;
    }

    private void FixUpdate()
    {
        ApplySteer();
        Drive();
        CheckNextWaypointDistance();
        Breaking();
    }

    private void ApplySteer()
    {
        //根据车的位置和路径点的位置坐标计算出相对向量
        UnityEngine.Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentIndex].position);
        //计算轮胎的实际转角
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        this.targetSteerAngle = newSteer;
        ////将实际转角运用到左前轮和右前轮的转角
        //LF.steerAngle = targetSteerAngle;
        //RF.steerAngle = targetSteerAngle;
        this.carHead.transform.Rotate(UnityEngine.Vector3.down * Time.deltaTime * targetSteerAngle);
    }

    private void Drive()
    {
        ////计算车速
        //currentSpeed = 2 * Mathf.PI * LF.radius * LF.rpm * 60 / 1000;
        ////如果车速小于最大速度，那么给予车轮动力
        ////修改一下汽车行驶的条件
        //if (currentSpeed < maxSpeed && !isBraking)
        //{
        //    LF.motorTorque = maxMotorTorque;
        //    RF.motorTorque = maxMotorTorque;
        //}
        //else
        //{
        //    LF.motorTorque = 0;
        //    RF.motorTorque = 0;
        //}
    }
    private void CheckNextWaypointDistance()
    {
        //判断汽车当前的距离和路径点之间的距离
        if (UnityEngine.Vector3.Distance(new UnityEngine.Vector3(transform.position.x, 0, transform.position.z),
                           new UnityEngine.Vector3(nodes[currentIndex].position.x, 0,
                           nodes[currentIndex].position.z)) < 0.5f)
        {
            //如果已经到达了最后一个路径点，那么将索引值置0，绕圈
            if (currentIndex == nodes.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
        }
    }

    private void Breaking()
    {
        //if (isBraking)
        //{
        //    carRender.material.mainTexture = textureBreaking;
        //    //后轮刹车
        //    RB.brakeTorque = maxBrakeTorque;
        //    LB.brakeTorque = maxBrakeTorque;
        //}
        //else
        //{
        //    carRender.material.mainTexture = textureNormal;
        //    RB.brakeTorque = 0;
        //    LB.brakeTorque = 0;
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
