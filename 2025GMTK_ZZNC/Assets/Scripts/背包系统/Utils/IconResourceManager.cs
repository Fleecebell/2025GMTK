using UnityEngine;
using System.Collections.Generic;

namespace InventorySystem.Utils
{
    /// <summary>
    /// 图标资源管理器 - 处理图标的加载、缓存和默认图标管理
    /// </summary>
    public static class IconResourceManager
    {
        // 图标缓存
        private static Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();
        
        // 默认图标
        private static Sprite defaultWeaponIcon;
        private static Sprite defaultEquipmentIcon;
        private static Sprite defaultConsumableIcon;
        
        // 路径常量
        public static class Paths
        {
            public const string WEAPON_SPRITES = "Assets/Resources/Sprites/Weapons/";
            public const string EQUIPMENT_SPRITES = "Assets/Resources/Sprites/Equipment/";
            public const string CONSUMABLE_SPRITES = "Assets/Resources/Sprites/Consumables/";
            public const string DEFAULT_ICONS = "Assets/Resources/Sprites/DefaultIcons/";
        }

        /// <summary>
        /// 初始化图标管理器
        /// </summary>
        public static void Initialize()
        {
            LoadDefaultIcons();
        }

        /// <summary>
        /// 加载默认图标
        /// </summary>
        private static void LoadDefaultIcons()
        {
            #if UNITY_EDITOR
            defaultWeaponIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{Paths.DEFAULT_ICONS}default_weapon.png");
            defaultEquipmentIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{Paths.DEFAULT_ICONS}default_equipment.png");
            defaultConsumableIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"{Paths.DEFAULT_ICONS}default_consumable.png");
            #endif
        }

        /// <summary>
        /// 加载武器图标
        /// </summary>
        /// <param name="iconName">图标名称</param>
        /// <returns>图标Sprite</returns>
        public static Sprite LoadWeaponIcon(string iconName)
        {
            return LoadIcon(Paths.WEAPON_SPRITES, iconName, defaultWeaponIcon);
        }

        /// <summary>
        /// 加载装备图标
        /// </summary>
        /// <param name="iconName">图标名称</param>
        /// <returns>图标Sprite</returns>
        public static Sprite LoadEquipmentIcon(string iconName)
        {
            return LoadIcon(Paths.EQUIPMENT_SPRITES, iconName, defaultEquipmentIcon);
        }

        /// <summary>
        /// 加载消耗品图标
        /// </summary>
        /// <param name="iconName">图标名称</param>
        /// <returns>图标Sprite</returns>
        public static Sprite LoadConsumableIcon(string iconName)
        {
            return LoadIcon(Paths.CONSUMABLE_SPRITES, iconName, defaultConsumableIcon);
        }

        /// <summary>
        /// 通用图标加载方法
        /// </summary>
        /// <param name="basePath">基础路径</param>
        /// <param name="iconName">图标名称</param>
        /// <param name="defaultIcon">默认图标</param>
        /// <returns>图标Sprite</returns>
        private static Sprite LoadIcon(string basePath, string iconName, Sprite defaultIcon)
        {
            if (string.IsNullOrEmpty(iconName))
                return defaultIcon;

            string cacheKey = $"{basePath}{iconName}";
            
            // 检查缓存
            if (iconCache.TryGetValue(cacheKey, out Sprite cachedIcon))
            {
                return cachedIcon ?? defaultIcon;
            }

            // 加载图标
            Sprite loadedIcon = null;
            
            #if UNITY_EDITOR
            string fullPath = $"{basePath}{iconName}.png";
            loadedIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            #else
            // 运行时使用Resources.Load
            string resourcePath = GetResourcePath(basePath, iconName);
            loadedIcon = Resources.Load<Sprite>(resourcePath);
            #endif

            // 缓存结果（即使是null也缓存，避免重复加载）
            iconCache[cacheKey] = loadedIcon;
            
            return loadedIcon ?? defaultIcon;
        }

        /// <summary>
        /// 获取Resources路径
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <param name="iconName">图标名称</param>
        /// <returns>Resources相对路径</returns>
        private static string GetResourcePath(string fullPath, string iconName)
        {
            // 将Assets/Resources/路径转换为Resources.Load可用的路径
            if (fullPath.StartsWith("Assets/Resources/"))
            {
                string resourcePath = fullPath.Substring("Assets/Resources/".Length);
                return $"{resourcePath}{iconName}";
            }
            
            return $"Sprites/{iconName}";
        }

        /// <summary>
        /// 预加载常用图标
        /// </summary>
        /// <param name="iconNames">图标名称列表</param>
        public static void PreloadIcons(string[] iconNames)
        {
            foreach (string iconName in iconNames)
            {
                LoadWeaponIcon(iconName);
                LoadEquipmentIcon(iconName);
                LoadConsumableIcon(iconName);
            }
        }

        /// <summary>
        /// 清理图标缓存
        /// </summary>
        public static void ClearCache()
        {
            iconCache.Clear();
        }

        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存信息</returns>
        public static string GetCacheStats()
        {
            int totalCached = iconCache.Count;
            int nullCached = 0;
            
            foreach (var kvp in iconCache)
            {
                if (kvp.Value == null)
                    nullCached++;
            }
            
            return $"图标缓存统计: 总计 {totalCached}, 有效 {totalCached - nullCached}, 无效 {nullCached}";
        }

        /// <summary>
        /// 验证图标是否存在
        /// </summary>
        /// <param name="basePath">基础路径</param>
        /// <param name="iconName">图标名称</param>
        /// <returns>是否存在</returns>
        public static bool ValidateIconExists(string basePath, string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
                return true;

            #if UNITY_EDITOR
            string fullPath = $"{basePath}{iconName}.png";
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(fullPath) != null;
            #else
            string resourcePath = GetResourcePath(basePath, iconName);
            return Resources.Load<Sprite>(resourcePath) != null;
            #endif
        }

        /// <summary>
        /// 批量验证图标
        /// </summary>
        /// <param name="basePath">基础路径</param>
        /// <param name="iconNames">图标名称列表</param>
        /// <returns>缺失的图标列表</returns>
        public static List<string> ValidateIcons(string basePath, string[] iconNames)
        {
            List<string> missingIcons = new List<string>();
            
            foreach (string iconName in iconNames)
            {
                if (!ValidateIconExists(basePath, iconName))
                {
                    missingIcons.Add(iconName);
                }
            }
            
            return missingIcons;
        }

        /// <summary>
        /// 创建默认图标文件夹和占位图标
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void CreateDefaultIconStructure()
        {
            #if UNITY_EDITOR
            // 创建文件夹结构
            EnsureDirectoryExists(Paths.WEAPON_SPRITES);
            EnsureDirectoryExists(Paths.EQUIPMENT_SPRITES);
            EnsureDirectoryExists(Paths.CONSUMABLE_SPRITES);
            EnsureDirectoryExists(Paths.DEFAULT_ICONS);
            
            // 创建占位图标的说明文件
            string readmePath = $"{Paths.DEFAULT_ICONS}README.txt";
            if (!System.IO.File.Exists(readmePath))
            {
                string readmeContent = @"默认图标文件夹

请在此文件夹中放置以下默认图标：
- default_weapon.png (武器默认图标)
- default_equipment.png (装备默认图标)  
- default_consumable.png (消耗品默认图标)

这些图标将在找不到指定图标时作为备用图标使用。

建议图标尺寸：64x64 像素
格式：PNG，支持透明背景";
                
                System.IO.File.WriteAllText(readmePath, readmeContent);
                UnityEditor.AssetDatabase.Refresh();
            }
            #endif
        }

        #if UNITY_EDITOR
        /// <summary>
        /// 确保目录存在
        /// </summary>
        private static void EnsureDirectoryExists(string path)
        {
            if (!UnityEditor.AssetDatabase.IsValidFolder(path))
            {
                string[] pathParts = path.TrimEnd('/').Split('/');
                string currentPath = pathParts[0];
                
                for (int i = 1; i < pathParts.Length; i++)
                {
                    string nextPath = currentPath + "/" + pathParts[i];
                    if (!UnityEditor.AssetDatabase.IsValidFolder(nextPath))
                    {
                        UnityEditor.AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                    }
                    currentPath = nextPath;
                }
            }
        }
        #endif
    }
}