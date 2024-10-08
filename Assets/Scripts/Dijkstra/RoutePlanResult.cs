﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Dijkstra
{
    public class RoutePlanResult
    {
        public RoutePlanResult(string[] passedNodes, double value)
        {
            m_resultNodes = passedNodes;
            m_value = value;
        }

        private string[] m_resultNodes;
        /// <summary>
        /// 最短路径经过的节点
        /// </summary>
        public string[] ResultNodes
        {
            get { return m_resultNodes; }
        }

        private double m_value;
        /// <summary>
        /// 最短路径的值
        /// </summary>
        private double Value
        {
            get { return m_value; }
        }
    }
}
