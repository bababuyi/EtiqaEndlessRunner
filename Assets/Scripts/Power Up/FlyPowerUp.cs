using UnityEngine;

public class FlyPowerUp : PowerUpBase
{
    [SerializeField] private float flightHeight = 5f;

    protected override void ApplyEffect(Collider player)
    {
        var movement = player.GetComponent<Movement>();
        if (movement == null) return;

        movement.StartFlying(flightHeight);
        ScheduleDeactivation(() => movement.StopFlying());
    }

    protected override void PlayPickupSound()
    {
        AudioManager.Instance?.PlayFlyPowerUp();
    }
}