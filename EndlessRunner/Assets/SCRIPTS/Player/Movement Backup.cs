using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBackup : MonoBehaviour
{
    // Movement parameters
    public float forwardSpeed = 5f; // Initial speed
    public float maxSpeed = 15f; // Maximum speed the player can reach
    public float speedIncreaseDuration = 30f; // Time in seconds to reach max speed

    public float laneDistance = 3f; // Distance between lanes
    public float laneSwitchSpeed = 10f; // Speed of lane switching

    // Jump & Roll mechanics
    public float jumpForce = 7f; // Force applied when jumping
    public float rollDuration = 0.8f; // Duration of the roll action
    public float rollSpeedMultiplier = 1.5f; // Speed multiplier during roll

    // Lane system (0 = Left, 1 = Middle, 2 = Right)
    public int targetLane = 1; // Player starts in the middle lane
    private Vector3 targetPosition;
    private Vector2 touchStartPos; // Stores the initial touch position
    private bool isSwiping = false; // Track if player is swiping

    // Player state variables
    public bool isJumping = false;
    public bool isRolling = false;
    public bool isFlying = false;

    // Components
    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private Vector3 originalColliderSize;
    private Vector3 originalColliderCenter;
    private Material playerMaterial;
    private Color originalColor;
    private float defaultGravity;

    void Start()
    {
        // Get the Rigidbody and Collider components
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();


        // Store original collider size and position for resetting later
        originalColliderSize = playerCollider.height * Vector3.up;
        originalColliderCenter = playerCollider.center;
        defaultGravity = rb.useGravity ? 1f : 0f;

        Debug.Log("Game Started - PlayerMovement Initialized");

        rb.isKinematic = false;

        StartCoroutine(IncreaseSpeedOverTime());

        // Get player's material to change color for power-ups
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            playerMaterial = renderer.material;
            originalColor = playerMaterial.color;
        }
    }

    void Update()
    {
        if (rb.isKinematic)
        {
            Debug.LogWarning("Rigidbody is Kinematic! Turning off Kinematic mode.");
            rb.isKinematic = false; // Disable Kinematic if it is on (so that physics works)
        }

        MoveForward(); // Constant forward movement
        HandleInput(); // Handles player input (swipes, keys, mouse)
        MoveToTargetLane(); // Moves the player to the correct lane smoothly
    }

    // Moves the player forward automatically
    void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    // Handles touch, keyboard, and mouse input
    void HandleInput()
    {
        // Mobile Touch Input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position; // Store touch start position
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                Vector2 swipeDelta = touch.position - touchStartPos;

                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y)) // Horizontal swipe
                {
                    if (swipeDelta.x > 50) ChangeLane(1); // Swipe right
                    else if (swipeDelta.x < -50) ChangeLane(-1); // Swipe left
                }
                else // Vertical swipe
                {
                    if (swipeDelta.y > 50) Jump(); // Swipe up
                    else if (swipeDelta.y < -50) StartCoroutine(Roll()); // Swipe down
                }

                isSwiping = false;
            }
        }

        // Keyboard Input for testing
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Jump
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl)) // Roll
        {
            StartCoroutine(Roll());
        }

        // Mouse Input (Left Click = Left, Right Click = Right)
        if (Input.GetMouseButtonDown(0)) ChangeLane(-1); // Left Click
        if (Input.GetMouseButtonDown(1)) ChangeLane(1); // Right Click
    }

    // Changes the player's lane
    public void ChangeLane(int direction)
    {
        targetLane += direction;
        targetLane = Mathf.Clamp(targetLane, 0, 2); // Keeps the player within the 3 lanes
    }

    // Moves the player towards the target lane
    public void MoveToTargetLane()
    {
        float targetX = (targetLane - 1) * laneDistance;
        targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, laneSwitchSpeed * Time.deltaTime);
    }

    // Handles jumping
    void Jump()
    {
        if (!isJumping && !isFlying) // Prevent double jumping
        {
            isJumping = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            Debug.Log("Player Jumped");
            StartCoroutine(ResetJump());
        }
    }

    // Resets jump state after a delay
    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
        Debug.Log("Jump Reset");
    }

    // Handles rolling
    IEnumerator Roll()
    {
        if (!isRolling)
        {
            isRolling = true;
            Debug.Log("Player Rolling");

            // Shrink the collider for rolling
            playerCollider.height *= 0.5f;
            playerCollider.center -= Vector3.up * 0.5f;

            // Increase speed temporarily
            forwardSpeed *= rollSpeedMultiplier;

            yield return new WaitForSeconds(rollDuration);

            // Reset collider and speed
            playerCollider.height = originalColliderSize.y;
            playerCollider.center = originalColliderCenter;
            forwardSpeed /= rollSpeedMultiplier;

            isRolling = false;
            Debug.Log("Roll Reset");
        }
    }

    IEnumerator IncreaseSpeedOverTime()
    {
        float startTime = Time.time;
        float startSpeed = forwardSpeed;

        while (Time.time - startTime < speedIncreaseDuration)
        {
            forwardSpeed = Mathf.Lerp(startSpeed, maxSpeed, (Time.time - startTime) / speedIncreaseDuration);
            yield return null;
        }

        forwardSpeed = maxSpeed; // Ensure the final speed is exactly maxSpeed
    }

    // Activates Flying Power-Up
    public void ActivateFlyingPowerUp(float flightHeight, float duration)
    {
        if (!isFlying)
        {
            StartCoroutine(Fly(flightHeight, duration));
        }
    }

    IEnumerator Fly(float flightHeight, float duration)
    {
        Debug.Log("Flying Power-Up Activated!");
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
        Debug.Log("Player landed. Flying power-up ended.");
    }

    public void ResetPlayer()
    {
        Debug.Log("ResetPlayer() called");

        // Reset player position and state
        transform.position = new Vector3(0, 0, 0); // Set to initial position
        forwardSpeed = 5f;
        maxSpeed = 15f;
        speedIncreaseDuration = 30f;
        isJumping = false;
        isRolling = false;
        isFlying = false;

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;

        // Reset collider size if changed during roll
        playerCollider.height = originalColliderSize.y;
        playerCollider.center = originalColliderCenter;

        // Ensure the Rigidbody is not Kinematic
        if (rb.isKinematic)
        {
            Debug.LogWarning("Rigidbody was Kinematic, turning off.");
            rb.isKinematic = false;
        }

        Debug.Log("Player position and state reset");
    }
}