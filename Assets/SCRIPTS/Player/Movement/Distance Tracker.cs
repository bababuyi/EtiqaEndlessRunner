using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceTracker : MonoBehaviour
{
    private Vector3 lastPosition;
    private float totalDistance = 0f;

    public Text distanceText; // UI Text to display the distance score
    public static float finalDistance; // Stores the final score for Game Over

    void Start()
    {
        lastPosition = transform.position; // Save the starting position
        totalDistance = 0f;
    }

    void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        totalDistance += distanceMoved;
        lastPosition = transform.position;

        // Update UI
        if (distanceText != null)
        {
            distanceText.text = "Distance: " + Mathf.Round(totalDistance) + " m";
        }
        finalDistance = totalDistance; // Store final distance for Game Over screen
        Debug.Log("Total Distance Moved: " + Mathf.Round(totalDistance) + " meters");
    }

    public float GetTotalDistance()
    {
        return Mathf.Round(totalDistance); // Return rounded value
    }
}