using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Dijkstra
{
    public class Node
    {
        private string iD;
        private Coord coord;
        private List<Edge> edgeList = new List<Edge>();//Edge的集合,出边表

        public Node(Node other)
        {
            this.iD = other.iD;
            this.coord = other.coord.Clone();
            //if (other.edgeList != null)
            //{
            //    this.edgeList = new List<Edge>();
            //    foreach (Edge edge in other.edgeList)
            //    {
            //        this.edgeList.Add(edge.Clone());
            //    }
            //}
        }

        public Node Clone()
        {
            return new Node(this);
        }

        public Node(string id, Coord coord)
        {
            this.iD = id;
            this.coord = coord;
        }

        #region property

        public string ID
        {
            get
            {
                return this.iD;
            }
        }

        public Coord Coord
        {
            get
            {
                return this.coord;
            }
        }

        public List<Edge> EdgeList
        {
            get
            {
                return this.edgeList;
            }
        }

        public void AddEdge(Edge edge) {
            this.edgeList.Add(edge);
        }

        public Edge GetEdge(string startNodeId, string endNodeId)
        {
            foreach (Edge edge in this.edgeList)
            {
                if (edge.StartNodeID == startNodeId && edge.EndNodeID == endNodeId)
                {
                    return edge;
                }
            }
            return null;
        }
        #endregion
    }

}
