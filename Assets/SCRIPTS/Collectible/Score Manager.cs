using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float scoreMultiplier = 1f; // Points per unit of distance
    private float distanceTraveled = 0f;
    private Vector3 lastPosition;
    private int score = 0;

    public static ScoreManager Instance { get; private set; } // Singleton for easy access
    public static int FinalScore { get; private set; } // Store final score

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate distance traveled since last frame
        float distance = Vector3.Distance(transform.position, lastPosition);
        distanceTraveled += distance;
        lastPosition = transform.position;

        // Update score
        score = Mathf.RoundToInt(distanceTraveled * scoreMultiplier);

    }

    public void SaveFinalScore()
    {
        FinalScore = score;
    }
}



