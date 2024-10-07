using Assets;
using Unity.VisualScripting;
using UnityEngine;

public class CraneController : MonoBehaviour
{

    public float moveSpeed = 5f; //移动速度
    private Transform containerOriginTransform; //集装箱原始位置
    private GameObject elevator;
    private GameObject chain;
    private GameObject hook;

    private CraneActionState moveState = CraneActionState.MoveCrane;
    float chainOriginLength;
    const float CONTAINER_HEIGHT = 4.5f;

    private Transform truckTransform;

    GameObject container;

    GameObject truck;

    void Start()
    {
        elevator = GameObject.Find("Elevator");
        chain = GameObject.Find("Chain");
        hook = GameObject.Find("Hook");
        container = GameObject.Find("Port-Container_SHIP1/Port-container_38");
        containerOriginTransform = container.transform;
        truck = GameObject.Find("HG0701");
        truckTransform = truck.transform;

        System.Console.WriteLine("start");
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
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, containerOriginTransform.position.z + 0.55f);//加上点误差
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        if (Mathf.Abs(transform.position.z - targetPos.z) < 0.01f)
        {
            System.Console.WriteLine("move x");
            moveState = CraneActionState.MoveSlide;
        }
    }

    private void MoveSlide()
    {
        Transform elevatorTransform = elevator.transform;
        Vector3 targetPos = new Vector3(containerOriginTransform.position.x, elevatorTransform.position.y, elevatorTransform.position.z);
        float step = moveSpeed * Time.deltaTime;
        elevatorTransform.position = Vector3.MoveTowards(elevatorTransform.position, targetPos, step);
        if (Mathf.Abs(elevatorTransform.position.x - targetPos.x) < 0.1f)
        {
            System.Console.WriteLine("move down");
            moveState = CraneActionState.HookDown;
            Transform chainTransform = chain.transform;
            Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
            chainOriginLength = renderBounds.size.y;
        }
    }

    private void HookDown()
    {
        float moveDistance = moveSpeed * Time.deltaTime / 2;
        Transform chainTransform = chain.transform;

        Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
        float scale = (renderBounds.size.y + moveDistance) / chainOriginLength;
        chainTransform.localScale = new Vector3(chainTransform.localScale.x, chainTransform.localScale.y, scale);

        Transform hookTransform = hook.transform;
        hookTransform.position = new Vector3(hookTransform.position.x, hookTransform.position.y - moveDistance, hookTransform.position.z);

        float distance = hookTransform.position.y - (containerOriginTransform.position.y + CONTAINER_HEIGHT);
        if (distance <= 0.0f)
        {
            System.Console.WriteLine("lift up");
            moveState = CraneActionState.LiftUp;
        }
    }

    private void LiftUp()
    {
        float moveDistance = moveSpeed * Time.deltaTime / 2;
        Transform chainTransform = chain.transform;

        Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
        float scale = (renderBounds.size.y - moveDistance) / chainOriginLength;
        chainTransform.localScale = new Vector3(chainTransform.localScale.x, chainTransform.localScale.y, scale);

        Transform hookTransform = hook.transform;
        hookTransform.position = new Vector3(hookTransform.position.x, hookTransform.position.y + moveDistance, hookTransform.position.z);

        Transform containerTransform = containerOriginTransform;
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
        Transform elevatorTransform = elevator.transform;
        Vector3 targetPos = new Vector3(truckTransform.position.x, elevatorTransform.position.y, elevatorTransform.position.z);
        float step = moveSpeed * Time.deltaTime;
        elevatorTransform.position = Vector3.MoveTowards(elevatorTransform.position, targetPos, step);

        Transform containerTransform = containerOriginTransform;
        containerTransform.position = new Vector3(elevatorTransform.position.x, containerTransform.position.y, containerTransform.position.z);


        if (Mathf.Abs(elevatorTransform.position.x - targetPos.x) < 0.1f)
        {
            System.Console.WriteLine("lift down");
            moveState = CraneActionState.LiftDown;

            //send start run event to truck
        }
    }

    private void LiftDown()
    {
        float moveDistance = moveSpeed * Time.deltaTime / 2;
        Transform chainTransform = chain.transform;

        Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
        float scale = (renderBounds.size.y + moveDistance) / chainOriginLength;
        chainTransform.localScale = new Vector3(chainTransform.localScale.x, chainTransform.localScale.y, scale);

        Transform hookTransform = hook.transform;
        hookTransform.position = new Vector3(hookTransform.position.x, hookTransform.position.y - moveDistance, hookTransform.position.z);

        Transform containerTransform = containerOriginTransform;
        containerTransform.position = new Vector3(containerTransform.position.x, containerTransform.position.y - moveDistance, containerTransform.position.z);

        float distance = containerOriginTransform.position.y - truckTransform.position.y - 1.4f;
        if (distance <= 0.0f)
        {
            System.Console.WriteLine("move back");
            moveState = CraneActionState.MoveBack;
            container.transform.SetParent(truckTransform);
        }
    }

    private void MoveBack()
    {
        float moveDistance = moveSpeed * Time.deltaTime / 2;
        Transform chainTransform = chain.transform;

        Bounds renderBounds = GameObjectUtils.GetCombinedRenderBounds(chainTransform);
        float scale = (renderBounds.size.y - moveDistance) / chainOriginLength;
        chainTransform.localScale = new Vector3(chainTransform.localScale.x, chainTransform.localScale.y, scale);

        Transform hookTransform = hook.transform;
        hookTransform.position = new Vector3(hookTransform.position.x, hookTransform.position.y + moveDistance, hookTransform.position.z);

        if (chainTransform.localScale.z <= 1.0f)
        {
            System.Console.WriteLine("stop");
            moveState = CraneActionState.Stop;
        }
    }
}
