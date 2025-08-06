using UnityEngine;
using UnityEngine.SceneManagement; // 添加对场景管理的引用
using InventorySystem.Character;

public class Gua : MonoBehaviour
{
    // 单例模式
    public static Gua Instance { get; private set; }

    // 传送目标位置
    [SerializeField] private Transform playerTransform; // 玩家的 Transform
    public Transform BossRoom; // Boss 房间的 Transform
    public Camera cameraBoss; // Boss 房间的相机
    // Boss 对象
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject MiniMap; // 小地图

    private void Awake()
    {
        // 确保只有一个实例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经存在一个实例，销毁当前实例
        }
    }

    private void Update()
    {
        // 检测按键【R】恢复生命值
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestorePlayerHealth();
        }

        // 检测按键【B】传送玩家并激活 Boss
        if (Input.GetKeyDown(KeyCode.B))
        {
            TeleportPlayerAndActivateBoss();
        }

        // 检测按键【G】加载并进入场景 Fleece_map2
        if (Input.GetKeyDown(KeyCode.G))
        {
            LoadScene("Fleece_map2");
        }
    }

    public void RestorePlayerHealth()
    {
        // 调用玩家管理器的方法来恢复生命值
        CharacterManager.Instance.RestoreHealth();
    }

    private void TeleportPlayerAndActivateBoss()
    {
        // 将玩家设置为 Boss 房间的子对象，并将相对父对象的位置设置为 (0, 0, 0)
        playerTransform.SetParent(BossRoom, true);
        playerTransform.localPosition = Vector3.zero;
        playerTransform.localRotation = Quaternion.identity;

        // 调整相机深度
        cameraBoss.depth = 3;

        // 激活 Boss
        if (boss != null)
        {
            boss.SetActive(true);
            MiniMap.SetActive(false);
            Debug.Log("Boss 已激活！");
        }
        else
        {
            Debug.LogError("Boss 对象未正确引用！");
        }
    }

    private void LoadScene(string sceneName)
    {
        // 加载指定的场景
        SceneManager.LoadScene(sceneName);
    }
}