using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float speedReduction = 3f;
    public float recoveryTime = 5f;
    public float hitWindow = 3f;
   
    private Movement movementScript;

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
        if (Input.GetKeyDown(KeyCode.H))
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
        if (isHurt)
        {
            Debug.Log("Player hit again within hit window! GAME OVER.");
 
            AudioManager.Instance?.PlayHit();
            AudioManager.Instance?.PlayLose();

            GameOver();
            yield break;
        }

        Debug.Log("Player stumbled! Speed reduced.");
        isHurt = true;

        TileManager.Instance.WorldSpeed = Mathf.Max(5f,TileManager.Instance.WorldSpeed - speedReduction);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance?.PlayHit();
        }

        bool moved = TryMoveToFreeLane();

        if (!moved)
        {
            Debug.Log("No free lane found. Player stumbles instead.");
        }

        yield return new WaitForSeconds(hitWindow);

        if (isHurt)
        {
            Debug.Log("Player recovered!");
            isHurt = false;
        }

        yield return new WaitForSeconds(recoveryTime - hitWindow);
        Debug.Log("Player is now vulnerable again.");
    }

    public void SetInvincible(bool active)
    {
        isInvincible = active;
    }

    bool TryMoveToFreeLane()
    {
        Debug.Log("Checking for a free lane...");

        bool canMoveLeft = movementScript.targetLane > 0;
        bool canMoveRight = movementScript.targetLane < 2;

        bool leftBlocked = canMoveLeft && IsLaneBlocked(movementScript.targetLane - 1);
        bool rightBlocked = canMoveRight && IsLaneBlocked(movementScript.targetLane + 1);

        if (canMoveRight && !rightBlocked)
        {
            Debug.Log("➡ Right lane is free! Moving right.");
            movementScript.ChangeLane(1);
            return true;
        }

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

        Collider[] hitColliders = Physics.OverlapSphere(lanePosition, 1f);

        foreach (Collider hit in hitColliders)
        {
            Debug.Log("Object found: " + hit.gameObject.name + " in lane " + laneIndex);

            if (hit.CompareTag("Obstacle"))
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