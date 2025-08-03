using UnityEngine;
using InventorySystem.Items;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 武器数据设置示例
    /// 展示如何正确设置WeaponData的attackInstance字段
    /// </summary>
    public class WeaponDataSetupExample : MonoBehaviour
    {
        [Header("示例武器数据")]
        [SerializeField] private WeaponData swordWeaponData;
        [SerializeField] private WeaponData bowWeaponData;
        [SerializeField] private WeaponData spearWeaponData;

        [Header("武器预制体")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private GameObject spearPrefab;

        /// <summary>
        /// 设置长剑武器数据
        /// </summary>
        [ContextMenu("设置长剑武器数据")]
        public void SetupSwordWeaponData()
        {
            if (swordWeaponData != null && swordPrefab != null)
            {
                // 在编辑器中设置attackInstance
                #if UNITY_EDITOR
                var serializedObject = new UnityEditor.SerializedObject(swordWeaponData);
                var attackInstanceProperty = serializedObject.FindProperty("attackInstance");
                attackInstanceProperty.objectReferenceValue = swordPrefab;
                serializedObject.ApplyModifiedProperties();
                #endif

                Debug.Log($"已设置长剑武器数据: {swordWeaponData.ItemName}");
                Debug.Log($"Attack Instance: {(swordWeaponData.attackInstance != null ? swordWeaponData.attackInstance.name : "未设置")}");
            }
            else
            {
                Debug.LogWarning("长剑武器数据或预制体为空");
            }
        }

        /// <summary>
        /// 设置弓箭武器数据
        /// </summary>
        [ContextMenu("设置弓箭武器数据")]
        public void SetupBowWeaponData()
        {
            if (bowWeaponData != null && bowPrefab != null)
            {
                // 在编辑器中设置attackInstance
                #if UNITY_EDITOR
                var serializedObject = new UnityEditor.SerializedObject(bowWeaponData);
                var attackInstanceProperty = serializedObject.FindProperty("attackInstance");
                attackInstanceProperty.objectReferenceValue = bowPrefab;
                serializedObject.ApplyModifiedProperties();
                #endif

                Debug.Log($"已设置弓箭武器数据: {bowWeaponData.ItemName}");
                Debug.Log($"Attack Instance: {(bowWeaponData.attackInstance != null ? bowWeaponData.attackInstance.name : "未设置")}");
            }
            else
            {
                Debug.LogWarning("弓箭武器数据或预制体为空");
            }
        }

        /// <summary>
        /// 设置长枪武器数据
        /// </summary>
        [ContextMenu("设置长枪武器数据")]
        public void SetupSpearWeaponData()
        {
            if (spearWeaponData != null && spearPrefab != null)
            {
                // 在编辑器中设置attackInstance
                #if UNITY_EDITOR
                var serializedObject = new UnityEditor.SerializedObject(spearWeaponData);
                var attackInstanceProperty = serializedObject.FindProperty("attackInstance");
                attackInstanceProperty.objectReferenceValue = spearPrefab;
                serializedObject.ApplyModifiedProperties();
                #endif

                Debug.Log($"已设置长枪武器数据: {spearWeaponData.ItemName}");
                Debug.Log($"Attack Instance: {(spearWeaponData.attackInstance != null ? spearWeaponData.attackInstance.name : "未设置")}");
            }
            else
            {
                Debug.LogWarning("长枪武器数据或预制体为空");
            }
        }

        /// <summary>
        /// 设置所有武器数据
        /// </summary>
        [ContextMenu("设置所有武器数据")]
        public void SetupAllWeaponData()
        {
            SetupSwordWeaponData();
            SetupBowWeaponData();
            SetupSpearWeaponData();
        }

        /// <summary>
        /// 验证武器数据设置
        /// </summary>
        [ContextMenu("验证武器数据设置")]
        public void ValidateWeaponDataSetup()
        {
            Debug.Log("=== 武器数据验证 ===");

            if (swordWeaponData != null)
            {
                Debug.Log($"长剑武器: {swordWeaponData.ItemName}");
                Debug.Log($"  Attack Instance: {(swordWeaponData.attackInstance != null ? swordWeaponData.attackInstance.name : "未设置")}");
                Debug.Log($"  攻击力: {swordWeaponData.Damage}");
                Debug.Log($"  攻击速度: {swordWeaponData.AttackSpeed}");
            }

            if (bowWeaponData != null)
            {
                Debug.Log($"弓箭武器: {bowWeaponData.ItemName}");
                Debug.Log($"  Attack Instance: {(bowWeaponData.attackInstance != null ? bowWeaponData.attackInstance.name : "未设置")}");
                Debug.Log($"  攻击力: {bowWeaponData.Damage}");
                Debug.Log($"  攻击速度: {bowWeaponData.AttackSpeed}");
            }

            if (spearWeaponData != null)
            {
                Debug.Log($"长枪武器: {spearWeaponData.ItemName}");
                Debug.Log($"  Attack Instance: {(spearWeaponData.attackInstance != null ? spearWeaponData.attackInstance.name : "未设置")}");
                Debug.Log($"  攻击力: {spearWeaponData.Damage}");
                Debug.Log($"  攻击速度: {spearWeaponData.AttackSpeed}");
            }
        }

        /// <summary>
        /// 手动设置武器数据的attackInstance
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        /// <param name="prefab">武器预制体</param>
        public void SetWeaponAttackInstance(WeaponData weaponData, GameObject prefab)
        {
            if (weaponData != null && prefab != null)
            {
                weaponData.attackInstance = prefab;
                Debug.Log($"已设置武器 {weaponData.ItemName} 的Attack Instance为 {prefab.name}");
            }
            else
            {
                Debug.LogWarning("武器数据或预制体为空");
            }
        }

        /// <summary>
        /// 创建示例武器数据
        /// </summary>
        [ContextMenu("创建示例武器数据")]
        public void CreateSampleWeaponData()
        {
            Debug.Log("请在Project窗口中右键 -> Create -> Inventory -> Weapon Data 来创建武器数据");
            Debug.Log("然后设置以下字段：");
            Debug.Log("1. Item Name: 武器名称");
            Debug.Log("2. Item Description: 武器描述");
            Debug.Log("3. Item Icon: 武器图标");
            Debug.Log("4. Attack Type: 攻击类型");
            Debug.Log("5. Damage: 攻击力");
            Debug.Log("6. Attack Speed: 攻击速度");
            Debug.Log("7. Attack Instance: 武器预制体（重要！）");
        }
    }
} 