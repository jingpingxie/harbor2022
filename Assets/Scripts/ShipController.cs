using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public int BaySum = 9;
    public int RowSum = 4;
    public int ColumnSum = 10;
    public int ColumnSumInFirstBay = 6;
    public int ColumnSumInLastBay = 6;

    static ContainerFactory _containerFactory;
    ShipPlan shipPlan;
    // Start is called before the first frame update
    void Start()
    {
        _containerFactory = new ContainerFactory();
        shipPlan = new ShipPlan();
        LoadContainer();
    }
    void LoadContainer()
    {
        //从数据库获取集装箱列表
        List<Container> containerList = shipPlan.GetPlanFromDb(this.name);
        if (containerList.Count == 0)
        {
            //如果无法获取，则动态生成集装箱列表
            containerList = shipPlan.Plan(this.name, BaySum, RowSum, ColumnSum, ColumnSumInFirstBay, ColumnSumInLastBay);
        }
        foreach (Container container in containerList)
        {
            GameObject containerGameObject = Object.Instantiate(_containerFactory.GetAssetContainer(container.ContainerType)) as GameObject;
            containerGameObject.name = container.Name;
            containerGameObject.transform.SetParent(this.transform, true);
            containerGameObject.transform.localPosition = new Vector3(container.X, container.Y, container.Z);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
