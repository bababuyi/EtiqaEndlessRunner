using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float laneDistance = 3f;
    public float laneSwitchSpeed = 10f;

    public float jumpForce = 7f;
    public float rollDuration = 0.8f;
    public float rollSpeedMultiplier = 1.5f;

    public int targetLane = 1;
    private Vector3 targetPosition;
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    // Player state variables
    private bool isJumping = false;
    private bool isRolling = false;
    private bool isFlying = false;

    // Expose these states as public properties
    public bool IsJumping => isJumping;
    public bool IsRolling => isRolling;
    public bool IsFlying => isFlying;

    // Components
    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private Vector3 originalColliderSize;
    private Vector3 originalColliderCenter;
    private float defaultGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        originalColliderSize = playerCollider.height * Vector3.up;
        originalColliderCenter = playerCollider.center;
        defaultGravity = rb.useGravity ? 1f : 0f;

        rb.isKinematic = false;
    }

    void Update()
    {
        if (rb.isKinematic)
        {
            rb.isKinematic = false;
        }

        HandleInput();
        MoveToTargetLane();
    }

    

    // Handles input
    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                Vector2 delta = touch.position - touchStartPos;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 50) ChangeLane(1);
                    else if (delta.x < -50) ChangeLane(-1);
                }
                else
                {
                    if (delta.y > 50) Jump();
                    else if (delta.y < -50) StartCoroutine(Roll());
                }
                isSwiping = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(Roll());
        }
    }

    // Changes the player's lane
    public void ChangeLane(int direction)
    {
        targetLane += direction;
        targetLane = Mathf.Clamp(targetLane, 0, 2);
    }

    public void MoveToTargetLane()
    {
        float targetX = (targetLane - 1) * laneDistance;
        targetPosition = new Vector3(targetX, transform.position.y, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, laneSwitchSpeed * Time.deltaTime);
    }

    // Handles jumping
    void Jump()
    {
        if (!isJumping && !isFlying)
        {
            isJumping = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            StartCoroutine(ResetJump());
        }
    }

    // Resets jump state after a delay
    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    // Handles rolling
    IEnumerator Roll()
    {
        if (!isRolling)
        {
            isRolling = true;

            float originalHeight = playerCollider.height;
            Vector3 originalCenter = playerCollider.center;

            playerCollider.height *= 0.5f;
            playerCollider.center = new Vector3(playerCollider.center.x, 1f, playerCollider.center.z);
            TileManager.Instance.WorldSpeed *= rollSpeedMultiplier;

            yield return new WaitForSeconds(rollDuration);

            playerCollider.height = originalHeight;
            playerCollider.center = originalCenter;
            TileManager.Instance.WorldSpeed /= rollSpeedMultiplier;

            isRolling = false;
        }
    }

    // Activates Flying Power-Up (if needed)
    public void ActivateFlyingPowerUp(float flightHeight, float duration)
    {
        if (!isFlying)
        {
            StartCoroutine(Fly(flightHeight, duration));
        }
    }

    IEnumerator Fly(float flightHeight, float duration)
    {
        isFlying = true;
        rb.useGravity = false;

        float targetY = transform.position.y + flightHeight;
        while (transform.position.y < targetY - 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), Time.deltaTime * 5f);
            yield return null;
        }
        yield return new WaitForSeconds(duration);

        rb.useGravity = true;
        isFlying = false;
    }

    // Resets the player's position and state (if needed)
    public void ResetPlayer()
    {
        transform.position = new Vector3(0, 1, 0);
        isJumping = false;
        isRolling = false;
        isFlying = false;

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;

        playerCollider.height = originalColliderSize.y;
        playerCollider.center = originalColliderCenter;

        if (rb.isKinematic)
            rb.isKinematic = false;
    }
}