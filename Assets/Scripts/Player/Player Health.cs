using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float speedReduction = 3f; // How much speed decreases on first hit
    public float recoveryTime = 5f; // Time before the player fully recovers
    public float hitWindow = 3f; // Time in which a second hit causes game over
    public float invincibilityDuration = 5f; // Duration of invincibility when power-up is collected


    private Movement movementScript; // Reference to Movement script

    private bool isHurt = false;
    private bool isInvincible = false;

    void Start()
    {
        movementScript = GetComponent<Movement>();
        if (movementScript == null)
            Debug.LogError("Movement script not found on player!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // Press 'H' to take damage manually
        {
            StartCoroutine(TakeDamage());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle hit!");
            if (!isInvincible)
            {
                StartCoroutine(TakeDamage());
            }
            else
            {
                Debug.Log("Player is invincible! No damage taken.");
            }
        }
    }

    IEnumerator TakeDamage()
    {
        if (isHurt) // Player is already hurt, so they die
        {
            Debug.Log("Player hit again within hit window! GAME OVER.");
 
            AudioManager.Instance?.PlayHit();
            AudioManager.Instance?.PlayLose();

            GameOver();
            yield break;
        }

        Debug.Log("Player stumbled! Speed reduced.");
        isHurt = true;

        // Reduce speed
        TileManager.Instance.WorldSpeed = Mathf.Max(5f,TileManager.Instance.WorldSpeed - speedReduction);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance?.PlayHit();
        }

        // Try to move to a free lane
        bool moved = TryMoveToFreeLane();

        if (!moved)
        {
            Debug.Log("No free lane found. Player stumbles instead.");
        }

        yield return new WaitForSeconds(hitWindow);

        if (isHurt) // If player wasn't hit again, they recover
        {
            Debug.Log("Player recovered!");
            isHurt = false;
        }

        yield return new WaitForSeconds(recoveryTime - hitWindow);
        Debug.Log("Player is now vulnerable again.");
    }

    public void ActivateInvincibility()
    {
        if (!isInvincible)
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        Debug.Log("Invincibility Activated! Player is now invincible.");

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        Debug.Log("Invincibility Expired! Player is now vulnerable.");
    }

    bool TryMoveToFreeLane()
    {
        Debug.Log("Checking for a free lane...");

        // Make sure we do not check lanes that don't exist
        bool canMoveLeft = movementScript.targetLane > 0;
        bool canMoveRight = movementScript.targetLane < 2;

        bool leftBlocked = canMoveLeft && IsLaneBlocked(movementScript.targetLane - 1);
        bool rightBlocked = canMoveRight && IsLaneBlocked(movementScript.targetLane + 1);

        // Ensure we don't move right if we're already at the rightmost lane
        if (canMoveRight && !rightBlocked)
        {
            Debug.Log("➡ Right lane is free! Moving right.");
            movementScript.ChangeLane(1);
            return true;
        }

        // Ensure we don't move left if we're already at the leftmost lane
        if (canMoveLeft && !leftBlocked)
        {
            Debug.Log("⬅ Left lane is free! Moving left.");
            movementScript.ChangeLane(-1);
            return true;
        }

        Debug.Log("🚧 No free lane found. Player will stumble in place.");
        return false;
    }

    bool IsLaneBlocked(int laneIndex)
    {
        Vector3 lanePosition = new Vector3((laneIndex - 1) * movementScript.laneDistance, transform.position.y, transform.position.z);

        Debug.Log("Checking obstacles in lane " + laneIndex + " at position: " + lanePosition);

        Collider[] hitColliders = Physics.OverlapSphere(lanePosition, 1f); // Check obstacles in a small area

        foreach (Collider hit in hitColliders)
        {
            Debug.Log("Object found: " + hit.gameObject.name + " in lane " + laneIndex);

            if (hit.CompareTag("Obstacle")) // Checking specifically for the "Obstacle" tag
            {
                Debug.Log("🚧 OBSTACLE DETECTED in lane " + laneIndex + " - " + hit.gameObject.name);
                return true;
            }
        }

        Debug.Log("✅ Lane " + laneIndex + " is CLEAR!");
        return false;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER! Player lost the run.");
        GameManager.Instance?.TriggerGameOver();
    }
}