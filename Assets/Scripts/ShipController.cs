using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    const int BaySum = 9;
    const int RowSum = 4;
    const int ColumnSumInFirstBay = 6;
    const int ColumnSum = 10;
    const int ColumnSumInLastBay = 6;
    Object[] m_AssetContainers = new GameObject[7];
    // Start is called before the first frame update
    void Start()
    {
        m_AssetContainers[0] = Resources.Load("Container/Port-container_blue", typeof(GameObject));
        m_AssetContainers[1] = Resources.Load("Container/Port-container_brown", typeof(GameObject));
        m_AssetContainers[2] = Resources.Load("Container/Port-container_darkred", typeof(GameObject));
        m_AssetContainers[3] = Resources.Load("Container/Port-container_green", typeof(GameObject));
        m_AssetContainers[4] = Resources.Load("Container/Port-container_grey", typeof(GameObject));
        m_AssetContainers[5] = Resources.Load("Container/Port-container_maersk", typeof(GameObject));
        m_AssetContainers[6] = Resources.Load("Container/Port-container_orange", typeof(GameObject));
        LoadContainer();
    }
    void LoadContainer()
    {
        for (int bayIndex = 0; bayIndex < BaySum; bayIndex++)
        {
            LoadContainerToBay(bayIndex);
        }
    }

    void LoadContainerToBay(int bayIndex)
    {
        float y;
        int columnSum;
        float xAdjust;
        if (bayIndex == 0)
        {
            //第一个bay
            columnSum = ColumnSumInFirstBay;
            xAdjust = -6;
            y = bayIndex * 16 - 72;//纵向
        }
        else if (bayIndex == BaySum - 1)
        {
            //最后一个bay
            columnSum = ColumnSumInLastBay;
            xAdjust = -6;
            y = bayIndex * 16 - 42;//纵向
        }
        else
        {
            //中间的Bay
            columnSum = ColumnSum;
            xAdjust = -11;
            y = bayIndex * 15.5f - 65;//纵向
        }
        for (int row = 0; row < RowSum; row++)
        {
            float z = row * 2.5f;//高度
            for (int column = 0; column < columnSum; column++)
            {
                int containerIndex = Random.Range(0, 6);
                GameObject container = Instantiate(m_AssetContainers[containerIndex]) as GameObject;
                container.name = string.Format("{0:D3}{1:D3}{2:D3}", bayIndex + 1, row + 1, column + 1);
                container.transform.SetParent(this.transform, true);
                //x:2.5,y:,z:2.5
                float x = column * 2.5f + xAdjust;//横向
                container.transform.localPosition = new Vector3(x, y, z);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
