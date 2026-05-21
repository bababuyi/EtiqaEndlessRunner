using UnityEngine;
using System;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public event Action<PowerUpType, int> OnUpgradePurchased;

    public const int MaxLevel = 10;

    private static readonly int[] UpgradeCosts ={ 50, 75, 100, 150, 200, 275, 350, 450, 600, 800 };

    private readonly Dictionary<PowerUpType, UpgradeDef> _defs =
        new Dictionary<PowerUpType, UpgradeDef>
    {
        { PowerUpType.Fly,           new UpgradeDef(baseDuration: 5f,  perLevelDuration: 1f,  baseStat: 5f,  perLevelStat: 0f)  },
        { PowerUpType.HighJump,      new UpgradeDef(baseDuration: 5f,  perLevelDuration: 0.5f, baseStat: 1.5f, perLevelStat: 0.1f) },
        { PowerUpType.Invincibility, new UpgradeDef(baseDuration: 5f,  perLevelDuration: 1f,  baseStat: 0f,  perLevelStat: 0f)  },
        { PowerUpType.DoublePoints,  new UpgradeDef(baseDuration: 5f,  perLevelDuration: 1f,  baseStat: 0f,  perLevelStat: 0f)  },
    };

    private Dictionary<PowerUpType, int> _levels = new Dictionary<PowerUpType, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllLevels();
    }

    public int GetLevel(PowerUpType type)
    {
        _levels.TryGetValue(type, out int lvl);
        return lvl;
    }

    public int GetNextUpgradeCost(PowerUpType type)
    {
        int lvl = GetLevel(type);
        if (lvl >= MaxLevel) return -1;
        return UpgradeCosts[lvl];
    }

    public float GetDuration(PowerUpType type)
    {
        var def = _defs[type];
        return def.BaseDuration + def.PerLevelDuration * GetLevel(type);
    }

    public float GetStat(PowerUpType type)
    {
        var def = _defs[type];
        return def.BaseStat + def.PerLevelStat * GetLevel(type);
    }

    public bool TryUpgrade(PowerUpType type)
    {
        int cost = GetNextUpgradeCost(type);
        if (cost < 0) return false;
        if (!GameManager.Instance.SpendCoins(cost)) return false;

        _levels[type]++;
        SaveLevel(type);
        OnUpgradePurchased?.Invoke(type, _levels[type]);
        return true;
    }

    private void LoadAllLevels()
    {
        foreach (PowerUpType type in Enum.GetValues(typeof(PowerUpType)))
            _levels[type] = PlayerPrefs.GetInt(PrefKey(type), 0);
    }

    private void SaveLevel(PowerUpType type) =>
        PlayerPrefs.SetInt(PrefKey(type), _levels[type]);

    private static string PrefKey(PowerUpType type) => "ShopLevel_" + type;

    private class UpgradeDef
    {
        public float BaseDuration;
        public float PerLevelDuration;
        public float BaseStat;
        public float PerLevelStat;

        public UpgradeDef(float baseDuration, float perLevelDuration,
                          float baseStat, float perLevelStat)
        {
            BaseDuration = baseDuration;
            PerLevelDuration = perLevelDuration;
            BaseStat = baseStat;
            PerLevelStat = perLevelStat;
        }
    }
}