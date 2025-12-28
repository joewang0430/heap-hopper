// GridManager.cs

using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    // 在 Inspector 窗口把刚才做好的 Prefab 拖到这个槽位
    public GameObject tilePrefab;
    public int rows = 5;    // 行数
    public int cols = 5;    // 列数
    public float spacing = 4.0f; // 地砖中心点之间的间距

    void Awake() {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < cols; z++)
            {
                // 1. 计算每一块地砖在世界坐标系中的位置，-8是因为原来的墙和球的位置有些偏移
                Vector3 spawnPos = new Vector3(x * spacing - 8, 0, z * spacing - 8);

                // 2. 实例化 (关键动作)：这相当于在堆(Heap)上申请一块内存，
                // 复制 Prefab 模板的内容，并返回该实体的地址
                GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);

                // 3. 注册：把这个新诞生的地砖地址，交给全局裁判
                FloorTile tileScript = newTile.GetComponent<FloorTile>();
                if (tileScript != null)
                {
                    // 顺着静态区的 Instance 指针找到 GameManager，并登记这块砖
                    GameManager.Instance.RegisterTile(tileScript);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
