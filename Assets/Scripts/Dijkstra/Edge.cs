using System;

namespace Assets.Scripts.Dijkstra
{
    public class Edge
    {
        public Coord StartCoord;
        public Coord EndCoord;
        public string StartNodeID;
        public string EndNodeID;
        public double Weight; //权值，代价       
        public Edge() { 
        }
        public Edge(Edge other)
        {
            this.StartCoord = other.StartCoord.Clone();
            this.EndCoord = other.EndCoord.Clone();
            this.StartNodeID = other.StartNodeID;
            this.EndNodeID = other.EndNodeID;
            this.Weight = other.Weight;
        }
        public Edge Clone()
        {
            return new Edge(this);
        }
        /// <summary>
        /// 获取指定坐标到路径的距离和垂足
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public double GetDistanceAndFootToPoint(Coord coord, out Coord foot)
        {
            double len = this.Weight;
            Coord startCoord = this.StartCoord;
            Coord endCoord = this.EndCoord;
            if (len == 0)
            {
                //线段长度为0，即线段为点
                foot = startCoord;
                return Utils.getDistance(coord, startCoord);
            }
            double r = ((coord.X - startCoord.X) * (endCoord.X - startCoord.X) + (coord.Z - startCoord.Z) * (endCoord.Z - startCoord.Z)) / Math.Pow(len, 2);
            if (r <= 0)
            {
                //垂足在StartCoord处
                foot = startCoord;
                return Utils.getDistance(coord, startCoord);
            }
            else if (r >= 1)
            {
                //垂足在p2处
                foot = endCoord;
                return Utils.getDistance(coord, endCoord);
            }
            else
            {
                //垂足在线段上
                foot = new Coord(startCoord.X + r * (endCoord.X - startCoord.X), 0, startCoord.Z + r * (endCoord.Z - startCoord.Z));
                return Utils.getDistance(coord, foot);
            }
        }
    }
}
