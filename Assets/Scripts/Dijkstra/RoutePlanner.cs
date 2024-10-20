﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Dijkstra
{
    /// <summary>
    /// RoutePlanner 提供图算法中常用的路径规划功能。
    /// 2005.09.06
    /// </summary>
    public class RoutePlanner
    {
        public RoutePlanner()
        {
        }

        #region Paln
        //获取权值最小的路径
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeList">所有的道路节点</param>
        /// <param name="originID">起始节点</param>
        /// <param name="destID">目标节点</param>
        /// <param name="ignoreNodeID">忽略走此节点，避免在道路中间掉头，浪费时间</param>
        /// <returns></returns>
        public RoutePlanResult Plan(List<Node> nodeList, string originID, string destID, string ignoreNodeID)
        {
            //初始化起始节点到其他节点的路径表(权值，经过的节点，是否被处理）
            //同时初始化其他节点的路径表
            PlanCourse planCourse = new PlanCourse(nodeList, originID);
            if (!string.IsNullOrEmpty(ignoreNodeID))
            {
                planCourse[ignoreNodeID].BeProcessed = true;
            }

            Node curNode = this.GetMinWeightRudeNode(planCourse, nodeList, originID);

            #region 计算过程
            while (curNode != null)
            {
                PassedPath curPath = planCourse[curNode.ID];
                foreach (Edge edge in curNode.EdgeList)
                {
                    PassedPath targetPath = planCourse[edge.EndNodeID];
                    if (null == targetPath)
                    {
                        continue;
                    }
                    double tempWeight = curPath.Weight + edge.Weight;

                    if (tempWeight < targetPath.Weight)
                    {
                        targetPath.Weight = tempWeight;
                        targetPath.PassedIDList.Clear();

                        for (int i = 0; i < curPath.PassedIDList.Count; i++)
                        {
                            targetPath.PassedIDList.Add(curPath.PassedIDList[i].ToString());
                        }

                        targetPath.PassedIDList.Add(curNode.ID);
                    }
                }

                //标志为已处理
                planCourse[curNode.ID].BeProcessed = true;
                //获取下一个未处理节点
                curNode = this.GetMinWeightRudeNode(planCourse, nodeList, originID);
            }
            #endregion

            //表示规划结束
            return this.GetResult(planCourse, destID);
        }
        #endregion

        #region private method
        #region GetResult

        /// <summary>
        /// 从PlanCourse表中取出目标节点的PassedPath，这个PassedPath即是规划结果
        /// </summary>
        /// <returns></returns>
        private RoutePlanResult GetResult(PlanCourse planCourse, string destID)
        {
            PassedPath pPath = planCourse[destID];

            if (pPath.Weight == int.MaxValue)
            {
                RoutePlanResult result1 = new RoutePlanResult(null, int.MaxValue);
                return result1;
            }

            string[] passedNodeIDs = new string[pPath.PassedIDList.Count + 1];
            for (int i = 0; i < pPath.PassedIDList.Count; i++)
            {
                passedNodeIDs[i] = pPath.PassedIDList[i].ToString();
            }
            passedNodeIDs[pPath.PassedIDList.Count] = destID;
            RoutePlanResult result = new RoutePlanResult(passedNodeIDs, pPath.Weight);

            return result;
        }
        #endregion

        #region GetMinWeightRudeNode

        /// <summary>
        /// 从PlanCourse取出一个当前累积权值最小，并且没有被处理过的节点
        /// </summary>
        /// <returns></returns>
        private Node GetMinWeightRudeNode(PlanCourse planCourse, List<Node> nodeList, string originID)
        {
            double weight = double.MaxValue;
            Node destNode = null;

            foreach (Node node in nodeList)
            {
                if (node.ID == originID)
                {
                    continue;
                }

                PassedPath pPath = planCourse[node.ID];
                if (pPath.BeProcessed)
                {
                    continue;
                }

                if (pPath.Weight < weight)
                {
                    weight = pPath.Weight;
                    destNode = node;
                }
            }

            return destNode;
        }
        #endregion
        #endregion
    }
}
