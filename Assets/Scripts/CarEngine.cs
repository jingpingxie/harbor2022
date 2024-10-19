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
    //���������ת��Ƕ�
    public float maxSteerAngle = 90;

    //��ȡ·��
    private List<Transform> nodes;
    //·��������ֵ
    private int currentIndex = 0;


    ////������ײ��
    //public WheelCollider LF;  //��ǰ��
    //public WheelCollider RF;  //��ǰ��

    ////��̥��ײ��
    //public WheelCollider LB; //���
    //public WheelCollider RB; //�Һ�
    GameObject carHead;

    private float targetSteerAngle; //������̥ʵ�ʵ�ת��

    //���ֶ�������
    public float maxMotorTorque = 600f; //���������   
    public float maxSpeed = 60f; //�����
    //public float currentSpeed;   //������ǰ����

    //�����ȶ��Ե�����
    private Rigidbody rb;
    public UnityEngine.Vector3 centerOfMass = new UnityEngine.Vector3(0, -1, 0);

    //��������
    public float maxBrakeTorque = 600f;  //ɲ�����ƶ���

    //ɲ�����ֵ�����
    public bool isBraking;
    public Texture2D textureNormal;     //��ɲ��ʱ���Ƶ���ͼ
    public Texture2D textureBreaking;   //ɲ��ʱ���Ƶ���ͼ
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
        //��ȡҪװж�ļ�װ��
        GameObject container = GameObject.Find("Port-Container_SHIP1/Port-container_38");
        //��װ��λ��
        Transform containerPos = container.transform;
        //��ȡ������λ��
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
        //�ı䳵��������,�����ȶ��Ե�����
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
        //���ݳ���λ�ú�·�����λ�����������������
        UnityEngine.Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentIndex].position);
        //������̥��ʵ��ת��
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        this.targetSteerAngle = newSteer;
        ////��ʵ��ת�����õ���ǰ�ֺ���ǰ�ֵ�ת��
        //LF.steerAngle = targetSteerAngle;
        //RF.steerAngle = targetSteerAngle;
        this.carHead.transform.Rotate(UnityEngine.Vector3.down * Time.deltaTime * targetSteerAngle);
    }

    private void Drive()
    {
        ////���㳵��
        //currentSpeed = 2 * Mathf.PI * LF.radius * LF.rpm * 60 / 1000;
        ////�������С������ٶȣ���ô���賵�ֶ���
        ////�޸�һ��������ʻ������
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
        //�ж�������ǰ�ľ����·����֮��ľ���
        if (UnityEngine.Vector3.Distance(new UnityEngine.Vector3(transform.position.x, 0, transform.position.z),
                           new UnityEngine.Vector3(nodes[currentIndex].position.x, 0,
                           nodes[currentIndex].position.z)) < 0.5f)
        {
            //����Ѿ����������һ��·���㣬��ô������ֵ��0����Ȧ
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
        //    //����ɲ��
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
