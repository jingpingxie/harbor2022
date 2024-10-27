using Assets.Scripts;
using Assets.Scripts.Dijkstra;
using System.Collections.Generic;
using UnityEngine;
using System;

//https://blog.csdn.net/qq_68117303/article/details/133011345
//https://docs.unity3d.com/cn/current/Manual/class-WheelCollider.html

// ����һ�����������¼�ί������
public delegate void LoadContainerFromShipToTruckEventHandler(TruckDrive carDrive, GameObject truck, GameObject loadedContainer, int param);
public delegate void LoadContainerFromTruckToYardEventHandler(TruckDrive carDrive, GameObject truck, GameObject loadedContainer, int param);
public class TruckDrive : MonoBehaviour
{
    //������������¼�
    public event LoadContainerFromShipToTruckEventHandler LoadContainerFromShipToTruckNotify;
    public event LoadContainerFromTruckToYardEventHandler LoadContainerFromTruckToYardNotify;

    //��ȡ·��
    private List<UnityEngine.Vector3> _nodes;
    //·��������ֵ
    private int _currentIndex = 0;

    public float Speed = 10;

    //�����ȶ��Ե�����
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
        //��ȡҪװж�ļ�װ��
        _currentContainer = GameObject.Find("Port-Container_SHIP1/Port-container_38");
        //��װ��λ��
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
    /// ����������Ϊ������ʻ
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
        //�ƻ����ͼ�װ�䵽�ѳ���·��
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
        //�ƶ�λ��
        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, _targetPosition, Speed * Time.deltaTime);

        if (UnityEngine.Vector3.Distance(new UnityEngine.Vector3(this.transform.position.x, CarHeight, this.transform.position.z), _targetPosition)
                            < 0.5f)
        {
            //����Ѿ����������һ��·���㣬��ô������ֵ��0����Ȧ
            if (_currentIndex == _nodes.Count - 1)
            {
                if (_truckActionState == TruckActionState.ToShip)
                {
                    _truckActionState = TruckActionState.WaitFromShipToTruck;
                    //��������װ��Ӵ���װ�ص������ϵ��¼�
                    LoadContainerFromShipToTruckNotify?.Invoke(this, this.gameObject, this._currentContainer, 0);
                }
                else if (_truckActionState == TruckActionState.FromShipToYard)
                {
                    _truckActionState = TruckActionState.WaitFromShipToTruck;
                    //��������װ��Ӽ���ж�ص��ѳ����¼�
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
            // �����µĳ���
            float vangle = Utils.getAngleBetweenVectorAndXAxis(this.transform.position, _targetPosition);
            //����������Ϊ������ʻ
            this.transform.position = adjustNextCoord(vangle, _nodes[_currentIndex - 1]);
            _targetPosition = adjustNextCoord(vangle, _targetPosition);
            //������������¼��㳯��
            vangle = Utils.getAngleBetweenVectorAndXAxis(this.transform.position, _targetPosition);
            vangle = 360 - vangle + 180;
            Vector3 eulerAngle = new Vector3(0, vangle, 0);//ŷ����
            transform.rotation = Quaternion.Euler(eulerAngle);
        }
    }
}
