using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyPower : MonoBehaviour
{
    public float flightHeight = 5f; // How high the player should fly
    public float duration = 5f; // How long the player stays in the air
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
            Debug.Log("Flying Power-Up Collected!");

            // Get the player's Movement script and activate flying
            Movement playerMovement = other.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.ActivateFlyingPowerUp(flightHeight, duration);
            }
            else
            {
                Debug.LogWarning("Player's Movement script is missing!");
            }

            // Play flying power-up sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance?.PlayFlyPowerUp();
            }

            Destroy(gameObject);
        }
    }
}
