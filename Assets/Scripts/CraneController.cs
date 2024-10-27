using Assets;
using Unity.VisualScripting;
using UnityEngine;

public class CraneController : MonoBehaviour
{

    public float MoveSpeed = 5f; //移动速度
    private Transform _containerOriginTransform; //集装箱原始位置
    private GameObject _elevator;
    private GameObject _chain;
    private GameObject _hook;

    private CraneActionState moveState = CraneActionState.Unkown;
    float _chainOriginLength;
    const float CONTAINER_HEIGHT = 4.5f;

    private Transform _truckTransform;
    GameObject _loadedContainer;
    GameObject _truck;
    TruckDrive _currentCarDrive;

    void Start()
    {
        _elevator = GameObject.Find("Elevator1");
        _chain = GameObject.Find("Chain1");
        _hook = GameObject.Find("Hook1");
        System.Console.WriteLine("start");


        TruckDrive carDrive = FindObjectOfType<TruckDrive>();
        // 订阅事件，并指定事件处理方法
        carDrive.LoadContainerFromShipToTruckNotify += LoadContainerFromShipToTruckNotified;
    }

    private void LoadContainerFromShipToTruckNotified(TruckDrive carDrive, GameObject truck, GameObject loadedContainer, int param)
    {
        if (this.moveState == CraneActionState.Unkown)
        {
            this.moveState = CraneActionState.MoveCrane;
            _currentCarDrive = carDrive;
            _loadedContainer = loadedContainer;
            _containerOriginTransform = _loadedContainer.transform;
            _truck = truck;
            _truckTransform = _truck.transform;
        }
    }

    void Update()
    {
        switch (moveState)
        {
            case CraneActionState.MoveCrane:
                MoveCrane();
                break;
            case CraneActionState.MoveSlide:
                MoveSlide();
                break;
            case CraneActionState.HookDown:
                HookDown();
                break;
            case CraneActionState.LiftUp:
                LiftUp();
                break;
            case CraneActionState.LiftMove:
                LiftMove();
                break;
            case CraneActionState.LiftDown:
                LiftDown();
                break;
            case CraneActionState.MoveBack:
                MoveBack();
                break;
            default:
                break;
        }
    }


    private void MoveCrane()
    {
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, _containerOriginTransform.position.z + 0.55f);//加上点误差
        float step = MoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        if (Mathf.Abs(transform.position.z - targetPos.z) < 0.01f)
        {
            System.Console.WriteLine("move x");
            moveState = CraneActionState.MoveSlide;
        }
    }

    private void MoveSlide()
    {
        Transform elevatorTransform = _elevator.transform;
        Vector3 targetPos = new Vector3(_containerOriginTransform.position.x, elevatorTransform.position.y, elevatorTransform.position.z);
        float step = MoveSpeed * Time.deltaTime;
        elevatorTransform.position = Vector3.MoveTowards(elevatorTransform.position, targetPos, step);
        if (Mathf.Abs(elevatorTransform.position.x - targetPos.x) < 0.1f)
        {
            System.Console.WriteLine("move down");
            moveState = CraneActionState.HookDown;
            Transform chainTransform = _chain.transform;
            Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
            _chainOriginLength = renderBounds.size.y;
        }
    }

    private void HookDown()
    {
        float moveDistance = MoveSpeed * Time.deltaTime / 2;
        Transform chainTransform = _chain.transform;

        Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
        float scale = (renderBounds.size.y + moveDistance) / _chainOriginLength;
        chainTransform.localScale = new Vector3(chainTransform.localScale.x, chainTransform.localScale.y, scale);

        Transform hookTransform = _hook.transform;
        hookTransform.position = new Vector3(hookTransform.position.x, hookTransform.position.y - moveDistance, hookTransform.position.z);

        float distance = hookTransform.position.y - (_containerOriginTransform.position.y + CONTAINER_HEIGHT);
        if (distance <= 0.0f)
        {
            System.Console.WriteLine("lift up");
            moveState = CraneActionState.LiftUp;
        }
    }

    private void LiftUp()
    {
        float moveDistance = MoveSpeed * Time.deltaTime / 2;
        Transform chainTransform = _chain.transform;

        Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
        float scale = (renderBounds.size.y - moveDistance) / _chainOriginLength;
        chainTransform.localScale = new Vector3(chainTransform.localScale.x, chainTransform.localScale.y, scale);

        Transform hookTransform = _hook.transform;
        hookTransform.position = new Vector3(hookTransform.position.x, hookTransform.position.y + moveDistance, hookTransform.position.z);

        Transform containerTransform = _containerOriginTransform;
        containerTransform.position = new Vector3(containerTransform.position.x, containerTransform.position.y + moveDistance, containerTransform.position.z);


        //float distance = hookTransform.position.y - (target.position.y + CONTAINER_HEIGHT);
        if (scale <= 1.0f)
        {
            System.Console.WriteLine("lift move");
            moveState = CraneActionState.LiftMove;
        }
    }

    private void LiftMove()
    {
        Transform elevatorTransform = _elevator.transform;
        Vector3 targetPos = new Vector3(Mathf.Min(_truckTransform.position.x, 8), elevatorTransform.position.y, elevatorTransform.position.z);
        float step = MoveSpeed * Time.deltaTime;

        elevatorTransform.position = Vector3.MoveTowards(elevatorTransform.position, targetPos, step);

        Transform containerTransform = _containerOriginTransform;
        containerTransform.position = new Vector3(elevatorTransform.position.x, containerTransform.position.y, containerTransform.position.z);

        if (Mathf.Abs(elevatorTransform.position.x - _truckTransform.position.x) < 0.1f)
        {
            System.Console.WriteLine("lift down");
            moveState = CraneActionState.LiftDown;

            //send start run event to truck
        }
    }

    private void LiftDown()
    {
        float moveDistance = MoveSpeed * Time.deltaTime / 2;
        Transform chainTransform = _chain.transform;

        Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
        float scale = (renderBounds.size.y + moveDistance) / _chainOriginLength;
        chainTransform.localScale = new Vector3(chainTransform.localScale.x, chainTransform.localScale.y, scale);

        Transform hookTransform = _hook.transform;
        hookTransform.position = new Vector3(hookTransform.position.x, hookTransform.position.y - moveDistance, hookTransform.position.z);

        Transform containerTransform = _containerOriginTransform;
        containerTransform.position = new Vector3(containerTransform.position.x, containerTransform.position.y - moveDistance, containerTransform.position.z);

        float distance = _containerOriginTransform.position.y - _truckTransform.position.y - 1.4f;
        if (distance <= 0.0f)
        {
            System.Console.WriteLine("move back");
            moveState = CraneActionState.MoveBack;
            _loadedContainer.transform.SetParent(_truckTransform);
            if (null != _currentCarDrive) {
                //集装箱已经装载到了集卡上
                _currentCarDrive.SetContainerLoadEnd(_loadedContainer);
            }
        }
    }

    private void MoveBack()
    {
        float moveDistance = MoveSpeed * Time.deltaTime / 2;
        Transform chainTransform = _chain.transform;

        Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
        float scale = (renderBounds.size.y - moveDistance) / _chainOriginLength;
        chainTransform.localScale = new Vector3(chainTransform.localScale.x, chainTransform.localScale.y, scale);

        Transform hookTransform = _hook.transform;
        hookTransform.position = new Vector3(hookTransform.position.x, hookTransform.position.y + moveDistance, hookTransform.position.z);

        if (chainTransform.localScale.z <= 1.0f)
        {
            System.Console.WriteLine("stop");
            moveState = CraneActionState.Stop;
        }
    }
}
