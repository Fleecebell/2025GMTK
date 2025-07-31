using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// UI管理器 (SRP: 只负责UI切换)
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("UIManager");
                        instance = go.AddComponent<UIManager>();
                    }
                }
                return instance;
            }
        }

        private Dictionary<System.Type, UIPanel> panels = new Dictionary<System.Type, UIPanel>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePanels();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializePanels()
        {
            // 查找并注册所有UI面板
            UIPanel[] foundPanels = FindObjectsOfType<UIPanel>();
            foreach (UIPanel panel in foundPanels)
            {
                panels[panel.GetType()] = panel;
                panel.Hide(); // 初始时隐藏所有面板
            }
        }

        /// <summary>
        /// 显示指定类型的UI面板
        /// </summary>
        /// <typeparam name="T">UI面板类型</typeparam>
        public void ShowPanel<T>() where T : UIPanel
        {
            System.Type panelType = typeof(T);
            if (panels.ContainsKey(panelType))
            {
                panels[panelType].Show();
            }
            else
            {
                Debug.LogWarning($"未找到类型为 {panelType.Name} 的UI面板");
            }
        }

        /// <summary>
        /// 隐藏指定类型的UI面板
        /// </summary>
        /// <typeparam name="T">UI面板类型</typeparam>
        public void HidePanel<T>() where T : UIPanel
        {
            System.Type panelType = typeof(T);
            if (panels.ContainsKey(panelType))
            {
                panels[panelType].Hide();
            }
        }

        /// <summary>
        /// 隐藏所有UI面板
        /// </summary>
        public void HideAllPanels()
        {
            foreach (var panel in panels.Values)
            {
                panel.Hide();
            }
        }

        /// <summary>
        /// 获取指定类型的UI面板
        /// </summary>
        /// <typeparam name="T">UI面板类型</typeparam>
        /// <returns>UI面板实例</returns>
        public T GetPanel<T>() where T : UIPanel
        {
            System.Type panelType = typeof(T);
            if (panels.ContainsKey(panelType))
            {
                return panels[panelType] as T;
            }
            return null;
        }
    }
}