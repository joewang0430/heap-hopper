// GameManager.cs

using UnityEngine;
using TMPro;    // 使用 TextMeshPro 必须引用此命名空间
using UnityEngine.SceneManagement;  // 场景管理（重启用）
using System.Collections;          // 必须有这个，为了非泛型的 IEnumerator
using System.Collections.Generic;  // 必须有这个，为了 List<FloorTile>

public class GameManager : MonoBehaviour
{
    // 单例模式：让全场景只有一个 GameManager，且别人都能通过 GameManager.Instance 访问它
    public static GameManager Instance;

    [Header("玩家引用")]
    private Transform playerTransform; // 在这里声明变量，解决“标红”问题

    [Header("地砖设置")]
    // 存储 25 块地砖地址的“容器” (在堆上分配空间)
    private List<FloorTile> allTiles = new List<FloorTile>();
    public float dropInterval = 2.0f; // 掉落间隔，会随时间缩短
    public float respawnDelay = 5.0f; // 地砖掉落后多久重生

    [Header("游戏数据")]
    public int score = 0;
    public float timeLeft = 60f;
    public bool isGameOver = false;

    [Header("UI 引用")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    void Awake()
    {
        // 初始化单例
        if (Instance == null) Instance = this;
    }

    // 由 GridManager 生成地砖后调用，把地砖地址存入列表
    public void RegisterTile(FloorTile tile)
    {
        allTiles.Add(tile);
    }

    void Start()
    {
        // 引擎会扫描堆内存，找到那个 Tag 为 "Player" 的物体，并把它的 Transform 地址存下来
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("Cannot find object with Tag=Player, please chack the scene!");
        }

        StartCoroutine(AutoDropSequence());
    }

    IEnumerator AutoDropSequence()
    {
        // 核心：等待 0.5 秒，确保 GridManager 已经在堆上 malloc 完了 25 块地砖
        yield return new WaitForSeconds(1f);

        // 游戏没结束就一直循环
        while (!isGameOver)
        {
            // 1. 每隔 ~ 秒（你可以根据难度调整这个数字）点名一块砖
            float currentInterval = Mathf.Max(0.5f, dropInterval - (Time.timeSinceLevelLoad / 60f));
            yield return new WaitForSeconds(currentInterval);

            // 2. 执行写的那个随机掉落函数
            DropRandomTile();
        }
    }

    // 核心逻辑：随机选一块砖掉下去
    public void DropRandomTile()
    {
        if (allTiles.Count > 0)
        {
            // 1. 随机获取数组下标
            int index = Random.Range(0, allTiles.Count);
            FloorTile tile = allTiles[index];

            // 2. 记录这块地砖的原始坐标（Value Type 复制，存入栈或闭包）
            Vector3 originalPos = tile.transform.position;

            // 3. 出发掉落逻辑
            tile.StartFalling(2.0f);

            // 4. 从可用列表中移除（因为它快要销毁了）
            allTiles.RemoveAt(index);

            // 5. 开启一个“重生计时器”，把坐标传进去
            StartCoroutine(RespawnTileSequence(originalPos));
        }
    }

    IEnumerator RespawnTileSequence(Vector3 position)
    {
        // 等待地砖销毁并过一段冷却时间
        yield return new WaitForSeconds(respawnDelay);

        if (!isGameOver)
        {
            // 1. 在原位重新申请内存（Instantiate）
            // 注意：这里需要 GridManager 里的那个 tilePrefab，
            // 我们可以在 GameManager 里也引用它，或者通过 GridManager 提供
            GameObject newTile = Instantiate(GridManager.Instance.tilePrefab, position, Quaternion.identity);

            // 2. 重新注册到列表中，使其可以再次被点名掉落
            FloorTile tileScript = newTile.GetComponent<FloorTile>();
            RegisterTile(tileScript);
        }
    }

    void OnDestroy() {
        if (Instance == this) Instance = null; // 离开前把路牌涂掉，防止别人顺着死地址找过来
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            return;
        }

        // 更新倒计时
        UpdateTimer();

        // 死亡判定逻辑：检测玩家实体的 Y 轴坐标
        // 假设玩家球体的变量名是 playerTransform
        if (playerTransform != null && playerTransform.position.y < -10f)
        {
            EndGame();
        }
    }

    void UpdateTimer()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime; // 每一帧减去经过的时间
            timerText.text = "Time: " + Mathf.CeilToInt(timeLeft).ToString();
        }
        else
        {
            EndGame();
        }
    }

    // 供其他脚本（如金币）调用的公共方法
    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        scoreText.text = "Score: " + score;
    }

    public void EndGame()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);  // 显示结束面板
        finalScoreText.text = "Final Score: " + score;

        // 游戏结束时停止时间流速（可选）
        Time.timeScale = 0;
    }

    void RestartGame()
    {
        Time.timeScale = 1; // 恢复时间流速
        // 重新加载当前激活的场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
