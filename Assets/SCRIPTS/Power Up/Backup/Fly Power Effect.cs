using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyPowerEffect : MonoBehaviour
{
    private Rigidbody rb;
    private Movement playerMovement;
    private float flightHeight;
    private float duration;
    private bool originalGravityState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<Movement>();

        if (rb == null)
        {
            Debug.LogError("FlyingEffect ERROR: No Rigidbody found on player! Trying to find it...");
            rb = gameObject.AddComponent<Rigidbody>(); // Ensure Rigidbody exists

            if (rb == null)
            {
                Debug.LogError("FlyingEffect ERROR: Failed to add Rigidbody! Destroying effect.");
                Destroy(this);
                return;
            }
            else
            {
                Debug.Log("FlyingEffect: Rigidbody was missing but has now been added!");
            }
        }

        Debug.Log("FlyingEffect: Rigidbody found, script is running.");
    }

    public void Activate(float height, float time)
    {
        Debug.Log("FlyingEffect: Activate() called. Flight height: " + height + ", Duration: " + time);

        flightHeight = height;
        duration = time;
        StartCoroutine(ApplyEffect()); // Start the flying effect
    }

    IEnumerator ApplyEffect()
    {
        Debug.Log("FlyingEffect: ApplyEffect() started.");

        if (rb == null)
        {
            Debug.LogError("FlyingEffect ERROR: Rigidbody is STILL null. Exiting coroutine.");
            yield break;
        }

        // Disable player movement temporarily
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            Debug.Log("FlyingEffect: Player movement disabled.");
        }

        // Save gravity state
        originalGravityState = rb.useGravity;
        rb.useGravity = false;
        Debug.Log("FlyingEffect: Gravity disabled.");

        // Move player up smoothly
        float startY = transform.position.y;
        float targetY = startY + flightHeight;

        Debug.Log("FlyingEffect: Starting lift-off. Target height: " + targetY);

        while (transform.position.y < targetY - 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), Time.deltaTime * 5f);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        Debug.Log("FlyingEffect: Player reached flight height!");

        yield return new WaitForSeconds(duration); // Wait for power-up to expire
        Debug.Log("FlyingEffect: Flight duration over, descending...");

        // Smoothly lower the player back to the ground
        while (transform.position.y > startY + 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, startY, transform.position.z), Time.deltaTime * 3f);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        Debug.Log("FlyingEffect: Player is back on the ground.");

        // Restore gravity and re-enable movement
        rb.useGravity = originalGravityState;
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("FlyingEffect: Player movement re-enabled.");
        }

        Debug.Log("FlyingEffect: Power-up effect ended.");
        Destroy(this); // Remove script after effect ends
    }
}