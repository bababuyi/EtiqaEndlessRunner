using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighJumpEffect : MonoBehaviour
{
    private Rigidbody rb;
    private float originalJumpForce;
    private float jumpMultiplier;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Destroy(this);
        }
    }

    public void Activate(float multiplier, float duration)
    {
        StartCoroutine(ApplyEffect(multiplier, duration));
    }

    IEnumerator ApplyEffect(float multiplier, float duration)
    {
        Movement playerMovement = GetComponent<Movement>();

        if (playerMovement != null)
        {
            originalJumpForce = playerMovement.jumpForce;
            jumpMultiplier = multiplier;
            playerMovement.jumpForce *= jumpMultiplier;
            Debug.Log("Higher Jump Activated! New Jump Force: " + playerMovement.jumpForce);

            yield return new WaitForSeconds(duration);

            playerMovement.jumpForce = originalJumpForce; // Reset after duration
            Debug.Log("Higher Jump Wore Off! Jump Force Reset to: " + playerMovement.jumpForce);

            Destroy(this); // Remove script after power-up expires
        }
    }
}
