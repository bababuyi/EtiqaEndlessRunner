using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpUpgradeRow : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private string displayName = "Power-Up";

    [Header("UI References")]
    [SerializeField] private Image[] bars;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private TextMeshProUGUI statLabel;
    [SerializeField] private Button levelUpButton;

    [Header("Bar Colours")]
    [SerializeField] private Color filledColour = Color.white;
    [SerializeField] private Color emptyColour = new Color(0.3f, 0.3f, 0.3f, 1f);

    private void Awake()
    {
        levelUpButton.onClick.AddListener(OnLevelUpClicked);
    }

    public void Refresh()
    {
        if (ShopManager.Instance == null) return;
        int level = ShopManager.Instance.GetLevel(powerUpType);
        int cost = ShopManager.Instance.GetNextUpgradeCost(powerUpType);
        bool maxed = level >= ShopManager.MaxLevel;
        bool canAfford = !maxed &&
                         GameManager.Instance != null &&
                         GameManager.Instance.TotalCoins >= cost;

        if (nameLabel != null)
            nameLabel.text = displayName;

        for (int i = 0; i < bars.Length; i++)
            bars[i].color = i < level ? filledColour : emptyColour;

        if (costLabel != null)
            costLabel.text = maxed ? "MAX" : cost + " coins";

        if (statLabel != null)
            statLabel.text = BuildStatLabel(level);

        levelUpButton.interactable = canAfford;
    }

    private void OnLevelUpClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        ShopManager.Instance?.TryUpgrade(powerUpType);
    }

    private string BuildStatLabel(int level)
    {
        float duration = ShopManager.Instance.GetDuration(powerUpType);
        float stat = ShopManager.Instance.GetStat(powerUpType);

        return powerUpType switch
        {
            PowerUpType.Fly => $"Duration: {duration:0.#}s  |  Height: {stat:0.#}",
            PowerUpType.HighJump => $"Duration: {duration:0.#}s  |  Multiplier: {stat:0.##}x",
            PowerUpType.Invincibility => $"Duration: {duration:0.#}s",
            PowerUpType.DoublePoints => $"Duration: {duration:0.#}s",
            _ => $"Duration: {duration:0.#}s"
        };
    }
}