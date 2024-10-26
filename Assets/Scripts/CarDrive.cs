using Assets.Scripts;
using Assets.Scripts.Dijkstra;
using RoadArchitect;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.AI;
using TMPro;
using UnityEngine.EventSystems;

//https://blog.csdn.net/qq_68117303/article/details/133011345
//https://docs.unity3d.com/cn/current/Manual/class-WheelCollider.html

// 定义一个带参数的事件委托类型
public delegate void ArrivedEventHandler(int param);
public class CarDrive : MonoBehaviour
{
    //定义带参数的事件
    public event ArrivedEventHandler ArrivedChanged;

    //获取路径
    private List<UnityEngine.Vector3> nodes;
    //路径点索引值
    private int currentIndex = 0;


    public float speed = 10;

    //汽车稳定性的提升
    //private Rigidbody rb;
    public UnityEngine.Vector3 centerOfMass = new UnityEngine.Vector3(0, -1, 0);

    RouteNet routeNet;
    UnityEngine.Vector3 _targetPosition;

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

        String[] resultNodes = this.Plan(new Coord(this.transform.position.x, this.transform.position.y, this.transform.position.z), new Coord(containerPos.position.x, containerPos.position.y, containerPos.position.z - 6.5f));
        Debug.Log(resultNodes);
        nodes = new List<UnityEngine.Vector3>();
        for (int i = 0; i < resultNodes.Length; i++)
        {
            Coord coord = routeNet.GetNodeById(resultNodes[i]).Coord;
            nodes.Add(new UnityEngine.Vector3(coord.X, 2.2f, coord.Z));
        }
        _targetPosition = nodes[0];
    }

    /// <summary>
    /// 将车辆调整为靠右行驶
    /// </summary>
    /// <param name="vangle"></param>
    /// <param name="position"></param>
    UnityEngine.Vector3 adjustNextCoord(float vangle, UnityEngine.Vector3 position)
    {
        float adjust = 2.5f;
        if (vangle > 45 && vangle <= 135)
        {
            position.x += adjust;
        }
        else if (vangle > 135 && vangle <= 225)
        {
            position.z += adjust;
        }
        else if (vangle > 225 && vangle < 315)
        {
            position.x -= adjust;
        }
        else
        {
            position.z -= adjust;
        }
        return position;
    }


    // Update is called once per frame
    void Update()
    {
        if (currentIndex < 0) { return; }
        //移动位置
        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);

        if (UnityEngine.Vector3.Distance(new UnityEngine.Vector3(this.transform.position.x, 2.2f, this.transform.position.z), _targetPosition)
                            < 0.5f)
        {
            //如果已经到达了最后一个路径点，那么将索引值置0，绕圈
            if (currentIndex == nodes.Count - 1)
            {
                //触发事件
                ArrivedChanged?.Invoke(0);
                currentIndex = -1;
                return;
            }
            else
            {
                currentIndex++;
            }
            _targetPosition = nodes[currentIndex];
            // 计算新的朝向
            float vangle = Utils.getAngleBetweenVectorAndXAxis(this.transform.position, _targetPosition);
            //将车辆调整为靠右行驶
            this.transform.position = adjustNextCoord(vangle, nodes[currentIndex - 1]);
            _targetPosition = adjustNextCoord(vangle, _targetPosition);
            //调整坐标后，重新计算朝向
            vangle = Utils.getAngleBetweenVectorAndXAxis(this.transform.position, _targetPosition);
            vangle = 360 - vangle + 180;
            Vector3 eulerAngle = new Vector3(0, vangle, 0);//欧拉角
            transform.rotation = Quaternion.Euler(eulerAngle);
        }
    }
}
