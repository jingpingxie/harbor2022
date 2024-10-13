using Assets.Scripts.Dijkstra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class ContainerLoader : MonoBehaviour
    {
        Assets.Scripts.Dijkstra.Graph graph = new Assets.Scripts.Dijkstra.Graph();
        RoutePlanner planner = new RoutePlanner();
        void Start()
        {
            try
            {
                graph.Init();
                //Plan("3_1", "1_1");
                dispatch();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
        /// <summary>
        /// 物流调度
        /// </summary>
        public void dispatch()
        {
            //获取要装卸的集装箱
            GameObject container = GameObject.Find("Port-Container_SHIP1/Port-container_38");
            //集装箱位置
            Transform containerPos = container.transform;
            //获取集卡的位置
            GameObject truck = GameObject.Find("HG0702");
            Transform truckPos = truck.transform;
            RouteNet routeNet = graph.RouteNet.Clone();
            Node startNode = routeNet.InsertNearstNode("startNodeId", new Coord(truckPos.position.x, truckPos.position.y, truckPos.position.z));
            Node endNode = routeNet.InsertNearstNode("endNodeId", new Coord(containerPos.position.x, containerPos.position.y, containerPos.position.z));
            RoutePlanResult routePlanResult = planner.Plan(routeNet.NodeList, startNode.ID, endNode.ID, null);
            String[] resultNodes = routePlanResult.ResultNodes;
            //routeNet.m_nodeList.Clear();
            Debug.Log(resultNodes);
        }
    }
}
