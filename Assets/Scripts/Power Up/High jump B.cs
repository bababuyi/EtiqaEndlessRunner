using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highjump : MonoBehaviour
{
    public float jumpMultiplier = 1.5f; // How much to increase the jump height
    public float duration = 5f; // How long the effect lasts
    public float rotationSpeed = 50f; // Speed of rotation

    private void Update()
    {
        // Rotate the power-up around the Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                HighJumpEffect effect = other.gameObject.AddComponent<HighJumpEffect>();
                effect.Activate(jumpMultiplier, duration);
            }

            // Play high jump power-up sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance?.PlayHighJump();
            }

            Destroy(gameObject);
        }
    }
}
