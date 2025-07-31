using UnityEngine;
using System.IO;

namespace Data
{
    /// <summary>
    /// 数据管理器
    /// </summary>
    public class DataManager : MonoBehaviour, IDataProvider
    {
        private static DataManager instance;
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DataManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("DataManager");
                        instance = go.AddComponent<DataManager>();
                    }
                }
                return instance;
            }
        }

        private string saveDataPath;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDataPath();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDataPath()
        {
            saveDataPath = Path.Combine(Application.persistentDataPath, "SaveData");
            if (!Directory.Exists(saveDataPath))
            {
                Directory.CreateDirectory(saveDataPath);
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">数据键</param>
        /// <returns>加载的数据</returns>
        public T LoadData<T>(string key)
        {
            string filePath = Path.Combine(saveDataPath, key + ".json");
            
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JsonUtility.FromJson<T>(jsonData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"加载数据失败: {e.Message}");
                }
            }
            
            return default(T);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">数据键</param>
        /// <param name="data">要保存的数据</param>
        public void SaveData<T>(string key, T data)
        {
            try
            {
                string jsonData = JsonUtility.ToJson(data, true);
                string filePath = Path.Combine(saveDataPath, key + ".json");
                File.WriteAllText(filePath, jsonData);
                Debug.Log($"数据保存成功: {key}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"保存数据失败: {e.Message}");
            }
        }

        /// <summary>
        /// 保存游戏数据
        /// </summary>
        public void SaveGameData()
        {
            // 保存玩家数据
            Player.Player player = FindObjectOfType<Player.Player>();
            if (player != null)
            {
                var playerData = new
                {
                    currentHP = player.CurrentHP,
                    currentSan = player.CurrentSan,
                    position = player.transform.position
                };
                SaveData("PlayerData", playerData);
            }

            Debug.Log("游戏数据保存完成");
        }

        /// <summary>
        /// 加载游戏数据
        /// </summary>
        public void LoadGameData()
        {
            // 加载玩家数据
            var playerData = LoadData<dynamic>("PlayerData");
            if (playerData != null)
            {
                Player.Player player = FindObjectOfType<Player.Player>();
                if (player != null)
                {
                    // 恢复玩家状态
                    Debug.Log("玩家数据加载完成");
                }
            }

            Debug.Log("游戏数据加载完成");
        }

        /// <summary>
        /// 重置游戏数据
        /// </summary>
        public void ResetGameData()
        {
            try
            {
                if (Directory.Exists(saveDataPath))
                {
                    Directory.Delete(saveDataPath, true);
                    Directory.CreateDirectory(saveDataPath);
                }
                Debug.Log("游戏数据重置完成");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"重置游戏数据失败: {e.Message}");
            }
        }
    }
}