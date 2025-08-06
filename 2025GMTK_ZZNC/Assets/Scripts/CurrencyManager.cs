using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    // 单例
    public static CurrencyManager Instance { get; private set; }

    // 玩家的货币数
    public int currencyAmount;
    public TextMeshProUGUI currencyText;


    private void Awake()
    {
        // 确保单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景持久化
        }
        else
        {
            Destroy(gameObject); // 销毁重复实例
        }
        currencyAmount = 0;
        UpdateCurrencyUI();
    }

    // 购买方法：扣除货币
    public bool Purchase(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("购买金额必须大于0！");
            return false;
        }

        if (currencyAmount >= amount)
        {
            currencyAmount -= amount;
            Debug.Log($"购买成功，扣除 {amount} 货币，剩余 {currencyAmount} 货币");
            UpdateCurrencyUI();
            return true;
        }
        else
        {
            Debug.Log("货币不足，购买失败！");
            return false;
        }
    }
    public bool HasEnoughCurrency(int price) {

        return currencyAmount >= price;
    }

    // 获取货币方法：增加货币
    public void GainCurrency(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("获取的货币数量必须大于0！");
            return;
        }

        currencyAmount += amount;
        Debug.Log($"获得 {amount} 货币，当前总货币数为 {currencyAmount}");
        UpdateCurrencyUI();
    }

    // 更新货币 UI
    private void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            StartCoroutine(AnimateCurrencyChange());
        }
    }

    // 货币数量变化的动效
    private IEnumerator AnimateCurrencyChange()
    {
        // 保存原始文本
        string originalText = currencyText.text;

        // 设置目标文本
        currencyText.text = currencyAmount.ToString();

        // 动效：逐渐放大和缩小
        float duration = 0.5f; // 动效持续时间
        float elapsedTime = 0f;
        float originalScale = currencyText.transform.localScale.x;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / duration;
            float scale = Mathf.Lerp(originalScale, originalScale * 1.2f, progress);

            currencyText.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        // 恢复原始大小
        currencyText.transform.localScale = new Vector3(originalScale, originalScale, originalScale);
    }
}