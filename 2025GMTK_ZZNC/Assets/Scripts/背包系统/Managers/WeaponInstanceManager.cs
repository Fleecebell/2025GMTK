using UnityEngine;
using UnityEngine.SceneManagement;
using InventorySystem.Items;

namespace InventorySystem.Managers
{
    public class WeaponInstanceManager : MonoBehaviour
    {
        [Header("武器挂载设置")]
        [SerializeField] private Transform weaponHolder;        // 武器挂载点
        [SerializeField] private Transform playerTransform;     // 玩家Transform

        private GameObject currentWeaponInstance;
        private Weapon currentWeaponScript;
        private WeaponData currentWeaponData;

        public System.Action<Weapon> OnWeaponInstanceCreated;
        public System.Action OnWeaponInstanceDestroyed;

        public GameObject CurrentWeaponInstance => currentWeaponInstance;
        public Weapon CurrentWeaponScript => currentWeaponScript;
        public WeaponData CurrentWeaponData => currentWeaponData;
        public bool HasWeaponEquipped => currentWeaponInstance != null;

        private void Awake()
        {
            // 监听场景加载完成事件
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitializeWeaponHolder();
            InitializePlayerTransform();
        }

        private void Start()
        {
            // 如果需要在 Start 中执行初始化逻辑，可以在这里调用 InitializeIntegration
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 在新场景加载完成后重新初始化
            InitializeWeaponHolder();
            InitializePlayerTransform();
        }

        private void InitializeWeaponHolder()
        {
            if (weaponHolder == null)
            {
                GameObject holderObj = GameObject.Find("WeaponHolder");
                if (holderObj == null)
                {
                    holderObj = new GameObject("WeaponHolder");
                    holderObj.transform.SetParent(transform);
                    holderObj.transform.localPosition = Vector3.zero;
                    holderObj.transform.localRotation = Quaternion.identity;
                }
                weaponHolder = holderObj.transform;
            }
        }

        private void InitializePlayerTransform()
        {
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.Find("player");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                }
                else
                {
                    Debug.LogError("未找到玩家对象！");
                }
            }
        }

        public bool CreateWeaponInstance(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogError("武器数据不能为空");
                return false;
            }

            DestroyCurrentWeaponInstance();

            if (weaponData.attackInstance != null)
            {
                currentWeaponInstance = Instantiate(weaponData.attackInstance, weaponHolder);
            }
            else
            {
                currentWeaponInstance = CreateBasicWeaponInstance(weaponData);
            }

            if (currentWeaponInstance == null)
            {
                Debug.LogError($"无法创建武器实例: {weaponData.ItemName}");
                return false;
            }

            currentWeaponScript = currentWeaponInstance.GetComponent<Weapon>();
            if (currentWeaponScript == null)
            {
                currentWeaponScript = currentWeaponInstance.AddComponent<Weapon>();
            }

            currentWeaponScript.Initialize(weaponData, playerTransform);
            Debug.Log($"武器实例初始化成功: {weaponData.ItemName}");
            currentWeaponData = weaponData;

            currentWeaponInstance.transform.SetParent(weaponHolder);
            currentWeaponInstance.transform.localPosition = Vector3.zero;
            currentWeaponInstance.transform.localRotation = Quaternion.identity;

            OnWeaponInstanceCreated?.Invoke(currentWeaponScript);

            Debug.Log($"武器实例创建成功: {weaponData.ItemName}");
            return true;
        }

        private GameObject CreateBasicWeaponInstance(WeaponData weaponData)
        {
            GameObject weaponObj = new GameObject($"Weapon_{weaponData.ItemName}");
            SpriteRenderer spriteRenderer = weaponObj.AddComponent<SpriteRenderer>();
            if (weaponData.Icon != null)
            {
                spriteRenderer.sprite = weaponData.Icon;
            }
            spriteRenderer.sortingOrder = 4;
            AudioSource audioSource = weaponObj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            return weaponObj;
        }

        public void DestroyCurrentWeaponInstance()
        {
            if (currentWeaponInstance != null)
            {
                string weaponName = currentWeaponData?.ItemName ?? "未知武器";
                OnWeaponInstanceDestroyed?.Invoke();
                Destroy(currentWeaponInstance);
                currentWeaponInstance = null;
                currentWeaponScript = null;
                currentWeaponData = null;
                Debug.Log($"武器实例已销毁: {weaponName}");
            }
        }

        public void UpdateWeaponStats(float newDamage, float newAttackSpeed)
        {
            if (currentWeaponScript != null)
            {
                currentWeaponScript.SetWeaponStats(newDamage, newAttackSpeed);
            }
        }

        public void SetWeaponHolder(Transform newHolder)
        {
            if (newHolder == null) return;
            weaponHolder = newHolder;
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.transform.SetParent(weaponHolder);
                currentWeaponInstance.transform.localPosition = Vector3.zero;
                currentWeaponInstance.transform.localRotation = Quaternion.identity;
            }
        }

        public string GetWeaponInstanceStatus()
        {
            if (!HasWeaponEquipped)
                return "未装备武器";
            return currentWeaponScript?.GetWeaponStatus() ?? "武器脚本未找到";
        }

        public void TriggerAttack()
        {
            if (currentWeaponScript != null)
            {
                currentWeaponScript.TryAttack();
            }
        }

        public bool CanAttack()
        {
            return currentWeaponScript?.CanAttack ?? false;
        }

        [ContextMenu("显示武器状态")]
        public void ShowWeaponStatus()
        {
            Debug.Log(GetWeaponInstanceStatus());
        }

        [ContextMenu("测试攻击")]
        public void TestAttack()
        {
            TriggerAttack();
        }

        private void OnDestroy()
        {
            // 清理事件监听
            SceneManager.sceneLoaded -= OnSceneLoaded;
            DestroyCurrentWeaponInstance();
        }
    }
}