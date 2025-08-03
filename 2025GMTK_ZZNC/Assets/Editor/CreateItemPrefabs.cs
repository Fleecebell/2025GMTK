#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using InventorySystem.Items;

public class CreateItemPrefabs : EditorWindow
{
    [MenuItem("Tools/Create Item Prefabs")]
    static void Open() => GetWindow<CreateItemPrefabs>("Create Prefabs");

    void OnGUI()
    {
        if (GUILayout.Button("一键生成所有物品预制体"))
        {
            GenerateAllPrefabs();
        }
    }

    static void GenerateAllPrefabs()
    {
        // 1. 收集所有 ScriptableObject 资源
        string dataRoot = "Assets/Resources/Data/Items";
        var allAssets = AssetDatabase.FindAssets("t:ScriptableObject", new[] { dataRoot })
                                     .Select(AssetDatabase.GUIDToAssetPath)
                                     .Select(AssetDatabase.LoadAssetAtPath<BaseItemData>)
                                     .Where(d => d != null)
                                     .ToList();

        if (!allAssets.Any())
        {
            Debug.LogWarning("未找到任何 BaseItemData！");
            return;
        }

        foreach (var data in allAssets)
        {
            CreateSinglePrefab(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("所有物品预制体生成完毕！");
    }

    static void CreateSinglePrefab(BaseItemData data)
    {
        // 根据运行时类型决定文件夹
        string typeFolder = GetTypeFolder(data);
        string prefabDir = $"Assets/Resources/Items/{typeFolder}";
        if (!Directory.Exists(prefabDir)) Directory.CreateDirectory(prefabDir);

        string prefabPath = $"{prefabDir}/{data.itemName}.prefab";

        // 如果已存在则覆盖
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            AssetDatabase.DeleteAsset(prefabPath);

        // 创建对象
        GameObject go = new GameObject(data.itemName);

        // 组件
        var sr      = go.AddComponent<SpriteRenderer>();
        var col     = go.AddComponent<CircleCollider2D>();
        var itemScr = go.AddComponent<Item>();

        // 设置
        sr.sprite   = data.icon;
        col.radius  = 0.5f;
        col.isTrigger = true;
        itemScr.SetItemData(data);

        // 保存为 Prefab
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

        // 立即销毁临时对象
        DestroyImmediate(go);
    }

    static string GetTypeFolder(BaseItemData data)
    {
        return data switch
        {
            WeaponData     => "Weapon",
            EquipmentData  => "Equipment",
            ConsumableData => "Consumable",
            _              => "Other"
        };
    }
}
#endif