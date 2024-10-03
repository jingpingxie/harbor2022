using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Dijkstra
{
    public class Coord
    {
        public double X;
        public double Y;
        public double Z;

        public Coord(double x,double y,double z) { 
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
