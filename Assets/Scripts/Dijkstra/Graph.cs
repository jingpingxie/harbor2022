using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using System.Drawing;
using Unity.VisualScripting;

namespace Assets.Scripts.Dijkstra
{
    public class Graph
    {
        //建立连接语句
        //charset=utf8这句要写，不然可能会报错                                 
        string constr = "server=127.0.0.1;User Id=root;password=test123456;Database=harbor;charset=utf8";
        //建立连接
        public MySqlConnection mycon;

        public List<Node> m_nodeList = new List<Node>();
        public Graph()
        {
            
        }

        private void ConnectMysql()
        {
            //建立连接
            mycon = new MySqlConnection(constr);
            //打开连接
            mycon.Open();

            bool isOK = mycon.Ping();
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
        /// 获取图的节点集合
        /// </summary>
        public List<Node> NodeList
        {
            get { return this.m_nodeList; }
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
            MySqlCommand myselect = new MySqlCommand(selstr, mycon);

            DataSet ds = new DataSet();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(selstr, mycon);
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
                            //不是终点，加入下一边
                            Coord nextCoord = new Coord(Double.Parse(coordArray[j + 1][1].ToString()),
                                Double.Parse(coordArray[j + 1][2].ToString()),
                                Double.Parse(coordArray[j + 1][3].ToString()));
                            double distance = Utils.getDistance(coord, nextCoord);
                            String nextNodeId = coordArray[j + 1][0].ToString();
                            Edge edge = new Edge();
                            edge.StartNodeID = currentNodeId;
                            edge.EndNodeID = nextNodeId;
                            edge.Weight = distance;
                            node.EdgeList.Add(edge);
                        }
                        if (j > 0)
                        {
                            //不是起点，加入上一边
                            Coord lastCoord = new Coord(Double.Parse(coordArray[j - 1][1].ToString()),
                                Double.Parse(coordArray[j - 1][2].ToString()),
                                Double.Parse(coordArray[j - 1][3].ToString()));
                            double distance = Utils.getDistance(coord, lastCoord);
                            String lastNodeId = coordArray[j - 1][0].ToString();
                            Edge edge = new Edge();
                            edge.StartNodeID = currentNodeId;
                            edge.EndNodeID = lastNodeId;
                            edge.Weight = distance;
                            node.EdgeList.Add(edge);
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
                        node.EdgeList.AddRange(nodeList[i].EdgeList);
                    }
                    else
                    {
                        coordMap.Add(nodeList[i].ID, nodeList[i]);
                    }
                }
                this.NodeList.AddRange(coordMap.Values);
            }
            catch (Exception e)
            {
                throw new Exception("SQL:" + selstr + "\n" + e.Message.ToString());
            }
        }
    }
}
