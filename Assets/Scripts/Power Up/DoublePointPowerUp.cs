using UnityEngine;

public class DoublePointsPowerUp : PowerUpBase
{
    protected override void ApplyEffect(Collider player)
    {
        GameManager.Instance?.SetDoublePoints(true);
        ScheduleDeactivation(() => GameManager.Instance?.SetDoublePoints(false));
    }

    protected override void PlayPickupSound()
    {
        AudioManager.Instance?.PlayDoubleCoin();
    }
}