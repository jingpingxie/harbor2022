using UnityEngine;

//W——前；S——后；A——左；D——右；Q——下降；E——上升；鼠标右键——旋转；鼠标滚轮——放缩
public class CameraController : MonoBehaviour
{
    public float speed = 50.0f; // 移动速度
    public float rotationSpeed = 2.0f; // 旋转速度
    public float shiftRate = 2.0f;// 按住Shift加速
    public float minDistance = 0.5f;// 相机离不可穿过的表面的最小距离（小于等于0时可穿透任何表面）

    void Update()
    {
        //// 移动
        //float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        //float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        //transform.Translate(moveX, 0, moveZ);

        float moveSpeed;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //加速移动
            moveSpeed = speed * shiftRate;
        }
        else {
            moveSpeed = speed;
        }
        //上移/下移
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.E))
        {
            transform.Translate(new Vector3(0, 1, 0) * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Q))
        {
            transform.Translate(new Vector3(0, -1, 0) * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-1, 0, 0) * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.W) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Translate(new Vector3(0, 0, 1) * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.S) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.Translate(new Vector3(0, 0, -1) * moveSpeed * Time.deltaTime);
        }

        // 旋转
        float rotateX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotateY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(0, -rotateX, 0);
        Camera.main.transform.Rotate(rotateY, 0, 0);

        //// 限制摄像机的上下旋转角度
        //float rotationY = Camera.main.transform.eulerAngles.y;
        //if (rotationY < 45 || rotationY > 135)
        //{
        //    Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
        //}

        #region 鼠标旋转
        if (Input.GetMouseButton(1))
        {
            // 转相机朝向
            this.transform.RotateAround(this.transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime);
            this.transform.RotateAround(this.transform.position, this.transform.right, -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime);
            //// 转运动速度方向
            //direction = V3RotateAround(direction, Vector3.up, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime);
            //direction = V3RotateAround(direction, this.transform.right, -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime);
        }
        #endregion
    }
    public Vector3 V3RotateAround(Vector3 source, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);// 旋转系数
        return q * source;// 返回目标点
    }
}