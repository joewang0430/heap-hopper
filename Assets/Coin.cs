// Coin.cs

using UnityEngine;

public class Coin : MonoBehaviour
{
    // 增加一个状态位，防止重复通知
    private bool hasNotified = false;

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("碰到了东西：" + other.name);
        // 1. 检查撞到的是不是玩家（利用之前设好的 Tag）
        if (other.CompareTag("Player"))
        {
            hasNotified = true; // 标记为已被正常收集

            // 2. 调用单例，增加分数
            GameManager.Instance.AddScore(10);

            // 3. 通知管理器：我被吃了，请在 0.5s 后刷新新的
            GameManager.Instance.NotifyCoinCollected();

            // 4. 销毁自己（释放内存）
            Destroy(gameObject);
        }
    }

    // --- 硬核补丁：临终遗言 ---
    private void OnDestroy()
    {
        // 如果金币在被销毁前还没来得及通知“被吃掉”（说明是随地砖掉落了）
        // 且游戏还没结束，且单例还在
        if (!hasNotified && GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            // 依然通知管理器：场上空出一个位子，请准备重生
            GameManager.Instance.NotifyCoinCollected();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 每一帧绕 Y 轴旋转 90 度 * 时间增量
        transform.Rotate(Vector3.up * 90f * Time.deltaTime, Space.Self);
    }
}
