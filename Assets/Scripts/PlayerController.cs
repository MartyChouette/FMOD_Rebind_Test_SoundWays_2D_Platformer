using FMODUnity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Horizontal movement speed")]
    public float moveSpeed = 8f;
    [Tooltip("How quickly the player accelerates")]
    public float acceleration = 60f;
    [Tooltip("How quickly the player decelerates when no input is given")]
    public float deceleration = 70f;

    [Header("Jump Settings")]
    [Tooltip("Initial jump velocity")]
    public float jumpForce = 16f;
    [Tooltip("Normal gravity scale when airborne")]
    public float gravityScale = 3f;
    [Tooltip("Multiplier for gravity when falling")]
    public float fallMultiplier = 2.5f;
    [Tooltip("Multiplier for gravity when releasing the jump early")]
    public float lowJumpMultiplier = 2f;
    [Tooltip("Time allowed after leaving a platform to still jump (coyote time)")]
    public float coyoteTime = 0.1f;
    [Tooltip("Time window for buffering a jump input before landing")]
    public float jumpBufferTime = 0.1f;

    [Header("Ground Check Settings")]
    [Tooltip("Position to check if the player is grounded (usually a child transform at the feet)")]
    public Transform groundCheck;
    [Tooltip("Radius of the ground check")]
    public float groundCheckRadius = 0.2f;
    [Tooltip("Layer mask defining what is ground")]
    public LayerMask groundLayer;

    // Internal timers
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    

    // Cached components
    private Rigidbody2D rb;
    private float moveInput;
    void Awake()
    {
        // Synchronously load the master bank so events are ready to be triggered immediately.
        RuntimeManager.LoadBank("SFX", true);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Set gravity scale on start (adjust as needed)
        rb.gravityScale = gravityScale;
    }

    private void Move()
    {
        moveInput = UserInput.instance.MoveInput.x;

        if (moveInput > 0 || moveInput < 0)
        {
            //anim.SetBool("isWalking", true);
            //TurnCheck();
        }
        else
        {
            //anim.SetBool("isWalking", false);
        }

        rb.linearVelocity = new Vector2(moveInput = moveSpeed, rb.linearVelocity.y);

    }

    private void Jump()
    {
        ////buttons was pushed this frame
        //if (UserInput.instance.JumpJustPressed && IsGrounded())
        //{
        //    isJumping = true;
        //    JumpTimeCounter = jumpTime;
        //    rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);

        //    //anim.SetTrigger("jump");
        //}
    }

    void Update()
    {
        
        // Jump input buffering: if the player pressed jump, store the buffer timer.
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
            Debug.Log("Jump input detected");

        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // --- Ground Check & Coyote Time ---
        // Check if the groundCheck circle overlaps with any colliders on the groundLayer.
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            Debug.Log("Grounded!");
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // --- Jumping ---
        // If jump was buffered and we are within coyote time, perform a jump.
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            // Reset jump buffering once used.
            jumpBufferCounter = 0f;
        }

        // --- Enhanced Jump Physics ---
        // Apply extra gravity when falling for a snappier descent.
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Allow for a shorter jump if the player releases the jump button early.
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // --- Horizontal Movement ---
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        // Calculate target horizontal speed.
        float targetSpeed = horizontalInput * moveSpeed;
        // Determine the difference between current velocity and target speed.
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        // Choose the proper acceleration (or deceleration) rate.
        float rate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        // Smoothly adjust the velocity towards the target.
        float movement = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, rate * Time.fixedDeltaTime);

        // Update the Rigidbody�s velocity while preserving vertical motion.
        rb.linearVelocity = new Vector2(movement, rb.linearVelocity.y);
    }

    // Optional: Visualize the ground check in the editor.
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
