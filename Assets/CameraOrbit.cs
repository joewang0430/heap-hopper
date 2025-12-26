using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // 盯着球
    private float mouseSensitivity = 5f; // 鼠标灵敏度
    private float rotationX = 0f; // 左右旋转角度
    private float rotationY = 45f; // 上下旋转角度（初始稍微低头看球）
    private float distance = 20f; // 相机离球的距离


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 锁定鼠标：点击 Play 后鼠标会消失，防止移出窗口，按 Esc 键可以找回鼠标
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 注意：相机必须在 LateUpdate 执行，确保球动完了相机再跟
    void LateUpdate()
    {
        // 1. 获取鼠标移动
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 限制上下旋转角度，防止相机翻转（比如不能从脚底看，也不能翻到头顶后面去）
        rotationY = Mathf.Clamp(rotationY, 10f, 80f);

        // 2. 计算相机的旋转角度
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // 3. 计算相机位置：球的位置 - (旋转方向 * 距离)
        // 这行数学逻辑会让相机始终保持在球的“后面”
        transform.position = target.position - (rotation * Vector3.forward * distance);

        // 4. 始终盯着球看
        transform.LookAt(target.position);
    }
}
