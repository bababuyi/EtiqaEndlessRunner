using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePointManager : MonoBehaviour
{
    public static DoublePointManager Instance;
    private bool doublePointsActive = false;
    private float doublePointsEndTime = 0f;

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

    void Update()
    {
        if (doublePointsActive && Time.time > doublePointsEndTime)
        {
            doublePointsActive = false;
            Debug.Log("Double Points Effect Wore Off!");
        }
    }

    public void ActivateDoublePoints(float duration)
    {
        doublePointsActive = true;
        doublePointsEndTime = Time.time + duration;

        Debug.Log("Double Points Activated for " + duration + " seconds!");
    }

    public bool IsDoublePointsActive()
    {
        return doublePointsActive;
    }
}
