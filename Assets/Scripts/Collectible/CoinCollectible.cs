using UnityEngine;

public class CoinCollectible : Collectible
{
    [SerializeField] private int coinValue = 1;

    protected override void OnCollected(Collider player)
    {
        GameManager.Instance?.AddCoins(coinValue);
        AudioManager.Instance?.PlayCoinCollect();
    }
}