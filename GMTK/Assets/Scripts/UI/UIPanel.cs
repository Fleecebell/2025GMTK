using UnityEngine;

namespace UI
{
    /// <summary>
    /// UI面板基类 (Template Method)
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        [SerializeField] protected bool isVisible = false;

        protected virtual void Awake()
        {
            Initialize();
            BindEvents();
        }

        public abstract void Initialize();

        public virtual void Show()
        {
            gameObject.SetActive(true);
            isVisible = true;
            Debug.Log($"显示UI面板：{GetType().Name}");
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            isVisible = false;
            Debug.Log($"隐藏UI面板：{GetType().Name}");
        }

        protected abstract void BindEvents();

        public bool IsVisible => isVisible;
    }
}