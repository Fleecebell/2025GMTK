using UnityEngine;
using InventorySystem.Items;

namespace InventorySystem.Managers
{
    /// <summary>
    /// 武器实例管理器
    /// 负责武器的实例化、初始化和销毁
    /// </summary>
    public class WeaponInstanceManager : MonoBehaviour
    {
        [Header("武器挂载设置")]
        [SerializeField] private Transform weaponHolder;        // 武器挂载点
        [SerializeField] private Transform playerTransform;     // 玩家Transform
        
        // 当前武器实例
        private GameObject currentWeaponInstance;
        private Weapon currentWeaponScript;
        private WeaponData currentWeaponData;

        // 事件
        public System.Action<Weapon> OnWeaponInstanceCreated;
        public System.Action OnWeaponInstanceDestroyed;

        // 属性访问器
        public GameObject CurrentWeaponInstance => currentWeaponInstance;
        public Weapon CurrentWeaponScript => currentWeaponScript;
        public WeaponData CurrentWeaponData => currentWeaponData;
        public bool HasWeaponEquipped => currentWeaponInstance != null;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            InitializeWeaponHolder();
        }

        /// <summary>
        /// 初始化武器挂载点
        /// </summary>
        private void InitializeWeaponHolder()
        {
            // 如果没有设置武器挂载点，创建一个
            if (weaponHolder == null)
            {
                GameObject holderObj = new GameObject("WeaponHolder");
                holderObj.transform.SetParent(transform);
                holderObj.transform.localPosition = Vector3.zero;
                holderObj.transform.localRotation = Quaternion.identity;
                weaponHolder = holderObj.transform;
            }

            // 如果没有设置玩家Transform，使用当前对象
            if (playerTransform == null)
            {
                playerTransform = transform;
            }
        }

        /// <summary>
        /// 创建武器实例
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        /// <returns>是否创建成功</returns>
        public bool CreateWeaponInstance(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogError("武器数据不能为空");
                return false;
            }

            // 销毁当前武器实例
            DestroyCurrentWeaponInstance();

            // 创建新武器实例
            if (weaponData.attackInstance != null)
            {
                // 使用武器数据中的预制体
                currentWeaponInstance = Instantiate(weaponData.attackInstance, weaponHolder);
            }
            else
            {
                // 如果没有预制体，创建一个基础武器对象
                currentWeaponInstance = CreateBasicWeaponInstance(weaponData);
            }

            if (currentWeaponInstance == null)
            {
                Debug.LogError($"无法创建武器实例: {weaponData.ItemName}");
                return false;
            }

            // 获取或添加Weapon脚本
            currentWeaponScript = currentWeaponInstance.GetComponent<Weapon>();
            if (currentWeaponScript == null)
            {
                currentWeaponScript = currentWeaponInstance.AddComponent<Weapon>();
            }

            // 初始化武器脚本
            currentWeaponScript.Initialize(weaponData, playerTransform);
            Debug.Log($"武器实例初始化成功: {weaponData.ItemName}");
            currentWeaponData = weaponData;

            // 设置武器位置和父级
            currentWeaponInstance.transform.SetParent(weaponHolder);
            currentWeaponInstance.transform.localPosition = Vector3.zero;
            currentWeaponInstance.transform.localRotation = Quaternion.identity;

            // 触发事件
            OnWeaponInstanceCreated?.Invoke(currentWeaponScript);

            Debug.Log($"武器实例创建成功: {weaponData.ItemName}");
            return true;
        }

        /// <summary>
        /// 创建基础武器实例
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        /// <returns>武器实例</returns>
        private GameObject CreateBasicWeaponInstance(WeaponData weaponData)
        {
            GameObject weaponObj = new GameObject($"Weapon_{weaponData.ItemName}");
            
            // 添加SpriteRenderer
            SpriteRenderer spriteRenderer = weaponObj.AddComponent<SpriteRenderer>();
            if (weaponData.Icon != null)
            {
                spriteRenderer.sprite = weaponData.Icon;
            }

            // 设置渲染层级
            spriteRenderer.sortingOrder = 4;

            // 添加AudioSource
            AudioSource audioSource = weaponObj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            return weaponObj;
        }

        /// <summary>
        /// 销毁当前武器实例
        /// </summary>
        public void DestroyCurrentWeaponInstance()
        {
            if (currentWeaponInstance != null)
            {
                string weaponName = currentWeaponData?.ItemName ?? "未知武器";
                
                // 触发事件
                OnWeaponInstanceDestroyed?.Invoke();

                // 销毁实例
                Destroy(currentWeaponInstance);
                currentWeaponInstance = null;
                currentWeaponScript = null;
                currentWeaponData = null;

                Debug.Log($"武器实例已销毁: {weaponName}");
            }
        }

        /// <summary>
        /// 更新武器属性
        /// </summary>
        /// <param name="newDamage">新伤害值</param>
        /// <param name="newAttackSpeed">新攻击速度</param>
        public void UpdateWeaponStats(float newDamage, float newAttackSpeed)
        {
            if (currentWeaponScript != null)
            {
                currentWeaponScript.SetWeaponStats(newDamage, newAttackSpeed);
            }
        }

        /// <summary>
        /// 设置武器挂载点
        /// </summary>
        /// <param name="newHolder">新的挂载点</param>
        public void SetWeaponHolder(Transform newHolder)
        {
            if (newHolder == null) return;

            weaponHolder = newHolder;

            // 如果有当前武器实例，移动到新的挂载点
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.transform.SetParent(weaponHolder);
                currentWeaponInstance.transform.localPosition = Vector3.zero;
                currentWeaponInstance.transform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// 获取武器状态信息
        /// </summary>
        /// <returns>状态信息</returns>
        public string GetWeaponInstanceStatus()
        {
            if (!HasWeaponEquipped)
                return "未装备武器";

            return currentWeaponScript?.GetWeaponStatus() ?? "武器脚本未找到";
        }

        /// <summary>
        /// 手动触发攻击
        /// </summary>
        public void TriggerAttack()
        {
            if (currentWeaponScript != null)
            {
                currentWeaponScript.TryAttack();
            }
        }

        /// <summary>
        /// 检查是否可以攻击
        /// </summary>
        /// <returns>是否可以攻击</returns>
        public bool CanAttack()
        {
            return currentWeaponScript?.CanAttack ?? false;
        }

        /// <summary>
        /// 调试用：在Inspector中显示武器信息
        /// </summary>
        [ContextMenu("显示武器状态")]
        public void ShowWeaponStatus()
        {
            Debug.Log(GetWeaponInstanceStatus());
        }

        /// <summary>
        /// 调试用：测试攻击
        /// </summary>
        [ContextMenu("测试攻击")]
        public void TestAttack()
        {
            TriggerAttack();
        }

        /// <summary>
        /// 清理
        /// </summary>
        private void OnDestroy()
        {
            DestroyCurrentWeaponInstance();
        }
    }
}