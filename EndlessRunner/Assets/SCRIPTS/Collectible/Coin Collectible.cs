using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    public int coinValue = 1; // Base coin value

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int finalCoinValue = coinValue;

            if (DoublePointManager.Instance != null && DoublePointManager.Instance.IsDoublePointsActive())
            {
                finalCoinValue *= 2; // Double the coin value
                Debug.Log("Double Points Applied! Coin Value: " + finalCoinValue);
            }

            Debug.Log("Coin Collected! Value: " + finalCoinValue);

            // Update total saved coins
            CoinManager.Instance.AddToTotalCoins(finalCoinValue);

            // Play coin collect sound using GameSoundManager
            if (GameSoundManager.instance != null)
            {
                GameSoundManager.instance.PlaySound(GameSoundManager.instance.coinCollectSound);
            }

            // Destroy the coin
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
}
