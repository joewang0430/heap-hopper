using JetBrains.Annotations;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 在 Inspector 面板里把 Sphere 拖到这个变量上
    public Transform target;
    private Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 游戏开始瞬间，计算相机相对于球的偏移量
        // 偏移量 = 相机位置 - 球位置
        offset = transform.position - target.position;

    }

    // Update is called once per frame
    void Update()
    {   
        // 每一帧，强制相机的位置 = 球当前位置 + 那个固定的偏移量
        transform.position = target.position + offset;
    }
}
