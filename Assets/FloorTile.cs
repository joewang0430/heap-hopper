// FloorTile.cs

using System.Collections;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    // 引用组件：在堆内存中预留两个指针槽位
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    
    private Vector3 targetScale;    // 用于存储地砖“原本大小”的变量 (在堆栈上存储 3 个 float)
    private float spawnDuration = 0.5f; // 动画持续时间

    void Awake()
    {
        // 1.【关键】在一切开始前，先记住 Prefab 设定好的目标大小
        targetScale = transform.localScale;

        // 2.【关键】立即把自己缩放到 0（在玩家还没看见的第一帧就执行）
        transform.localScale = Vector3.zero;

        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 3. 出生时，启动“生长”动画协程
        StartCoroutine(SpawnAnimation());
    }

    IEnumerator SpawnAnimation()
    {
        float timer = 0f;

        // 只要计时器没到时间，就一直循环
        while (timer < spawnDuration)
        {
            // 每一帧增加经过的时间
            timer += Time.deltaTime;

            // 计算进度条 (0.0 到 1.0 之间)
            float progress = timer / spawnDuration;

            // 【核心算法】线性插值 (Lerp - Linear Interpolation)
            // 意思：在 Vector3.zero 和 targetScale 之间，按照 progress 的比例取一个中间值
            // 比如 progress 是 0.5 时，就取一半大小。
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);

            // 等待下一帧再继续循环
            yield return null;
        }

        // 确保循环结束后，大小是精确的目标大小（防止浮点数误差）
        transform.localScale = targetScale;
    }

    public void StartFalling(float delay)
    {
        StartCoroutine(FallSequence(delay));
    }

    IEnumerator FallSequence(float delay)
    {
        // 1. 变红
        meshRenderer.material.color = Color.red;

        // 2. 暂停执行（释放控制权给引擎）
        // 引擎会在每一帧检查：delay 时间到了吗？没到就跳过，到了就恢复执行下一行
        yield return new WaitForSeconds(delay);

        // 3. 恢复执行：物理掉落
        rb.isKinematic = false;

        // 4. 等待 3 秒后销毁
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
