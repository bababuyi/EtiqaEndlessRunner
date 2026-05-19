using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private readonly Dictionary<PowerUpType, (PowerUpEffect effect, Coroutine coroutine)> _active = new();

    public void ApplyEffect(PowerUpEffect effect)
    {
        if (_active.TryGetValue(effect.Type, out var existing))
        {
            StopCoroutine(existing.coroutine);
            existing.effect.Remove(gameObject);
            _active.Remove(effect.Type);
        }

        effect.Apply(gameObject);
        var coroutine = StartCoroutine(ExpireAfter(effect));
        _active[effect.Type] = (effect, coroutine);
    }

    public bool IsActive(PowerUpType type) => _active.ContainsKey(type);

    public void ClearAll()
    {
        foreach (var entry in _active)
        {
            StopCoroutine(entry.Value.coroutine);
            entry.Value.effect.Remove(gameObject);
        }
        _active.Clear();
    }

    private IEnumerator ExpireAfter(PowerUpEffect effect)
    {
        yield return new WaitForSeconds(effect.Duration);
        effect.Remove(gameObject);
        _active.Remove(effect.Type);
    }
}