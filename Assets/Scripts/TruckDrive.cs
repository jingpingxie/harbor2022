using Assets.Scripts;
using Assets.Scripts.Dijkstra;
using System.Collections.Generic;
using UnityEngine;
using System;

//https://blog.csdn.net/qq_68117303/article/details/133011345
//https://docs.unity3d.com/cn/current/Manual/class-WheelCollider.html

// 定义一个带参数的事件委托类型
public delegate void LoadContainerFromShipToTruckEventHandler(TruckDrive carDrive, GameObject truck, GameObject loadedContainer, int param);
public delegate void LoadContainerFromTruckToYardEventHandler(TruckDrive carDrive, GameObject truck, GameObject loadedContainer, int param);
public class TruckDrive : MonoBehaviour
{
    //定义带参数的事件
    public event LoadContainerFromShipToTruckEventHandler LoadContainerFromShipToTruckNotify;
    public event LoadContainerFromTruckToYardEventHandler LoadContainerFromTruckToYardNotify;

    //获取路径
    private List<UnityEngine.Vector3> _nodes;
    //路径点索引值
    private int _currentIndex = 0;

    public float Speed = 10;

    //汽车稳定性的提升
    //private Rigidbody rb;
    public UnityEngine.Vector3 centerOfMass = new UnityEngine.Vector3(0, -1, 0);

    RouteNet _routeNet;
    UnityEngine.Vector3 _targetPosition;
    GameObject _currentContainer;
    const float CarHeight = 2.2f;
    String _ignoreNodeId = null;

    TruckActionState _truckActionState;

    public String[] Plan(Coord startPosition, Coord endPosition, string ignoreNodeID)
    {
        _routeNet = HarborApp.graph.RouteNet.Clone();
        Node startNode = _routeNet.InsertNearstNode("startNodeId", startPosition);
        Node endNode = _routeNet.InsertNearstNode("endNodeId", endPosition);
        RoutePlanner planner = new RoutePlanner();
        RoutePlanResult routePlanResult = planner.Plan(_routeNet.NodeList, startNode.ID, endNode.ID, ignoreNodeID);
        return routePlanResult.ResultNodes;
    }

    // Start is called before the first frame update
    void Start()
    {
        //获取要装卸的集装箱
        _currentContainer = GameObject.Find("Port-Container_SHIP1/Port-container_38");
        //集装箱位置
        Transform containerPos = _currentContainer.transform;

        String[] resultNodes = this.Plan(new Coord(this.transform.position.x, this.transform.position.y, this.transform.position.z),
            new Coord(containerPos.position.x, containerPos.position.y, containerPos.position.z - 6.5f), null);
        _nodes = new List<UnityEngine.Vector3>();
        for (int i = 0; i < resultNodes.Length; i++)
        {
            Coord coord = _routeNet.GetNodeById(resultNodes[i]).Coord;
            _nodes.Add(new UnityEngine.Vector3(coord.X, CarHeight, coord.Z));
        }
        _ignoreNodeId = resultNodes[resultNodes.Length - 2];
        _targetPosition = _nodes[0];
        _truckActionState = TruckActionState.ToShip;
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

    public void SetContainerLoadEnd(GameObject container)
    {
        _currentContainer = container;
        //计划运送集装箱到堆场的路径
        _nodes.Clear();
        String[] resultNodes = Plan(new Coord(_targetPosition.x, _targetPosition.y, _targetPosition.z), new Coord(330, CarHeight, 10), _ignoreNodeId);
        for (int i = 0; i < resultNodes.Length; i++)
        {
            Coord coord = _routeNet.GetNodeById(resultNodes[i]).Coord;
            _nodes.Add(new UnityEngine.Vector3(coord.X, CarHeight, coord.Z));
        }
        _targetPosition = _nodes[0];
        _currentIndex = 0;
        _truckActionState = TruckActionState.FromShipToYard;
    }


    // Update is called once per frame
    void Update()
    {
        if (_currentIndex < 0) { return; }
        //移动位置
        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, _targetPosition, Speed * Time.deltaTime);

        if (UnityEngine.Vector3.Distance(new UnityEngine.Vector3(this.transform.position.x, CarHeight, this.transform.position.z), _targetPosition)
                            < 0.5f)
        {
            //如果已经到达了最后一个路径点，那么将索引值置0，绕圈
            if (_currentIndex == _nodes.Count - 1)
            {
                if (_truckActionState == TruckActionState.ToShip)
                {
                    _truckActionState = TruckActionState.WaitFromShipToTruck;
                    //触发将集装箱从船上装载到集卡上的事件
                    LoadContainerFromShipToTruckNotify?.Invoke(this, this.gameObject, this._currentContainer, 0);
                }
                else if (_truckActionState == TruckActionState.FromShipToYard)
                {
                    _truckActionState = TruckActionState.WaitFromShipToTruck;
                    //触发将集装箱从集卡卸载到堆场的事件
                    LoadContainerFromTruckToYardNotify?.Invoke(this, this.gameObject, this._currentContainer, 0);
                }
                _currentIndex = -1;
                return;
            }
            else
            {
                _currentIndex++;
            }
            _targetPosition = _nodes[_currentIndex];
            // 计算新的朝向
            float vangle = Utils.getAngleBetweenVectorAndXAxis(this.transform.position, _targetPosition);
            //将车辆调整为靠右行驶
            this.transform.position = adjustNextCoord(vangle, _nodes[_currentIndex - 1]);
            _targetPosition = adjustNextCoord(vangle, _targetPosition);
            //调整坐标后，重新计算朝向
            vangle = Utils.getAngleBetweenVectorAndXAxis(this.transform.position, _targetPosition);
            vangle = 360 - vangle + 180;
            Vector3 eulerAngle = new Vector3(0, vangle, 0);//欧拉角
            transform.rotation = Quaternion.Euler(eulerAngle);
        }
    }
}
