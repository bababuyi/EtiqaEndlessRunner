using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float speedReduction = 3f;
    public float recoveryTime = 5f;
    public float hitWindow = 3f;
   
    private Movement movementScript;

    private bool isHurt = false;
    private bool isInvincible = false;

    public event Action<int, int> OnHealthChanged;
    public int MaxHP { get; private set; } = 2;
    public int CurrentHP { get; private set; }

    void Start()
    {
        movementScript = GetComponent<Movement>();
        if (movementScript == null)
            Debug.LogError("Movement script not found on player!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && !isInvincible)
        {
            StartCoroutine(TakeDamage());
        }
    }

    IEnumerator TakeDamage()
    {
        if (isHurt)
        {
            CurrentHP = 0;
            OnHealthChanged?.Invoke(CurrentHP, MaxHP);;
 
            AudioManager.Instance?.PlayHit();
            AudioManager.Instance?.PlayLose();

            GameOver();
            yield break;
        }

        isHurt = true;
        CurrentHP = 1;
        OnHealthChanged?.Invoke(CurrentHP, MaxHP);

        TileManager.Instance.WorldSpeed = Mathf.Max(5f,TileManager.Instance.WorldSpeed - speedReduction);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance?.PlayHit();
        }

        TryMoveToFreeLane();

        yield return new WaitForSeconds(hitWindow);

        if (isHurt)
        {
            isHurt = false;
        }

        yield return new WaitForSeconds(recoveryTime - hitWindow);
    }

    public void SetInvincible(bool active)
    {
        isInvincible = active;
    }

    bool TryMoveToFreeLane()
    {
        bool canMoveLeft = movementScript.targetLane > 0;
        bool canMoveRight = movementScript.targetLane < 2;

        bool leftBlocked = canMoveLeft && IsLaneBlocked(movementScript.targetLane - 1);
        bool rightBlocked = canMoveRight && IsLaneBlocked(movementScript.targetLane + 1);

        if (canMoveRight && !rightBlocked)
        {
            movementScript.ChangeLane(1);
            return true;
        }

        if (canMoveLeft && !leftBlocked)
        {
            movementScript.ChangeLane(-1);
            return true;
        }
        return false;
    }

    bool IsLaneBlocked(int laneIndex)
    {
        Vector3 lanePosition = new Vector3((laneIndex - 1) * movementScript.laneDistance, transform.position.y, transform.position.z);

        Collider[] hitColliders = Physics.OverlapSphere(lanePosition, 1f);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Obstacle"))
            {
                return true;
            }
        }

        return false;
    }

    void GameOver()
    {
        GameManager.Instance?.TriggerGameOver();
    }
}