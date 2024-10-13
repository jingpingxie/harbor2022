using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Newtonsoft.Json;

namespace Assets.Scripts.Dijkstra
{
    public class Graph
    {
        //建立连接语句
        //charset=utf8这句要写，不然可能会报错                                 
        string mConstr = "server=127.0.0.1;User Id=root;password=test123456;Database=harbor;charset=utf8";
        //建立连接
        MySqlConnection mConnection;

        RouteNet mRouteNet = new RouteNet();

        public Graph()
        {
        }

        public RouteNet RouteNet { get { return mRouteNet; } }

        private void ConnectMysql()
        {
            //建立连接
            mConnection = new MySqlConnection(mConstr);
            //打开连接
            mConnection.Open();

            bool isOK = mConnection.Ping();
            if (isOK)
            {
                Debug.Log("数据库已连接");
            }
            else
            {
                Debug.Log("数据库连接错误");
            }
        }


        /// <summary>
        /// 初始化拓扑图
        /// 
        /// 
        /// </summary>
        public void Init()
        {
            ConnectMysql();
            //查询数据
            string selstr = "select * from roads";
            MySqlCommand myselect = new MySqlCommand(selstr, mConnection);

            DataSet ds = new DataSet();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(selstr, mConnection);
                da.Fill(ds);
                Debug.Log("数据库数据:");
                List<Node> nodeList = new List<Node>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    String roadName = ds.Tables[0].Rows[i][1].ToString();
                    String json = ds.Tables[0].Rows[i][2].ToString();
                    Debug.Log(json);
                    List<List<object>> coordArray = JsonConvert.DeserializeObject<List<List<object>>>(json);
                    for (int j = 0; j < coordArray.Count; j++)
                    {
                        //将坐标加入map列表，并且去重
                        Coord coord = new Coord(
                            Double.Parse(coordArray[j][1].ToString()),
                            Double.Parse(coordArray[j][2].ToString()),
                            Double.Parse(coordArray[j][3].ToString()));
                        String currentNodeId = coordArray[j][0].ToString();
                        Node node = new Node(currentNodeId, coord);
                        if (j < coordArray.Count - 1)
                        {
                            //顺时针edge,不是终点，加入下一边
                            Coord nextCoord = new Coord(Double.Parse(coordArray[j + 1][1].ToString()),
                                Double.Parse(coordArray[j + 1][2].ToString()),
                                Double.Parse(coordArray[j + 1][3].ToString()));
                            double distance = Utils.getDistance(coord, nextCoord);
                            String nextNodeId = coordArray[j + 1][0].ToString();
                            Edge edge = new Edge();
                            edge.StartNodeID = currentNodeId;
                            edge.EndNodeID = nextNodeId;
                            edge.StartCoord = coord;
                            edge.EndCoord = nextCoord;
                            edge.Weight = distance;
                            node.EdgeList.Add(edge);
                            mRouteNet.AddEdge(edge);
                        }
                        if (j > 0)
                        {
                            //逆时针edge,不是起点，加入上一边
                            Coord lastCoord = new Coord(Double.Parse(coordArray[j - 1][1].ToString()),
                                Double.Parse(coordArray[j - 1][2].ToString()),
                                Double.Parse(coordArray[j - 1][3].ToString()));
                            double distance = Utils.getDistance(coord, lastCoord);
                            String lastNodeId = coordArray[j - 1][0].ToString();
                            Edge edge = new Edge();
                            edge.StartNodeID = currentNodeId;
                            edge.EndNodeID = lastNodeId;
                            edge.StartCoord = coord;
                            edge.EndCoord = lastCoord;
                            edge.Weight = distance;
                            node.EdgeList.Add(edge);
                            mRouteNet.AddEdge(edge);
                        }
                        nodeList.Add(node);
                    }
                }
                //合并相同坐标的节点
                Dictionary<String, Node> coordMap = new Dictionary<String, Node>();
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (coordMap.TryGetValue(nodeList[i].ID, out Node node))
                    {
                        //将node id同名的node中的edge合并到一个node
                        node.EdgeList.AddRange(nodeList[i].EdgeList);
                    }
                    else
                    {
                        coordMap.Add(nodeList[i].ID, nodeList[i]);
                    }
                }
                //foreach (KeyValuePair<String, Node> kvp in coordMap)
                //{
                //    foreach (Edge edge in kvp.Value.EdgeList)
                //    {
                //        edge.StartNode = coordMap[edge.StartNodeID];
                //        edge.EndNode = coordMap[edge.EndNodeID];
                //    }
                //}
                this.mRouteNet.SetNodeMap(coordMap);
                this.mRouteNet.AddNodeList(coordMap.Values);
            }
            catch (Exception e)
            {
                throw new Exception("SQL:" + selstr + "\n" + e.Message.ToString());
            }
        }
    }
}
