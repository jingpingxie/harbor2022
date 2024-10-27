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

    GameObject _container;

    GameObject _truck;
    CarDrive _currentCarDrive;

    void Start()
    {
        _elevator = GameObject.Find("Elevator1");
        _chain = GameObject.Find("Chain1");
        _hook = GameObject.Find("Hook1");
        _container = GameObject.Find("Port-Container_SHIP1/Port-container_38");
        _containerOriginTransform = _container.transform;
        _truck = GameObject.Find("HG0701");
        _truckTransform = _truck.transform;

        System.Console.WriteLine("start");


        CarDrive carDrive = FindObjectOfType<CarDrive>();
        // 订阅事件，并指定事件处理方法
        carDrive.ArrivedChanged += CarDrive_ArrivedChanged;
    }

    private void CarDrive_ArrivedChanged(CarDrive carDrive, int param)
    {
        if (this.moveState == CraneActionState.Unkown)
        {
            this.moveState = CraneActionState.MoveCrane;
            _currentCarDrive = carDrive;
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
            _container.transform.SetParent(_truckTransform);
            if (null != _currentCarDrive) {
                _currentCarDrive.SetContainerLoaded(_container);
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
