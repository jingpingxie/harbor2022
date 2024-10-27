using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal enum TruckActionState
    {
        Unkown,
        /// <summary>
        /// 驶往船舶
        /// </summary>
        ToShip,
        /// <summary>
        /// 驶往堆场
        /// </summary>
        ToYard,
        /// <summary>
        /// 等待从船舶卸载集装箱到集卡
        /// </summary>
        WaitFromShipToTruck,
        /// <summary>
        /// 从船舶驶往堆场
        /// </summary>
        FromShipToYard,
        /// <summary>
        /// 等待从集卡卸载集装箱到堆场
        /// </summary>
        WaitFromTruckToYard,
        /// <summary>
        /// 等待从堆场装载集装箱到集卡
        /// </summary>
        WaitFromYardToTruck,
        /// <summary>
        /// 等待从集卡装载集装箱到船舶
        /// </summary>
        WaitFromTruckToShip,
        BackToPark
    }
}
