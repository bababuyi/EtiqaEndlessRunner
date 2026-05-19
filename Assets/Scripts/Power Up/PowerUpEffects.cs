using UnityEngine;

public class FlyEffect : PowerUpEffect
{
    public override PowerUpType Type => PowerUpType.Fly;
    public override float Duration => _duration;

    private readonly float _height;
    private readonly float _duration;

    public FlyEffect(float height, float duration)
    {
        _height = height;
        _duration = duration;
    }

    public override void Apply(GameObject player) => player.GetComponent<Movement>()?.StartFlying(_height);
    public override void Remove(GameObject player) => player.GetComponent<Movement>()?.StopFlying();
}

public class InvincibilityEffect : PowerUpEffect
{
    public override PowerUpType Type => PowerUpType.Invincibility;
    public override float Duration => _duration;

    private readonly float _duration;

    public InvincibilityEffect(float duration) => _duration = duration;

    public override void Apply(GameObject player) => player.GetComponent<PlayerHealth>()?.SetInvincible(true);
    public override void Remove(GameObject player) => player.GetComponent<PlayerHealth>()?.SetInvincible(false);
}

public class HighJumpEffect : PowerUpEffect
{
    public override PowerUpType Type => PowerUpType.HighJump;
    public override float Duration => _duration;

    private readonly float _multiplier;
    private readonly float _duration;
    private float _originalJumpForce;

    public HighJumpEffect(float multiplier, float duration)
    {
        _multiplier = multiplier;
        _duration = duration;
    }

    public override void Apply(GameObject player)
    {
        Movement movement = player.GetComponent<Movement>();
        if (movement == null) return;

        _originalJumpForce = movement.jumpForce;
        movement.jumpForce *= _multiplier;
    }

    public override void Remove(GameObject player)
    {
        Movement movement = player.GetComponent<Movement>();
        if (movement == null) return;

        movement.jumpForce = _originalJumpForce;
    }
}

public class DoublePointsEffect : PowerUpEffect
{
    public override PowerUpType Type => PowerUpType.DoublePoints;
    public override float Duration => _duration;

    private readonly float _duration;

    public DoublePointsEffect(float duration) => _duration = duration;

    public override void Apply(GameObject player) => GameManager.Instance?.SetDoublePoints(true);
    public override void Remove(GameObject player) => GameManager.Instance?.SetDoublePoints(false);
}