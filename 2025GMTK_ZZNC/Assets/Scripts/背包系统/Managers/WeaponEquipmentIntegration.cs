using UnityEngine;
using UnityEngine.SceneManagement;
using InventorySystem.Items;

namespace InventorySystem.Managers
{
    public class WeaponEquipmentIntegration : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private EquipmentManager equipmentManager;
        [SerializeField] private WeaponInstanceManager weaponInstanceManager;
        [SerializeField] private ZZNC_Player playerController;

        private void Awake()
        {
            // 监听场景加载完成事件
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            InitializeIntegration();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 在新场景加载完成后重新初始化
            InitializeIntegration();
        }

        private void InitializeIntegration()
        {
            // 获取组件引用
            if (equipmentManager == null)
                equipmentManager = GetComponent<EquipmentManager>();

            if (weaponInstanceManager == null)
                weaponInstanceManager = GetComponent<WeaponInstanceManager>();

            // 自动查找玩家控制器
            if (playerController == null)
            {
                playerController = FindObjectOfType<ZZNC_Player>();
                if (playerController == null)
                {
                    Debug.LogError("未找到玩家控制器！");
                }
            }

            // 绑定事件
            if (equipmentManager != null)
            {
                equipmentManager.OnWeaponEquipped += OnWeaponEquipped;
                equipmentManager.OnWeaponUnequipped += OnWeaponUnequipped;
            }
        }

        private void OnWeaponEquipped(WeaponData weaponData)
        {
            if (weaponInstanceManager != null)
            {
                // 创建武器实例
                bool success = weaponInstanceManager.CreateWeaponInstance(weaponData);

                if (success)
                {
                    Debug.Log($"武器实例创建成功: {weaponData.ItemName}");
                    playerController.SetPlayerWeapon(weaponInstanceManager.CurrentWeaponInstance, weaponInstanceManager.CurrentWeaponScript);
                }
                else
                {
                    Debug.LogError($"武器实例创建失败: {weaponData.ItemName}");
                }
            }
        }

        private void OnWeaponUnequipped(WeaponData weaponData)
        {
            if (weaponInstanceManager != null)
            {
                // 销毁武器实例
                weaponInstanceManager.DestroyCurrentWeaponInstance();
                Debug.Log($"武器实例已销毁: {weaponData.ItemName}");
                playerController.SetPlayerWeapon(null);
            }
        }

        private void OnDestroy()
        {
            // 清理事件绑定
            if (equipmentManager != null)
            {
                equipmentManager.OnWeaponEquipped -= OnWeaponEquipped;
                equipmentManager.OnWeaponUnequipped -= OnWeaponUnequipped;
            }

            // 移除场景加载完成事件监听
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}