using UnityEngine;

public class InvincibilityPowerUp : PowerUpBase
{
    protected override void ApplyEffect(Collider player)
    {
        var health = player.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.SetInvincible(true);
        ScheduleDeactivation(() => health.SetInvincible(false));
    }

    protected override void PlayPickupSound()
    {
        AudioManager.Instance?.PlayInvincibility();
    }
}