using System.Collections;
using UnityEngine;

public abstract class PowerUpBase : Collectible
{
    [Header("Power-Up")]
    [SerializeField] protected float duration = 5f;

    protected override void OnCollected(Collider player)
    {
        PlayPickupSound();
        ApplyEffect(player);
    }

    protected abstract void ApplyEffect(Collider player);
    protected virtual void PlayPickupSound() { }

    protected void ScheduleDeactivation(System.Action onExpire)
    {
        GameManager.Instance?.StartCoroutine(DeactivateRoutine(onExpire));
    }

    private IEnumerator DeactivateRoutine(System.Action onExpire)
    {
        yield return new WaitForSeconds(duration);
        onExpire?.Invoke();
    }
}