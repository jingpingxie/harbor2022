using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Dijkstra
{
    public class Utils
    {
        static public double getDistance(Coord c1, Coord c2) {
            double dx = c2.X - c1.X;
            double dy = c2.Z - c1.Z;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
