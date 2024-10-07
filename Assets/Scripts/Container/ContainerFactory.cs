using Assets.Scripts.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ContainerFactory
    {
        UnityEngine.Object[] m_AssetContainers = new GameObject[7];

        public ContainerFactory()
        {
            this.Init();
        }

        private void Init()
        {
            m_AssetContainers[(int)ContainerType.Blue] = Resources.Load("Container/Port-container_blue", typeof(GameObject));
            m_AssetContainers[(int)ContainerType.Brown] = Resources.Load("Container/Port-container_brown", typeof(GameObject));
            m_AssetContainers[(int)ContainerType.Darkred] = Resources.Load("Container/Port-container_darkred", typeof(GameObject));
            m_AssetContainers[(int)ContainerType.Green] = Resources.Load("Container/Port-container_green", typeof(GameObject));
            m_AssetContainers[(int)ContainerType.Grey] = Resources.Load("Container/Port-container_grey", typeof(GameObject));
            m_AssetContainers[(int)ContainerType.Maersk] = Resources.Load("Container/Port-container_maersk", typeof(GameObject));
            m_AssetContainers[(int)ContainerType.Orange] = Resources.Load("Container/Port-container_orange", typeof(GameObject));
        }

        public UnityEngine.Object GetAssetContainer(ContainerType containerType) { 
            return m_AssetContainers[(int)containerType];
        }
    }
}
