using UnityEngine;

public class FlyPickup : PowerUpPickup
{
    [SerializeField] private float flightHeight = 5f;
    [SerializeField] private float duration = 5f;

    protected override PowerUpEffect CreateEffect() => new FlyEffect(flightHeight, duration);
    protected override void PlaySound() => AudioManager.Instance?.PlayFlyPowerUp();
}

public class InvincibilityPickup : PowerUpPickup
{
    [SerializeField] private float duration = 5f;

    protected override PowerUpEffect CreateEffect() => new InvincibilityEffect(duration);
    protected override void PlaySound() => AudioManager.Instance?.PlayInvincibility();
}

public class HighJumpPickup : PowerUpPickup
{
    [SerializeField] private float jumpMultiplier = 1.5f;
    [SerializeField] private float duration = 5f;

    protected override PowerUpEffect CreateEffect() => new HighJumpEffect(jumpMultiplier, duration);
    protected override void PlaySound() => AudioManager.Instance?.PlayHighJump();
}

public class DoublePointsPickup : PowerUpPickup
{
    [SerializeField] private float duration = 5f;

    protected override PowerUpEffect CreateEffect() => new DoublePointsEffect(duration);
    protected override void PlaySound() => AudioManager.Instance?.PlayDoubleCoin();
}