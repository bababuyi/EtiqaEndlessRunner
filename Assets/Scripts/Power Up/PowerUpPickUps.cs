using UnityEngine;

public class FlyPickup : PowerUpPickup
{
    [SerializeField] private float fallbackFlightHeight = 5f;
    [SerializeField] private float fallbackDuration = 5f;

    protected override PowerUpEffect CreateEffect()
    {
        float height = ShopManager.Instance != null
            ? ShopManager.Instance.GetStat(PowerUpType.Fly)
            : fallbackFlightHeight;
        float duration = ShopManager.Instance != null
            ? ShopManager.Instance.GetDuration(PowerUpType.Fly)
            : fallbackDuration;

        return new FlyEffect(height, duration);
    }

    protected override void PlaySound() => AudioManager.Instance?.PlayFlyPowerUp();
}

public class InvincibilityPickup : PowerUpPickup
{
    [SerializeField] private float fallbackDuration = 5f;

    protected override PowerUpEffect CreateEffect()
    {
        float duration = ShopManager.Instance != null
            ? ShopManager.Instance.GetDuration(PowerUpType.Invincibility)
            : fallbackDuration;

        return new InvincibilityEffect(duration);
    }

    protected override void PlaySound() => AudioManager.Instance?.PlayInvincibility();
}

public class HighJumpPickup : PowerUpPickup
{
    [SerializeField] private float fallbackJumpMultiplier = 1.5f;
    [SerializeField] private float fallbackDuration = 5f;

    protected override PowerUpEffect CreateEffect()
    {
        float multiplier = ShopManager.Instance != null
            ? ShopManager.Instance.GetStat(PowerUpType.HighJump)
            : fallbackJumpMultiplier;
        float duration = ShopManager.Instance != null
            ? ShopManager.Instance.GetDuration(PowerUpType.HighJump)
            : fallbackDuration;

        return new HighJumpEffect(multiplier, duration);
    }

    protected override void PlaySound() => AudioManager.Instance?.PlayHighJump();
}

public class DoublePointsPickup : PowerUpPickup
{
    [SerializeField] private float fallbackDuration = 5f;

    protected override PowerUpEffect CreateEffect()
    {
        float duration = ShopManager.Instance != null
            ? ShopManager.Instance.GetDuration(PowerUpType.DoublePoints)
            : fallbackDuration;

        return new DoublePointsEffect(duration);
    }

    protected override void PlaySound() => AudioManager.Instance?.PlayDoubleCoin();
}