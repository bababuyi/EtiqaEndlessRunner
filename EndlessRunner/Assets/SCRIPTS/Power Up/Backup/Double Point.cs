using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePoint : MonoBehaviour
{
    public float duration = 5f; // Duration of double points effect
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
            Debug.Log("Double Points Power-Up Collected!");

            // Use Singleton for easier access
            if (DoublePointManager.Instance != null)
            {
                DoublePointManager.Instance.ActivateDoublePoints(duration);
            }
            else
            {
                Debug.LogWarning("DoublePointsManager is missing from the scene!");
            }

            // Play double points power-up sound
            if (GameSoundManager.instance != null)
            {
                GameSoundManager.instance.PlaySound(GameSoundManager.instance.doubleCoinPowerUpSound);
            }

            Destroy(gameObject);
        }
    }
}