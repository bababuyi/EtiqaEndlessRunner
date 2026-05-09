using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int totalCoins = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadTotalCoins();
            DontDestroyOnLoad(gameObject); // Keeps it alive between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Press "J" to subtract 10 coins (for testing)
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpendCoins(10); // Adjust the amount as needed
        }
    }

    public void AddToTotalCoins(int amount)
    {
        totalCoins += amount;
        SaveCoins();
        Debug.Log("Total Coins Saved: " + totalCoins);
    }

    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            SaveCoins();
            Debug.Log("Spent " + amount + " coins. Remaining: " + totalCoins);
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }

    private void LoadTotalCoins()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        Debug.Log("Total Coins Loaded: " + totalCoins);
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
    }
}
