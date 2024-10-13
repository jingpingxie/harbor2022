using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Dijkstra
{
    public class RouteParser : MonoBehaviour
    {
        RoutePlanner planner = new RoutePlanner();
        Graph graph = new Graph();
        void Start()
        {
            try
            {
                graph.Init();
                Plan("3_1", "1_1");
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        public String[] Plan(String startId, String destId)
        {            
            RoutePlanResult routePlanResult = planner.Plan(graph.RouteNet.NodeList, startId, destId,"2_1");
            String[] resultNodes = routePlanResult.ResultNodes;
            Debug.Log(resultNodes);
            return resultNodes;
        }
    }
}
