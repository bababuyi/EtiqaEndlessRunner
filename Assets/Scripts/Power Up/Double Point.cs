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

                GameManager.Instance?.ActivateDoublePoints(duration);
                AudioManager.Instance?.PlayDoubleCoin();
/*
            else
            {
                Debug.LogWarning("DoublePointsManager is missing from the scene!");
            }
*/
            // Play double points power-up sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance?.PlayDoubleCoin();
            }

            Destroy(gameObject);
        }
    }
}