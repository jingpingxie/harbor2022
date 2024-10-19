using Assets.Scripts.Dijkstra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class HarborApp
    {
        public static Graph graph = new Graph();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Main()
        {
            graph.Init();
        }

        public static String[] Plan(String startId, String destId, string ignoreNodeID)
        {
            RoutePlanner planner = new RoutePlanner();
            RoutePlanResult routePlanResult = planner.Plan(graph.RouteNet.NodeList, startId, destId, ignoreNodeID);
            String[] resultNodes = routePlanResult.ResultNodes;
            return resultNodes;
        }

        public static String[] Plan(Coord startPosition, Coord endPosition)
        {
            RouteNet routeNet = graph.RouteNet.Clone();
            Node startNode = routeNet.InsertNearstNode("startNodeId", startPosition);
            Node endNode = routeNet.InsertNearstNode("endNodeId", endPosition);
            RoutePlanner planner = new RoutePlanner();
            RoutePlanResult routePlanResult = planner.Plan(routeNet.NodeList, startNode.ID, endNode.ID, null);
            return routePlanResult.ResultNodes;
        }
    }
}
