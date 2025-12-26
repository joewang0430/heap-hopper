using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform playerTransform; // 玩家的位置引用
    public float speed = 10f;         // 追逐速度
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 如果你没在面板拖入玩家，代码自动去场景里找带有 "Player" 标签的物体
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        // --- 核心算法：向量减法 ---
        // 目标位置 - 当前位置 = 从当前指向目标的向量
        Vector3 direction = (playerTransform.position - transform.position);

        // 只关心方向，不希望离得越远力越大，所以需要归一化 (Normalize)
        direction.y = 0; // 忽略高度差，防止敌人想“飞”起来
        Vector3 forceDirection = direction.normalized;
        // 施加持续的力
        rb.AddForce(forceDirection * speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
