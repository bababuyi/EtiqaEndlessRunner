using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityMain : MonoBehaviour
{
    public float rotationSpeed = 50f; // Speed of rotation

    private void Update()
    {
        // Rotate the power-up around the Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Invincibility Power-Up Collected!");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.ActivateInvincibility();
            }
            else
            {
                Debug.LogError("PlayerHealth script not found on Player!");
            }

            // Play invincibility power-up sound using GameSoundManager
            if (GameSoundManager.instance != null)
            {
                GameSoundManager.instance.PlaySound(GameSoundManager.instance.invincibilityPowerUpSound);
            }

            gameObject.SetActive(false); // Deactivate the power-up object
        }
    }
}