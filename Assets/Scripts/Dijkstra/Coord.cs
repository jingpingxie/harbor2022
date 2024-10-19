using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Dijkstra
{
    public class Coord
    {
        public float X;
        public float Y;
        public float Z;

        public Coord(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Coord(Coord other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
        }
        public Coord Clone()
        {
            return new Coord(this);
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            if ((Object)c1 == null)
            {
                return (Object)c2 == null;
            }
            else
            {
                return c1.Equals(c2);
            }
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            if ((object)this == obj)
            {
                return true;
            }
            Coord myClass = obj as Coord;
            return (this.X == myClass.X && this.Y == myClass.Y && this.Z == myClass.Z);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }
}
