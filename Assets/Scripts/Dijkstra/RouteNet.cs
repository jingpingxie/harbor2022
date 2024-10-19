using System;
using System.Collections.Generic;

namespace Assets.Scripts.Dijkstra
{
    public class RouteNet
    {
        List<Node> m_nodeList = new List<Node>();
        List<Edge> m_edgeList = new List<Edge>();
        Dictionary<String, Node> m_nodeMap;

        public RouteNet()
        {
        }

        public void SetNodeMap(Dictionary<string, Node> nodeMap)
        {
            this.m_nodeMap = nodeMap;
        }

        public RouteNet(RouteNet other)
        {
            m_nodeMap = new Dictionary<string, Node>();
            foreach (Node node in other.m_nodeList)
            {
                if (!m_nodeMap.TryGetValue(node.ID, out Node value))
                {
                    Node newNode = node.Clone();
                    this.m_nodeList.Add(newNode);
                    m_nodeMap[newNode.ID] = newNode;
                }
            }

            foreach (Edge edge in other.m_edgeList)
            {
                Edge newEdge = edge.Clone();
                this.m_edgeList.Add(newEdge);
                m_nodeMap[newEdge.StartNodeID].AddEdge(newEdge);
            }
        }
        public RouteNet Clone()
        {
            return new RouteNet(this);
        }
        /// <summary>
        /// 获取图的节点集合
        /// </summary>
        public List<Node> NodeList
        {
            get { return this.m_nodeList; }
        }

        public void AddEdge(Edge edge)
        {
            m_edgeList.Add(edge);
        }

        public void AddNodeList(IEnumerable<Node> nodeList)
        {
            m_nodeList.AddRange(nodeList);
        }

        public Node GetNodeById(string nodeId)
        {
            this.m_nodeMap.TryGetValue(nodeId, out Node node);
            return node;
        }

        public Coord GetMinDistanceCoord(Coord coord)
        {
            double minDistance = double.MaxValue;
            Edge nearestEdge = null;//距离最近的路径
            Coord foot = null;//点到该路径的垂足
            for (int i = 0; i < m_edgeList.Count; i++)
            {
                Coord footTemp;
                double distance = m_edgeList[i].GetDistanceAndFootToPoint(coord, out footTemp);
                if (distance < minDistance)
                {
                    nearestEdge = m_edgeList[i];
                    foot = footTemp;
                    minDistance = distance;
                }
            }
            return foot;
        }

        public Node InsertNearstNode(string nodeId, Coord coord)
        {
            double minDistance = double.MaxValue;
            Edge nearestEdge1 = null;//距离最近的路径
            Coord foot = null;//点到该路径的垂足
            for (int i = 0; i < m_edgeList.Count; i++)
            {
                Coord footTemp;
                double distance = m_edgeList[i].GetDistanceAndFootToPoint(coord, out footTemp);
                if (distance < minDistance)
                {
                    //获取距离最近的原顺时针路径
                    nearestEdge1 = m_edgeList[i];
                    foot = footTemp;
                    minDistance = distance;
                }
            }
            if (foot == null)
            {
                return null;
            }
            Node startNode = this.GetNodeById(nearestEdge1.StartNodeID);
            if (startNode.Coord == coord)
            {
                return startNode;
            }
            Node endNode = this.GetNodeById(nearestEdge1.EndNodeID);
            if (endNode.Coord == coord)
            {
                return endNode;
            }
            //获取距离最近的原逆时针路径
            Edge nearestEdge2 = endNode.GetEdge(endNode.ID, startNode.ID);

            //插入一个节点
            Node insertNode = new Node(nodeId, foot);
            //修改原先的路径
            nearestEdge1.EndCoord = foot;
            nearestEdge1.EndNodeID = insertNode.ID;
            nearestEdge1.Weight = Utils.getDistance(startNode.Coord, foot);
            nearestEdge2.EndCoord = foot;
            nearestEdge2.EndNodeID = insertNode.ID;
            nearestEdge2.Weight = Utils.getDistance(endNode.Coord, foot);

            //插入2条路径
            //顺时针路径
            Edge insertEdge1 = new Edge();
            insertEdge1.StartCoord = foot;
            insertEdge1.StartNodeID = insertNode.ID;
            insertEdge1.EndCoord = endNode.Coord;
            insertEdge1.EndNodeID = endNode.ID;
            insertEdge1.Weight = nearestEdge2.Weight;
            insertNode.AddEdge(insertEdge1);
            m_edgeList.Add(insertEdge1);

            //逆时针路径
            Edge insertEdge2 = new Edge();
            insertEdge2.StartCoord = foot;
            insertEdge2.StartNodeID = insertNode.ID;
            insertEdge2.EndCoord = startNode.Coord;
            insertEdge2.EndNodeID = startNode.ID;
            insertEdge2.Weight = nearestEdge1.Weight;
            insertNode.AddEdge(insertEdge2);
            m_edgeList.Add(insertEdge2);

            m_nodeList.Add(insertNode);
            return insertNode;
        }
    }
}
