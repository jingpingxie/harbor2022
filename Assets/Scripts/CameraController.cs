using UnityEngine;

//W����ǰ��S������A������D�����ң�Q�����½���E��������������Ҽ�������ת�������֡�������
public class CameraController : MonoBehaviour
{
    public float speed = 50.0f; // �ƶ��ٶ�
    public float rotationSpeed = 2.0f; // ��ת�ٶ�
    public float shiftRate = 2.0f;// ��סShift����
    public float minDistance = 0.5f;// ����벻�ɴ����ı������С���루С�ڵ���0ʱ�ɴ�͸�κα��棩

    void Update()
    {
        //// �ƶ�
        //float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        //float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        //transform.Translate(moveX, 0, moveZ);

        float moveSpeed;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //�����ƶ�
            moveSpeed = speed * shiftRate;
        }
        else {
            moveSpeed = speed;
        }
        //����/����
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

        // ��ת
        float rotateX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotateY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(0, -rotateX, 0);
        Camera.main.transform.Rotate(rotateY, 0, 0);

        //// �����������������ת�Ƕ�
        //float rotationY = Camera.main.transform.eulerAngles.y;
        //if (rotationY < 45 || rotationY > 135)
        //{
        //    Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
        //}

        #region �����ת
        if (Input.GetMouseButton(1))
        {
            // ת�������
            this.transform.RotateAround(this.transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime);
            this.transform.RotateAround(this.transform.position, this.transform.right, -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime);
            //// ת�˶��ٶȷ���
            //direction = V3RotateAround(direction, Vector3.up, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime);
            //direction = V3RotateAround(direction, this.transform.right, -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime);
        }
        #endregion
    }
    public Vector3 V3RotateAround(Vector3 source, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);// ��תϵ��
        return q * source;// ����Ŀ���
    }
}