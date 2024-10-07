using Assets.Scripts.Ship;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ShipPlan
    {

        public List<Container> Plan(string shipName,int baySum, int rowSum, int columnSum, int columnSumInFirstBay, int columnSumInLastBay)
        {
            List<Container> containerList = new List<Container>();
            for (int bayIndex = 0; bayIndex < baySum; bayIndex++)
            {
                List<Container> bayContainerList = LoadContainerToBay(shipName,baySum, rowSum, columnSum, columnSumInFirstBay, columnSumInLastBay, bayIndex);
                containerList.AddRange(bayContainerList);
            }
            return containerList;
        }

        List<Container> LoadContainerToBay(string shipName, int baySum, int rowSum, int columnSum, int columnSumInFirstBay, int columnSumInLastBay, int bayIndex)
        {
            float y;
            int columnSumTemp;
            float xAdjust;
            if (bayIndex == 0)
            {
                //第一个bay
                columnSumTemp = columnSumInFirstBay;
                xAdjust = -6;
                y = bayIndex * 16 - 72;//纵向
            }
            else if (bayIndex == baySum - 1)
            {
                //最后一个bay
                columnSumTemp = columnSumInLastBay;
                xAdjust = -6;
                y = bayIndex * 16 - 42;//纵向
            }
            else
            {
                //中间的Bay
                columnSumTemp = columnSum;
                xAdjust = -11;
                y = bayIndex * 15.5f - 65;//纵向
            }
            List<Container> containerList = new List<Container>();
            for (int row = 0; row < rowSum; row++)
            {
                float z = row * 2.5f;//高度
                for (int column = 0; column < columnSumTemp; column++)
                {
                    ContainerType containerType = (ContainerType)Random.Range(0, 6);
                    //x:2.5,y:,z:2.5
                    string name = string.Format("{0}_{1:D3}{2:D3}{3:D3}", shipName,bayIndex + 1, row + 1, column + 1);
                    float x = column * 2.5f + xAdjust;//横向
                    Container container = new Container(name, containerType, x, y, z);
                    containerList.Add(container);
                }
            }
            return containerList;
        }
    }
}
