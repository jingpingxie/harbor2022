using Assets.Scripts.Ship;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Container
    {
        string _name;
        ContainerType _containerType;
        float _x;
        float _y;
        float _z;
        public float X { get { return _x; } }
        public float Y { get { return _y; } }
        public float Z { get { return _z; } }
        public ContainerType ContainerType { get { return _containerType; } }

        public Container(String name, ContainerType containerType, float x, float y, float z)
        {
            this._name = name;
            this._containerType = containerType;
            this._x = x;
            this._y = y;
            this._z = z;
        }
    }
}
