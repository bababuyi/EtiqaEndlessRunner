using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    public int coinValue = 1; // Base coin value

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance?.AddCoins(coinValue);
        AudioManager.Instance?.PlayCoinCollect();
        Destroy(gameObject);
    }

    void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
}
