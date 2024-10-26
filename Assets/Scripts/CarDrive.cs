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

// ����һ�����������¼�ί������
public delegate void ArrivedEventHandler(int param);
public class CarDrive : MonoBehaviour
{
    //������������¼�
    public event ArrivedEventHandler ArrivedChanged;

    //��ȡ·��
    private List<UnityEngine.Vector3> nodes;
    //·��������ֵ
    private int currentIndex = 0;


    public float speed = 10;

    //�����ȶ��Ե�����
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
        //��ȡҪװж�ļ�װ��
        GameObject container = GameObject.Find("Port-Container_SHIP1/Port-container_38");
        //��װ��λ��
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


    // Update is called once per frame
    void Update()
    {
        if (currentIndex < 0) { return; }
        //�ƶ�λ��
        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);

        if (UnityEngine.Vector3.Distance(new UnityEngine.Vector3(this.transform.position.x, 2.2f, this.transform.position.z), _targetPosition)
                            < 0.5f)
        {
            //����Ѿ����������һ��·���㣬��ô������ֵ��0����Ȧ
            if (currentIndex == nodes.Count - 1)
            {
                //�����¼�
                ArrivedChanged?.Invoke(0);
                currentIndex = -1;
                return;
            }
            else
            {
                currentIndex++;
            }
            _targetPosition = nodes[currentIndex];
            // �����µĳ���
            float vangle = Utils.getAngleBetweenVectorAndXAxis(this.transform.position, _targetPosition);
            //����������Ϊ������ʻ
            this.transform.position = adjustNextCoord(vangle, nodes[currentIndex - 1]);
            _targetPosition = adjustNextCoord(vangle, _targetPosition);
            //������������¼��㳯��
            vangle = Utils.getAngleBetweenVectorAndXAxis(this.transform.position, _targetPosition);
            vangle = 360 - vangle + 180;
            Vector3 eulerAngle = new Vector3(0, vangle, 0);//ŷ����
            transform.rotation = Quaternion.Euler(eulerAngle);
        }
    }
}
