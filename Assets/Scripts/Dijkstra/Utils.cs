using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Dijkstra
{
    public class Utils
    {
        // 计算两点之间的距离
        static public double getDistance(Coord c1, Coord c2)
        {
            double dx = c2.X - c1.X;
            double dy = c2.Z - c1.Z;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        //计算矢量线和X轴之间的角度
        static public float getAngleBetweenVectorAndXAxis(Vector3 from, Vector3 to)
        {
            Vector3 xAxis = Vector3.right; // X轴的向量表示
            UnityEngine.Vector3 targetDirection = to - from;
            //Vector3.Project计算向量在指定轴上的投影，向量本身减去此投影向量就为在XZ平面上的向量
            Vector3 vxz = targetDirection - Vector3.Project(targetDirection, Vector3.up);
            //这个方法的意图是求x轴与矢量线之间的夹角，并且此夹角的大小为两者之间的锐角，所以，不管什么情况，这个函数返回的值都不可能大于180
            float angle = Vector3.Angle(xAxis, vxz);
            ////Vector3.Cross 叉乘返回为同时垂直于两个参数向量的向量，方向可朝上也可朝下，由两向量夹角的方向决定。
            //Vector3 ABCross = Vector3.Cross(targetDirection, xAxis);
            ////判断是x轴上面还是下面
            //return (ABCross.z > 0) ? angle : (360 - angle);
            if (from.z > to.z) { 
                angle = 360 - angle; 
            }
            return angle;
        }
        static public Quaternion getRotation(Vector3 from, Vector3 to)
        {
            float vangle = Utils.getAngleBetweenVectorAndXAxis(from, to);
            vangle = 360 - vangle + 180;
            Vector3 eulerAngle = new Vector3(0, vangle, 0);//欧拉角
            return Quaternion.Euler(eulerAngle);
        }
    }
}
