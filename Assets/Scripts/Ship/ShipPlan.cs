using Assets.Scripts.Db;
using Assets.Scripts.Ship;
//using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static Google.Protobuf.Reflection.FeatureSet.Types;

namespace Assets.Scripts
{
    public class ShipPlan
    {
        public List<Container> GetPlanFromDb(string shipName)
        {
            return GetContainerListFormDb(shipName);
        }


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
        /// <summary>
        /// 从数据库加载集装箱计划
        /// </summary>
        /// <param name="shipName"></param>
        /// <param name="baySum"></param>
        /// <param name="rowSum"></param>
        /// <param name="columnSum"></param>
        /// <param name="columnSumInFirstBay"></param>
        /// <param name="columnSumInLastBay"></param>
        /// <param name="bayIndex"></param>
        /// <returns></returns>
        List<Container> LoadContainerFromDb(string shipName, int baySum, int rowSum, int columnSum, int columnSumInFirstBay, int columnSumInLastBay, int bayIndex)
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
                    string name = string.Format("{0}_{1:D3}{2:D3}{3:D3}", shipName, bayIndex + 1, row + 1, column + 1);
                    float x = column * 2.5f + xAdjust;//横向
                    Container container = new Container(name, containerType, x, y, z);
                    containerList.Add(container);
                    InsertToDb(shipName, name, containerType, x, y, z);
                }
            }
            return containerList;
        }
        /// <summary>
        /// 动态生成集装箱计划数据，并且保存到数据库
        /// </summary>
        /// <param name="shipName"></param>
        /// <param name="baySum"></param>
        /// <param name="rowSum"></param>
        /// <param name="columnSum"></param>
        /// <param name="columnSumInFirstBay"></param>
        /// <param name="columnSumInLastBay"></param>
        /// <param name="bayIndex"></param>
        /// <returns></returns>

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
                    InsertToDb(shipName, name, containerType, x, y, z);
                }
            }
            return containerList;
        }

        void InsertToDb(string shipName,string containerName, ContainerType containerType,float x, float y,float z) {
            string query = string.Format("insert into containers(shipName,containerName,containerType,x,y,z) values('{0}','{1}',{2},{3},{4},{5})", shipName,containerName, (int)containerType, x, y, z);
            DbManager.ExecuteNonQuery(query);
        }

        List<Container> GetContainerListFormDb(string shipName)
        {
            string query = string.Format("select * from containers where shipName='{0}'", shipName);
            DataTable resultTable = DbManager.ExecuteQuery(query);
            List<Container> containerList = new List<Container>();
            foreach (DataRow dataRow in resultTable.Rows) // 遍历结果集。 
            {
                string containerName = dataRow.Field<string>("containerName");
                float x = dataRow.Field<float>("x");
                float y = dataRow.Field<float>("y");
                float z = dataRow.Field<float>("z");
                object ctype = dataRow["containerType"];
                ContainerType containerType = (ContainerType)System.Enum.Parse(typeof(ContainerType), ctype.ToString());
                Container container = new Container(containerName, containerType, x, y, z);
                containerList.Add(container);
            }
            return containerList;
        }
    }
}
