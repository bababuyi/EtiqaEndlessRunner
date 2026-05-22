using UnityEngine;

public enum PowerUpType { Fly, Invincibility, HighJump, DoublePoints }

public abstract class PowerUpEffect
{
    public abstract PowerUpType Type { get; }
    public abstract float Duration { get; }

    public abstract void Apply(GameObject player);
    public abstract void Remove(GameObject player);
}

public abstract class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PowerUpManager manager = other.GetComponent<PowerUpManager>();
        if (manager == null) return;

        manager.ApplyEffect(CreateEffect());
        PlaySound();
        Destroy(gameObject);
    }

    protected abstract PowerUpEffect CreateEffect();
    protected abstract void PlaySound();
}