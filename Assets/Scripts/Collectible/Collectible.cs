using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Collectible : MonoBehaviour
{
    [Header("Spin")]
    [SerializeField] private float rotationSpeed = 100f;

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        OnCollected(other);
        Destroy(gameObject);
    }

    protected abstract void OnCollected(Collider player);
}