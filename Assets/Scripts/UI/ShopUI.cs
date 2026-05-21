using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Coin Display")]
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Upgrade Rows — order must match PowerUpType enum")]
    [SerializeField] private PowerUpUpgradeRow[] upgradeRows;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCoinsUpdated += RefreshCoinDisplay;

        if (ShopManager.Instance != null)
            ShopManager.Instance.OnUpgradePurchased += HandleUpgradePurchased;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCoinsUpdated -= RefreshCoinDisplay;

        if (ShopManager.Instance != null)
            ShopManager.Instance.OnUpgradePurchased -= HandleUpgradePurchased;
    }

    public void OnShopOpened()
    {
        int coins = GameManager.Instance != null
        ? GameManager.Instance.TotalCoins
        : PlayerPrefs.GetInt("TotalCoins", 0);
        RefreshCoinDisplay(coins);

        foreach (var row in upgradeRows)
            row.Refresh();
    }

    private void RefreshCoinDisplay(int coins)
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins;
    }

    private void HandleUpgradePurchased(PowerUpType type, int newLevel)
    {
        foreach (var row in upgradeRows)
            row.Refresh();

        RefreshCoinDisplay(GameManager.Instance.TotalCoins);
    }
}