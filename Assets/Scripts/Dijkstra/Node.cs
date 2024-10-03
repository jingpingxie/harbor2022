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
        private List<Edge> edgeList;//Edge的集合,出边表

        public Node(string id, Coord coord)
        {
            this.edgeList = new List<Edge>();
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

        public Coord Coord {
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

        #endregion
    }

}
