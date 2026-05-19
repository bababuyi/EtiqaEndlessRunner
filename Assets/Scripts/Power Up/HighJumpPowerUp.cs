using UnityEngine;

public class HighJumpPowerUp : PowerUpBase
{
    [SerializeField] private float jumpMultiplier = 1.5f;

    protected override void ApplyEffect(Collider player)
    {
        var movement = player.GetComponent<Movement>();
        if (movement == null) return;

        float original = movement.jumpForce;
        movement.jumpForce *= jumpMultiplier;
        ScheduleDeactivation(() => movement.jumpForce = original);
    }

    protected override void PlayPickupSound()
    {
        AudioManager.Instance?.PlayHighJump();
    }
}