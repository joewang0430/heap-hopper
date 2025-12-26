using NUnit.Framework;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{

    // 1. 定义一个变量来存放 Rigidbody 组件的引用
    private Rigidbody rb;

    [Header("移动设置")]
    // 2. 定义跳跃的力量大小（我可以随时回来改这个数字）
    public float jumpForce = 6f;  // 跳跃的力量
    public float moveSpeed = 10f;   // 跳跃的力量

    [Header("状态监控")]
    // private bool isGrounded = true;
    private bool isGrounded;
    public float groundCheckDistance = 0.2f; // 雷达探测距离
    public LayerMask groundLayer; // 告诉雷达哪些东西算“地面”

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 3. 在游戏开始时，获取挂在同一个物体上的 Rigidbody 组件
        // 这相当于把代码里的 rb 变量和 Editor 里的组件“连上线”
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        // --- 核心修改：主动探测地面 ---
        // 从球心向下发射一个虚构的“小球探头”
        // 只要这个探头碰到了东西，就认为接地了
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.5f, 0.2f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // Debug.Log("跑跳起飞！");
        }
    }

    void FixedUpdate()
    {
        // 1. 获取输入轴数值 (Input Axes)
        // Horizontal 代表键盘 A/D 或 左右方向键 (返回 -1 到 1 之间的数值)
        float moveHorizontal = Input.GetAxis("Horizontal");
        // Vertical 代表键盘 W/S 或 上下方向键 (返回 -1 到 1 之间的数值)
        float moveVertical = Input.GetAxis("Vertical");

        // 2. 获取相机在水平面上的方向（去掉 Y 轴，防止球往地里钻或往天上飞）
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0; // 关键：抹掉高度差
        camForward.Normalize(); // 归一化，变成长度为 1 的方向向量

        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        // 3. 计算最终的移动力方向
        // 现在的“前”是相机看的方向，“右”是相机的右侧
        Vector3 movement = (camForward * moveVertical + camRight * moveHorizontal);

        // 4. 施加推力
        rb.AddForce(movement * moveSpeed);
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // 教学点：使用 && (逻辑与) 符号
    //     // 只有当“按下空格” 并且 “在地面上” 这两个条件同时满足时，才执行跳跃
    //     if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
    //     {
    //         Debug.Log("Jump!");

    //         // 4. 关键指令：给刚体施加一个向上的力
    //         // Vector3.up 等于 (0, 1, 0)
    //         // ForceMode.Impulse 表示这是一个瞬间的冲力（就像踢球一样）
    //         rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    //         // 一旦跳起来，立刻标记为“不在地面上”
    //         isGrounded = false; 
    //         Debug.Log("在地面起跳！");
    //     }
    // }

    // void FixedUpdate()
    // {
    //     // 2. 获取输入轴数值 (Input Axes)
    //     // Horizontal 代表键盘 A/D 或 左右方向键 (返回 -1 到 1 之间的数值)
    //     float moveHorizontal = Input.GetAxis("Horizontal");
    //     // Vertical 代表键盘 W/S 或 上下方向键 (返回 -1 到 1 之间的数值)
    //     float moveVertical = Input.GetAxis("Vertical");

    //     // 3. 构建力向量
    //     // x轴对应左右，z轴对应前后 (y轴是上下，这里不加力)
    //     Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

    //     // 4. 施加持续推力
    //     // 乘以 moveSpeed 控制速度，乘以 Time.fixedDeltaTime 保证不同电脑速度一致
    //     rb.AddForce(movement * moveSpeed);
    // }

    // // 教学点：这是 Unity 内置的事件函数，当球撞到任何东西时，引擎会自动调用它
    // void OnCollisionEnter(Collision collision)
    // {
    //     // 只要撞到了东西（比如地面），就认为落地了
    //     isGrounded = true;
    //     Debug.Log("碰到地面，恢复跳跃能力");
    // }

    // void OnCollisionExit(Collision collision)
    // {
    //     // 离开地面时（比如掉下悬崖），也要失去跳跃能力
    //     isGrounded = false;
    //     Debug.Log("离开表面");
    // }
}
